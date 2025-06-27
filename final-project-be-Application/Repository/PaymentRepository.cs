 using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Mentor;
using final_project_be_Domain.DTOs.Payment;
using final_project_be_Domain.DTOs.Post;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
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
        private readonly CouponDAO _couponDAO;
        private readonly UserDAO _userDAO;
        private readonly UserCourseDAO _userCourseDAO;
        private readonly CourseDAO _courseDAO;
        private readonly MentorDAO _mentorDAO;
        private readonly PaymentDAO _paymentDAO;
        private readonly PaymentCourseDAO _paymentCourseDAO;
        private readonly ILogger<PaymentRepository> _logger;
        public PaymentRepository(UserDAO userDAO, CourseDAO courseDAO, PaymentDAO paymentDAO, PaymentCourseDAO paymentCourseDAO, ILogger<PaymentRepository> logger,MentorDAO mentorDAO,CouponDAO couponDAO, UserCourseDAO userCourseDAO) 
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

    }

}

