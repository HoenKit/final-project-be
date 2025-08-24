using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Coupon;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace final_project_be_Tests
{
    public class CouponRepositoryTests
    {
        private readonly Mock<ICouponDAO> _couponDaoMock;
        private readonly Mock<ICourseCouponDAO> _courseCouponDaoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<CouponRepository>> _loggerMock;
        private readonly CouponRepository _repository;

        public CouponRepositoryTests()
        {
            _couponDaoMock = new Mock<ICouponDAO>();
            _courseCouponDaoMock = new Mock<ICourseCouponDAO>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CouponRepository>>();
            _repository = new CouponRepository(
                _couponDaoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _courseCouponDaoMock.Object
            );
        }

        [Fact]
        public async Task GetAllCouponsAsync_ShouldReturnCoupons()
        {
            var coupons = new List<CouponDto>
            {
                new CouponDto { CouponId = 1, CouponName = "A", Discount = 10 },
                new CouponDto { CouponId = 2, CouponName = "B", Discount = 20 }
            };
            _couponDaoMock.Setup(d => d.GetAllCouponsAsync()).ReturnsAsync(coupons);

            var result = await _repository.GetAllCouponsAsync();

            Assert.Equal(2, result.Count);
            Assert.Equal("A", result[0].CouponName);
        }

        [Fact]
        public async Task GetCouponsByCourseIdAsync_ShouldReturnCoupons()
        {
            var coupons = new List<CouponDto>
            {
                new CouponDto { CouponId = 1, CouponName = "A", Discount = 10 }
            };
            _couponDaoMock.Setup(d => d.GetCouponsByCourseIdAsync(1)).ReturnsAsync(coupons);

            var result = await _repository.GetCouponsByCourseIdAsync(1);

            Assert.Single(result);
            Assert.Equal("A", result[0].CouponName);
        }

        [Fact]
        public async Task CreateCourseCouponAsync_ShouldReturnTrue_WhenSuccess()
        {
            var courseCoupon = new CourseCoupon { CourseId = 1, CouponId = 2 };
            var oldCoupons = new List<CourseCoupon>
            {
                new CourseCoupon { CourseId = 1, CouponId = 2 }
            };
            _courseCouponDaoMock.Setup(d => d.GetCourseCoupons(1, 2)).Returns(oldCoupons.AsQueryable());
            _courseCouponDaoMock.Setup(d => d.RemoveCourseCouponsAsync(It.IsAny<IEnumerable<CourseCoupon>>())).Returns(Task.CompletedTask);
            _courseCouponDaoMock.Setup(d => d.AddCourseCouponAsync(courseCoupon)).Returns(Task.CompletedTask);
            _courseCouponDaoMock.Setup(d => d.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _repository.CreateCourseCouponAsync(courseCoupon);

            Assert.True(result);
            _courseCouponDaoMock.Verify(d => d.RemoveCourseCouponsAsync(It.IsAny<IEnumerable<CourseCoupon>>()), Times.Once);
            _courseCouponDaoMock.Verify(d => d.AddCourseCouponAsync(courseCoupon), Times.Once);
            _courseCouponDaoMock.Verify(d => d.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateCourseCouponAsync_ShouldReturnFalse_WhenExceptionThrown()
        {
            var courseCoupon = new CourseCoupon { CourseId = 1, CouponId = 2 };
            _courseCouponDaoMock.Setup(d => d.GetCourseCoupons(1, 2)).Throws(new Exception("DB error"));

            var result = await _repository.CreateCourseCouponAsync(courseCoupon);

            Assert.False(result);
        }
    }
}