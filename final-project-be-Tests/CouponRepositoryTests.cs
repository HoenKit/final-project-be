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
        public async Task AddCourseCouponsAsync_ShouldAddNewCoupon_WhenNotExist()
        {
            // Arrange
            var dto = new AddCouponDto
            {
                CouponId = 2,
                ExpiredAt = DateTime.UtcNow.AddDays(10),
                CourseIds = new List<int> { 1 }
            };

            _courseCouponDaoMock
                .Setup(d => d.GetCourseCoupons(1, 2))
                .Returns(new List<CourseCoupon>().AsQueryable()); // không có coupon cũ

            _courseCouponDaoMock
                .Setup(d => d.AddCourseCouponAsync(It.IsAny<CourseCoupon>()))
                .Returns(Task.CompletedTask);

            _courseCouponDaoMock
                .Setup(d => d.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _repository.AddCourseCouponsAsync(dto);

            // Assert
            _courseCouponDaoMock.Verify(d => d.AddCourseCouponAsync(It.Is<CourseCoupon>(c => c.CourseId == 1 && c.CouponId == 2)), Times.Once);
            _courseCouponDaoMock.Verify(d => d.UpdateAsync(It.IsAny<CourseCoupon>()), Times.Never);
            _courseCouponDaoMock.Verify(d => d.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddCourseCouponsAsync_ShouldThrowException_WhenDaoFails()
        {
            // Arrange
            var dto = new AddCouponDto
            {
                CouponId = 2,
                ExpiredAt = DateTime.UtcNow.AddDays(5),
                CourseIds = new List<int> { 1 }
            };

            _courseCouponDaoMock
                .Setup(d => d.GetCourseCoupons(1, 2))
                .Throws(new Exception("DB error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _repository.AddCourseCouponsAsync(dto));
            Assert.Equal("DB error", ex.Message);
        }
    }
}