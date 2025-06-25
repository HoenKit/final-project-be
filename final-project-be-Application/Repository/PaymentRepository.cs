 using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Mentor;
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
        private readonly CourseDAO _courseDAO;
        private readonly MentorDAO _mentorDAO;
        private readonly PaymentDAO _paymentDAO;
        private readonly PaymentCourseDAO _paymentCourseDAO;
        private readonly ILogger<PaymentRepository> _logger;
        public PaymentRepository(UserDAO userDAO, CourseDAO courseDAO, PaymentDAO paymentDAO, PaymentCourseDAO paymentCourseDAO, ILogger<PaymentRepository> logger,MentorDAO mentorDAO,CouponDAO couponDAO) 
        {
            _mentorDAO = mentorDAO;
            _userDAO = userDAO;
            _courseDAO = courseDAO;
            _paymentDAO = paymentDAO;
            _paymentCourseDAO = paymentCourseDAO;
            _couponDAO = couponDAO;
            _logger = logger;
        }

        public async Task<bool> BuyCourseAsync(Guid userId, int courseId, int couponId)
        {
            try
            {
                await _paymentDAO.BeginTransactionAsync();

                var user = await _userDAO.GetByIdAsync(userId);
                var course = await _courseDAO.GetByIdAsync(courseId);

                if (user == null || course == null)
                    return false;

                decimal originalCost = course.Cost;
                decimal finalCost = originalCost;

                // Áp dụng coupon (giảm theo %)
                if (couponId > 0)
                {
                    var coupon = await _couponDAO.GetByIdAsync(couponId);
                    if (coupon != null && coupon.Discount > 0)
                    {
                        finalCost = originalCost * (1 - (decimal)coupon.Discount / 100m);
                        if (finalCost < 0) finalCost = 0;
                    }
                }

                // Kiểm tra user có đủ điểm không
                if (user.Point < finalCost)
                    return false;

                // Lấy mentor
                var mentor = await _mentorDAO.GetMentorinCourseAsync(course.MentorId);
                if (mentor == null)
                    return false;

                // Tạo payment
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

                // Ghi nhận mua khóa học
                var paymentCourse = new PaymentCourse
                {
                    PaymentId = payment.PaymentId,
                    CourseId = courseId,
                    CouponId = couponId > 0 ? couponId : null,
                    CreatedAt = DateTime.UtcNow
                };
                await _paymentCourseDAO.AddAsync(paymentCourse);

                // Trừ điểm người dùng
                user.Point -= finalCost;
                await _userDAO.UpdateAsync(user);

                // Cộng điểm cho mentor: 85% của finalCost
                mentor.User.Point += finalCost * 0.85m;
                await _mentorDAO.UpdateAsync(mentor);

                await _paymentDAO.SaveChangesAsync();
                await _paymentDAO.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _paymentDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when processing course purchase");
                return false;
            }
        }

    }
}
