 using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.Mentor;
using final_project_be_Domain.DTOs.Payment;
using final_project_be_Domain.DTOs.Post;
using final_project_be_Domain.DTOs.Transaction;
using final_project_be_Domain.DTOs.Users;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Repository
{

    public class PaymentRepository:  IPaymentRepositoty
    {
        private readonly ICouponDAO _couponDAO;
        private readonly IUserDAO _userDAO;
        private readonly IUserCourseDAO _userCourseDAO;
        private readonly ICourseDAO _courseDAO;
        private readonly IMentorDAO _mentorDAO;
        private readonly IPaymentDAO _paymentDAO;
        private readonly IPaymentCourseDAO _paymentCourseDAO;
        private readonly ILogger<PaymentRepository> _logger;
        public PaymentRepository(IUserDAO userDAO, ICourseDAO courseDAO, IPaymentDAO paymentDAO, IPaymentCourseDAO paymentCourseDAO, ILogger<PaymentRepository> logger,IMentorDAO mentorDAO,ICouponDAO couponDAO, IUserCourseDAO userCourseDAO) 
        {
            _mentorDAO = mentorDAO;
            _userDAO = userDAO;
            _userCourseDAO = userCourseDAO;
            _courseDAO = courseDAO;
            _paymentDAO = paymentDAO;
            _paymentCourseDAO = paymentCourseDAO;
            _couponDAO = couponDAO;
            _logger = logger;
        }

        public async Task<BuyCourseResult> BuyCourseAsync(Guid userId, int courseId, int couponId)
        {
            try
            {
                await _paymentDAO.BeginTransactionAsync();

                // Lấy user và course
                var user = await _userDAO.GetByIdAsync(userId);
                var course = await _courseDAO.GetByIdAsync(courseId);

                if (user == null || course == null)
                {
                    await _paymentDAO.RollbackTransactionAsync();
                    return new BuyCourseResult { Success = false, Error = "NotFound" };
                }


                // ✅ Kiểm tra nếu đã mua khóa học
                var existingUserCourse = await _userCourseDAO.GetUserCourse(userId, courseId);
                if (existingUserCourse != null)
                {
                    var status = existingUserCourse.Status ?? string.Empty;
                    if (status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                    {
                        await _paymentDAO.RollbackTransactionAsync();
                        return new BuyCourseResult { Success = false, Error = "PreviouslyPurchased" };
                    }
                }

                // Tính toán giá sau khi giảm
                decimal originalCost = course.Cost;
                decimal finalCost = originalCost;

                if (couponId > 0)
                {
                    var coupon = await _couponDAO.GetByIdAsync(couponId);
                    if (coupon != null && coupon.Discount > 0)
                    {
                        finalCost = originalCost * (1 - (decimal)coupon.Discount / 100m);
                        if (finalCost < 0) finalCost = 0;
                    }
                }

                // ✅ Kiểm tra điểm
                if (user.Point < finalCost)
                {
                    await _paymentDAO.RollbackTransactionAsync();
                    return new BuyCourseResult { Success = false, Error = "NotEnoughPoint" };
                }

                // Lấy mentor
                var mentor = await _mentorDAO.GetMentorinCourseAsync(course.MentorId);
                if (mentor == null)
                {
                    await _paymentDAO.RollbackTransactionAsync();
                    return new BuyCourseResult { Success = false, Error = "MentorNotFound" };
                }

                // Tạo Payment
                var payment = new Payment
                {
                    UserId = userId,
                    Amount = finalCost,
                    Status = "Success",
                    ServiceType = "Course",
                    CreatedAt = DateTime.UtcNow
                };
                await _paymentDAO.AddAsync(payment);
                await _paymentDAO.SaveChangesAsync();

                // Ghi nhận vào bảng PaymentCourse
                var paymentCourse = new PaymentCourse
                {
                    PaymentId = payment.PaymentId,
                    CourseId = courseId,
                    CouponId = couponId > 0 ? couponId : null,
                    CreatedAt = DateTime.UtcNow
                };
                await _paymentCourseDAO.AddAsync(paymentCourse);

                // Ghi nhận vào bảng UserCourse
                await _userCourseDAO.AddUserCourseAsync(new UserCourse
                {
                    UserId = userId,
                    CourseId = courseId,
                    CompletedAt = DateTime.UtcNow,
                    Status = "Not Started"
                });

                course.StudentCount += 1;
                await _courseDAO.UpdateAsync(course);
                // Trừ điểm người dùng
                user.Point -= finalCost;
                await _userDAO.UpdateAsync(user);

                // Cộng điểm cho mentor
                mentor.User.Point += finalCost * 0.85m;
                await _mentorDAO.UpdateAsync(mentor);

                await _paymentDAO.SaveChangesAsync();
                await _paymentDAO.CommitTransactionAsync();

                return new BuyCourseResult { Success = true };
            }
            catch (Exception ex)
            {
                await _paymentDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when processing course purchase");
                return new BuyCourseResult { Success = false, Error = "Exception" };
            }
        }

        public PageResult<GetPaymentDto> GetAll(int page, int pageSize, Guid? UserId, string? sortOption, List<ServiceTypeEnum>? ServiceType)
        {
            try
            {
                var query = _paymentDAO.GetAll()
                    .Include(c => c.User)
                    .ThenInclude(c => c.UserMetaData)
                    .Include(c => c.PaymentCourses)
                    .ThenInclude(pc => pc.Courses)
                    .Include(c => c.PaymentPlans)
                    .ThenInclude(pp => pp.MembershipPlan)
                    .Where(p => ServiceType == null || ServiceType.Count == 0 || ServiceType.Select(s => s.ToString()).Contains(p.ServiceType));


                if (UserId.HasValue && UserId != Guid.Empty)
                    query = query.Where(p => p.UserId == UserId.Value);

                query = sortOption?.ToLower() switch
                {
                    "asc_date" => query.OrderBy(c => c.CreatedAt),
                    "desc_date" => query.OrderByDescending(c => c.CreatedAt),
                    _ => query.OrderByDescending(c => c.CreatedAt)
                };

                var totalCount = query.Count();

                var payment = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var paymentDto = payment.Select(p => new GetPaymentDto
                {
                    PaymentId = p.PaymentId,
                    UserId = p.UserId,
                    Email = p.User.Email,
					CourseId = p.PaymentCourses?.FirstOrDefault()?.CourseId ?? 0,
					CourseName = p.PaymentCourses?.FirstOrDefault()?.Courses?.CourseName ?? string.Empty,
                    PlanName = p.PaymentPlans?.FirstOrDefault()?.MembershipPlan?.Name ?? string.Empty,
					Amount = p.Amount,
                    Status = p.Status,
                    ServiceType = p.ServiceType,
                    CreatedAt = p.CreatedAt,
                }).ToList();

                _logger.LogInformation("Get filtered payment success");
                return new PageResult<GetPaymentDto>(paymentDto, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting filtered payments");
                return new PageResult<GetPaymentDto>(new List<GetPaymentDto>(), 0, page, pageSize);
            }
        }

        public async Task<List<MothlyStatPaymentDto>> GetStatisticsByMonth(int? year)
        {
            var query = _paymentDAO.GetAll()
                .Include(p => p.PaymentCourses)
                    .ThenInclude(pc => pc.Courses)
                        .ThenInclude(c => c.Modules)
                .Where(p => p.Status == "Success");

            if (year.HasValue)
            {
                query = query.Where(p => p.CreatedAt.Year == year.Value);
            }

            var payments = await query.ToListAsync();

            var stats = payments
                .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month })
                .Select(g => new MothlyStatPaymentDto
                {
                    Time = $"{g.Key.Month:D2}/{g.Key.Year}",
                    TotalPremium = g.Count(p => p.ServiceType == "Premium"),
                    TotalPoint = g.Sum(p =>
                    {
                        // Nếu là Premium thì tính 100%
                        if (p.ServiceType == "Premium")
                        {
                            return p.Amount * 1.0m;
                        }
                        // Nếu là Course thì tính 25% hoặc 35% tùy điều kiện
                        else if (p.ServiceType == "Course")
                        {
                            var course = p.PaymentCourses?.FirstOrDefault()?.Courses;
                            if (course == null || course.IsDeleted) return 0m;
                            bool hasPremiumModules = course.Modules?.Any(m => m.IsPremium) ?? false;
                            return p.Amount * (hasPremiumModules ? 0.25m : 0.35m);
                        }

                        return p.Amount * 1.0m;
                    })
                })
                .OrderBy(s => s.Time)
                .ToList();

            return stats;
        }
    }

}

