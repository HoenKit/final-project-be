 using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.Mentor;
using final_project_be_Domain.DTOs.Notification;
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
        private readonly IPaymentPlanDAO _paymentPlanDAO;
        private readonly IMembershipPlanDAO _MembershipPlanDAO;
        private readonly ILogger<PaymentRepository> _logger;
        public PaymentRepository(IUserDAO userDAO, ICourseDAO courseDAO, IPaymentDAO paymentDAO, IPaymentPlanDAO paymentPlanDAO, IMembershipPlanDAO MembershipPlanDAO, IPaymentCourseDAO paymentCourseDAO, ILogger<PaymentRepository> logger,IMentorDAO mentorDAO,ICouponDAO couponDAO, IUserCourseDAO userCourseDAO) 
        {
            _mentorDAO = mentorDAO;
            _userDAO = userDAO;
            _userCourseDAO = userCourseDAO;
            _courseDAO = courseDAO;
            _paymentDAO = paymentDAO;
            _paymentCourseDAO = paymentCourseDAO;
            _paymentPlanDAO = paymentPlanDAO;
            _MembershipPlanDAO = MembershipPlanDAO;
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
                    await _paymentDAO.RollbackTransactionAsync();
                    return new BuyCourseResult
                    {
                        Success = false,
                        Error = "PreviouslyPurchased"
                    };
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

        public async Task<IEnumerable<MembershipPlan>> GetAllMembershipplanAsync()
        {
            try
            {
                var membershipPlans = await _MembershipPlanDAO.GetAll().ToListAsync();

                _logger.LogInformation("Get Membership plans success");

                return membershipPlans;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting Membership plans");
                return new List<MembershipPlan>();
            }
        }

        public async Task<bool> BuyPremiumAsync(Guid userId, int planId)
        {
            await _paymentDAO.BeginTransactionAsync();

            try
            {
                var user = await _userDAO.GetByIdAsync(userId);
                if (user == null) throw new Exception("User not found");

                var plan = await _MembershipPlanDAO.GetByIdAsync(planId);
                if (plan == null) throw new Exception("Membership plan not found");

                if (user.Point < plan.Price)
                    throw new Exception("Not enough points");

                // Trừ point và bật Premium
                user.Point -= plan.Price;
                user.IsPremium = true;
                await _userDAO.UpdateAsync(user);

                // Tạo Payment
                var payment = new Payment
                {
                    UserId = user.UserId,
                    Amount = plan.Price,
                    ServiceType = "Membership",
                    Status = "Success",
                    CreatedAt = DateTime.UtcNow
                };
                 await _paymentDAO.AddAsync(payment); // đảm bảo PaymentId được set

                // Tính ExpiredAt từ Name
                int months = ParseMonthsFromPlanName(plan.Name); // helper function
                var paymentPlan = new PaymentPlan
                {
                    PaymentId = payment.PaymentId,
                    PlanId = plan.PlanId,
                    CreatedAt = DateTime.UtcNow,
                    ExpiredAt = DateTime.UtcNow.AddMonths(months)
                };
                await _paymentPlanDAO.AddAsync(paymentPlan);

                await _paymentDAO.CommitTransactionAsync();
                return true;
            }
            catch
            {
                await _paymentDAO.RollbackTransactionAsync();
                throw;
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
                    hasPremiumModules = p.PaymentCourses?.FirstOrDefault()?.Courses?.Modules?.Any(m => m.IsPremium) ?? false,
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
            try
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

                _logger.LogInformation("Successfully generated payment statistics for {Count} months", stats.Count);
                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate payment statistics");
                throw;
            }
        }
        private int ParseMonthsFromPlanName(string planName)
        {
            // ví dụ: "6 Month Subscription"
            if (string.IsNullOrEmpty(planName)) return 1;
            var parts = planName.Split(' ');
            if (parts.Length < 1) return 1;

            if (int.TryParse(parts[0], out int months))
                return months;

            return 1;
        }
    }

}

