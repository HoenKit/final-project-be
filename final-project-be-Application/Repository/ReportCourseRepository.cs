using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Report;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using final_project_be_Infrastructure.DAO;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Application.Repository
{
    public class ReportCourseRepository : Repository<ReportCourse>, IReportCourseRepository
    {
        private readonly IReportCourseDAO _ReportCourseDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<ReportCourseRepository> _logger;
        private readonly IReportDAO _reportDAO;
        public ReportCourseRepository(IReportCourseDAO ReportCourseDAO, IMapper mapper, ILogger<ReportCourseRepository> logger, IReportDAO reportDAO) : base(ReportCourseDAO)
        {
            _ReportCourseDAO = ReportCourseDAO;
            _mapper = mapper;
            _logger = logger;
            _reportDAO = reportDAO;
        }

        public async Task<ReportCourse> CreateReportCourse(ReportCourseDto dto)
        {
            try
            {
                await _ReportCourseDAO.BeginTransactionAsync();

                // Tạo Report trước
                var report = _mapper.Map<Report>(dto);
                await _reportDAO.AddAsync(report);

                if (report == null || report.ReportId <= 0)
                {
                    _logger.LogError("Failed to create Report, cannot proceed with ReportCourse.");
                    await _ReportCourseDAO.RollbackTransactionAsync();
                    return null;
                }

                // Gán ReportId vừa tạo vào dto để map sang ReportCourse
                dto.ReportId = report.ReportId;

                var reportCourse = _mapper.Map<ReportCourse>(dto);
                await _ReportCourseDAO.AddAsync(reportCourse);

                await _ReportCourseDAO.CommitTransactionAsync();

                _logger.LogInformation("Successfully added ReportCourse with ID: {Id}", reportCourse.ReportId);
                return reportCourse;
            }
            catch (Exception ex)
            {
                await _ReportCourseDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when adding ReportCourse");
                return null;
            }
        }

        public PageResult<ReportCourse> GetAllReportCourses(int page, int pageSize)
        {
            try
            {
                var totalCount = _ReportCourseDAO.GetAll().Count();
                var ReportCourses = _ReportCourseDAO.GetAll()
                    .Include(x => x.Report)
                    .Include(c => c.Course)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Get ReportCourses success");

                return new PageResult<ReportCourse>(ReportCourses, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting ReportCourses");
                return new PageResult<ReportCourse>(new List<ReportCourse>(), 0, page, pageSize);
            }
        }

        public async Task<ReportCourse> GetReportCourse(int id)
        {
            try
            {
                await _ReportCourseDAO.BeginTransactionAsync();
                var ReportCourse = _ReportCourseDAO.GetByReportId(id);
                await _ReportCourseDAO.CommitTransactionAsync();

                _logger.LogInformation("Get ReportCourse success");
                return ReportCourse;
            }
            catch (Exception ex)
            {
                await _ReportCourseDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when get ReportCourse");
                return null;
            }
        }
        public PageResult<GroupedReportDto<int, ReportCourse>> GetGroupedReportCourses(int page, int pageSize)
        {
            var allReportCourses = _ReportCourseDAO.GetAll()
                .Include(rp => rp.Report)
                .Include(rp => rp.Course)
                .AsEnumerable();

            var grouped = allReportCourses
                .GroupBy(rp => rp.CourseId)
                .Select(group => new GroupedReportDto<int, ReportCourse>
                {
                    Id = group.Key,
                    ReportCount = group.Count(),
                    Reports = group.ToList()
                })
                .OrderByDescending(g => g.ReportCount)
                .ToList();

            var totalCount = grouped.Count;
            var paged = grouped.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PageResult<GroupedReportDto<int, ReportCourse>>(paged, totalCount, page, pageSize);
        }

        public async Task<bool> DeleteReportsByCourseId(int courseId)
        {
            try
            {
                await _reportDAO.BeginTransactionAsync();

                var reportCourses = _ReportCourseDAO.GetByCoursedId(courseId);

                if (reportCourses == null || !reportCourses.Any())
                {
                    _logger.LogWarning($"No reports found for courseId: {courseId}");
                    await _reportDAO.CommitTransactionAsync(); // vẫn commit để tránh giữ transaction
                    return false;
                }

                // 2. Lấy danh sách reportId
                var reportIds = reportCourses.Select(rp => rp.ReportId).Distinct().ToList();

                // 3. Xóa các bản ghi ReportCourse
                foreach (var rp in reportCourses)
                {
                    _ReportCourseDAO.DeleteByReportAndCourseId(rp.ReportId, rp.CourseId);
                }

                // 4. Xóa các bản ghi Report
                foreach (var reportId in reportIds)
                {
                    await _reportDAO.DeleteAsync(reportId);
                }

                await _reportDAO.CommitTransactionAsync();

                _logger.LogInformation($"Successfully deleted all reports for CourseId: {courseId}");
                return true;
            }
            catch (Exception ex)
            {
                await _reportDAO.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error deleting reports for courseId: {courseId}");
                return false;
            }
        }

    }
}
