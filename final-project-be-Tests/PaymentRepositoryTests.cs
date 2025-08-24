using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Payment;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace final_project_be_Tests
{
    public class PaymentRepositoryTests
    {
        private readonly Mock<IUserDAO> _userDaoMock = new();
        private readonly Mock<ICourseDAO> _courseDaoMock = new();
        private readonly Mock<IPaymentDAO> _paymentDaoMock = new();
        private readonly Mock<IPaymentCourseDAO> _paymentCourseDaoMock = new();
        private readonly Mock<IMentorDAO> _mentorDaoMock = new();
        private readonly Mock<ICouponDAO> _couponDaoMock = new();
        private readonly Mock<IUserCourseDAO> _userCourseDaoMock = new();
        private readonly Mock<ILogger<PaymentRepository>> _loggerMock = new();
        private readonly PaymentRepository _repository;
        private readonly Mock<IPaymentPlanDAO> _paymentPlanDaoMock = new();
        private readonly Mock<IMembershipPlanDAO> _membershipPlanDaoMock = new();

        public PaymentRepositoryTests()
        {
            _repository = new PaymentRepository(
                _userDaoMock.Object,
                _courseDaoMock.Object,
                _paymentDaoMock.Object,
                _paymentPlanDaoMock.Object,
                _membershipPlanDaoMock.Object,
                _paymentCourseDaoMock.Object,
                _loggerMock.Object,
                _mentorDaoMock.Object,
                _couponDaoMock.Object,
                _userCourseDaoMock.Object
            );
        }

        [Fact]
        public async Task BuyCourseAsync_ShouldReturnNotFound_WhenUserOrCourseNull()
        {
            var userId = Guid.NewGuid();
            var courseId = 1;
            _userDaoMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync((User)null);
            _courseDaoMock.Setup(d => d.GetByIdAsync(courseId)).ReturnsAsync((Courses)null);
            _paymentDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _paymentDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.BuyCourseAsync(userId, courseId, 0);

            Assert.False(result.Success);
            Assert.Equal("NotFound", result.Error);
            _paymentDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task BuyCourseAsync_ShouldReturnPreviouslyPurchased_WhenAlreadyCompleted()
        {
            var userId = Guid.NewGuid();
            var courseId = 1;
            var user = new User { Point = 100 };
            var course = new Courses { Cost = 50, Mentor = new Mentor { User = new User() } };
            var userCourse = new UserCourse { Status = "Completed" };
            _userDaoMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync(user);
            _courseDaoMock.Setup(d => d.GetByIdAsync(courseId)).ReturnsAsync(course);
            _userCourseDaoMock.Setup(d => d.GetUserCourse(userId, courseId)).ReturnsAsync(userCourse);
            _paymentDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _paymentDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.BuyCourseAsync(userId, courseId, 0);

            Assert.False(result.Success);
            Assert.Equal("PreviouslyPurchased", result.Error);
            _paymentDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task BuyCourseAsync_ShouldReturnNotEnoughPoint_WhenUserPointLow()
        {
            var userId = Guid.NewGuid();
            var courseId = 1;
            var user = new User { Point = 10 };
            var course = new Courses { Cost = 100, MentorId = 1, Mentor = new Mentor { User = new User() } };
            _userDaoMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync(user);
            _courseDaoMock.Setup(d => d.GetByIdAsync(courseId)).ReturnsAsync(course);
            _userCourseDaoMock.Setup(d => d.GetUserCourse(userId, courseId)).ReturnsAsync((UserCourse)null);
            _paymentDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _paymentDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.BuyCourseAsync(userId, courseId, 0);

            Assert.False(result.Success);
            Assert.Equal("NotEnoughPoint", result.Error);
            _paymentDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task BuyCourseAsync_ShouldReturnMentorNotFound_WhenMentorNull()
        {
            var userId = Guid.NewGuid();
            var courseId = 1;
            var user = new User { Point = 1000 };
            var course = new Courses { Cost = 100, MentorId = 1 };
            _userDaoMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync(user);
            _courseDaoMock.Setup(d => d.GetByIdAsync(courseId)).ReturnsAsync(course);
            _userCourseDaoMock.Setup(d => d.GetUserCourse(userId, courseId)).ReturnsAsync((UserCourse)null);
            _couponDaoMock.Setup(d => d.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Coupon)null);
            _mentorDaoMock.Setup(d => d.GetMentorinCourseAsync(course.MentorId)).ReturnsAsync((Mentor)null);
            _paymentDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _paymentDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.BuyCourseAsync(userId, courseId, 0);

            Assert.False(result.Success);
            Assert.Equal("MentorNotFound", result.Error);
            _paymentDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task BuyCourseAsync_ShouldReturnSuccess_WhenAllValid()
        {
            var userId = Guid.NewGuid();
            var courseId = 1;
            var mentorUser = new User { Point = 0 };
            var mentor = new Mentor { User = mentorUser };
            var user = new User { Point = 1000 };
            var course = new Courses { Cost = 100, MentorId = 1, Mentor = mentor, StudentCount = 0 };
            _userDaoMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync(user);
            _courseDaoMock.Setup(d => d.GetByIdAsync(courseId)).ReturnsAsync(course);
            _userCourseDaoMock.Setup(d => d.GetUserCourse(userId, courseId)).ReturnsAsync((UserCourse)null);
            _couponDaoMock.Setup(d => d.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Coupon)null);
            _mentorDaoMock.Setup(d => d.GetMentorinCourseAsync(course.MentorId)).ReturnsAsync(mentor);
            _paymentDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _paymentDaoMock.Setup(d => d.AddAsync(It.IsAny<Payment>())).Returns(Task.CompletedTask);
            _paymentDaoMock.Setup(d => d.SaveChangesAsync()).Returns(Task.CompletedTask);
            _paymentCourseDaoMock.Setup(d => d.AddAsync(It.IsAny<PaymentCourse>())).Returns(Task.CompletedTask);
            _userCourseDaoMock.Setup(d => d.AddUserCourseAsync(It.IsAny<UserCourse>())).Returns(Task.CompletedTask);
            _courseDaoMock.Setup(d => d.UpdateAsync(course)).Returns(Task.CompletedTask);
            _userDaoMock.Setup(d => d.UpdateAsync(user)).Returns(Task.CompletedTask);
            _mentorDaoMock.Setup(d => d.UpdateAsync(mentor)).Returns(Task.CompletedTask);
            _paymentDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.BuyCourseAsync(userId, courseId, 0);

            Assert.True(result.Success);
            _paymentDaoMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task BuyCourseAsync_ShouldReturnException_WhenExceptionThrown()
        {
            var userId = Guid.NewGuid();
            var courseId = 1;
            _paymentDaoMock.Setup(d => d.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _paymentDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.BuyCourseAsync(userId, courseId, 0);

            Assert.False(result.Success);
            Assert.Equal("Exception", result.Error);
            _paymentDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public void GetAll_ShouldReturnPagedResult()
        {
            var userId = Guid.NewGuid();
            var payments = new List<Payment>
    {
        new Payment
        {
            PaymentId = 1,
            UserId = userId,
            Amount = 100,
            Status = "Success",
            ServiceType = "Course",
            CreatedAt = DateTime.UtcNow,
            User = new User { Email = "test@example.com" },
            PaymentCourses = new List<PaymentCourse>
            {
                new PaymentCourse { CourseId = 2, Courses = new Courses { CourseName = "C#" } }
            },
            PaymentPlans = new List<PaymentPlan>
            {
                new PaymentPlan { MembershipPlan = new MembershipPlan { Name = "Gold" } }
            }
        }
    }.AsQueryable();

            var paymentDaoMock = new Mock<IPaymentDAO>();
            paymentDaoMock.Setup(d => d.GetAll()).Returns(payments); // AsQueryable in-memory

            var repository = new PaymentRepository(
                _userDaoMock.Object,
                _courseDaoMock.Object,
                paymentDaoMock.Object, // dùng mock này
                _paymentPlanDaoMock.Object,
                _membershipPlanDaoMock.Object,
                _paymentCourseDaoMock.Object,
                _loggerMock.Object,
                _mentorDaoMock.Object,
                _couponDaoMock.Object,
                _userCourseDaoMock.Object
            );

            var result = repository.GetAll(1, 10, userId, "desc_date", new List<ServiceTypeEnum> { ServiceTypeEnum.Course });

            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Items);
            Assert.Equal("C#", result.Items.First().CourseName);
        }


        [Fact]
        public void GetAll_ShouldReturnEmptyResult_WhenExceptionThrown()
        {
            var paymentDaoMock = new Mock<IPaymentDAO>();
            paymentDaoMock.Setup(d => d.GetAll()).Throws(new Exception("DB error"));

            var repository = new PaymentRepository(
                _userDaoMock.Object,
                _courseDaoMock.Object,
                _paymentDaoMock.Object,
                _paymentPlanDaoMock.Object,
                _membershipPlanDaoMock.Object,
                _paymentCourseDaoMock.Object,
                _loggerMock.Object,
                _mentorDaoMock.Object,
                _couponDaoMock.Object,
                _userCourseDaoMock.Object
            );

            var result = repository.GetAll(1, 10, null, null, null);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task GetStatisticsByMonth_ShouldReturnStats_WhenSuccess()
        {
            var payments = new List<Payment>
        {
            new Payment
            {
                PaymentId = 1,
                Amount = 100,
                Status = "Success",
                ServiceType = "Premium",
                CreatedAt = new DateTime(2024, 1, 15),
                PaymentCourses = new List<PaymentCourse>
                {
                    new PaymentCourse { Courses = new Courses { IsDeleted = false, Modules = new List<Module>() } }
                }
            },
            new Payment
            {
                PaymentId = 2,
                Amount = 200,
                Status = "Success",
                ServiceType = "Course",
                CreatedAt = new DateTime(2024, 1, 20),
                PaymentCourses = new List<PaymentCourse>
                {
                    new PaymentCourse { Courses = new Courses { IsDeleted = false, Modules = new List<Module> { new Module { IsPremium = true } } } }
                }
            }
        }.AsQueryable();

            var paymentDaoMock = new Mock<IPaymentDAO>();
            paymentDaoMock.Setup(d => d.GetAll()).Returns(payments);

            var repository = new PaymentRepository(
                _userDaoMock.Object,
                _courseDaoMock.Object,
                paymentDaoMock.Object,
                _paymentPlanDaoMock.Object,
                _membershipPlanDaoMock.Object,
                _paymentCourseDaoMock.Object,
                _loggerMock.Object,
                _mentorDaoMock.Object,
                _couponDaoMock.Object,
                _userCourseDaoMock.Object
            );

            var result = await repository.GetStatisticsByMonth(2024);

            Assert.Single(result);
            Assert.Equal("01/2024", result[0].Time);
            Assert.Equal(1, result[0].TotalPremium);
            Assert.True(result[0].TotalPoint > 0);
        }
    }
}