using final_project_be_Application.Interface;
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
        private readonly UserDAO _userDAO;
        private readonly CourseDAO _courseDAO;
        private readonly PaymentDAO _paymentDAO;
        private readonly PaymentCourseDAO _paymentCourseDAO;
        private readonly ILogger<PaymentRepository> _logger;
        public PaymentRepository(UserDAO userDAO, CourseDAO courseDAO, PaymentDAO paymentDAO, PaymentCourseDAO paymentCourseDAO, ILogger<PaymentRepository> logger) 
        {
            _userDAO = userDAO;
            _courseDAO = courseDAO;
            _paymentDAO = paymentDAO;
            _paymentCourseDAO = paymentCourseDAO;
            _logger = logger;
        }

        public async Task<bool> BuyCourseAsync(Guid userId, int courseId)
        {
            try
            {
                await _paymentDAO.BeginTransactionAsync();

                var user = await _userDAO.GetByIdAsync(userId);
                var course = await _courseDAO.GetByIdAsync(courseId);

                if (user == null || course == null || user.Point < course.Cost)
                    return false;

                var payment = new Payment
                {
                    UserId = userId,
                    Amount = 1,
                    Status = "Success",
                    ServiceType = "Course",
                    CreatedAt = DateTime.UtcNow
                };
                await _paymentDAO.AddAsync(payment);
                await _paymentDAO.SaveChangesAsync();

                var paymentCourse = new PaymentCourse
                {
                    PaymentId = payment.PaymentId,
                    CourseId = courseId,
                    CreatedAt = DateTime.UtcNow
                };
                await _paymentCourseDAO.AddAsync(paymentCourse);

                user.Point -= course.Cost;
                await _userDAO.UpdateAsync(user);
                await _paymentDAO.SaveChangesAsync();
                await _paymentDAO.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _paymentDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when adding PostFile");
                return false;
            }
        }
    }
}
