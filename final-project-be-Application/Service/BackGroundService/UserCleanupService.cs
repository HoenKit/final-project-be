using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Service.BackGroundService
{
    public class UserCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UserCleanupService> _logger;

        public UserCleanupService(IServiceProvider serviceProvider, ILogger<UserCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        var twoMonthAgo = DateTime.Now.AddMonths(-2);

                        // Lấy danh sách user cần xóa
                        var usersToDelete = await dbContext.users
                            .Include(u => u.UserRoles)
                            .Include(u => u.UserMetaData)
                            .Where(u => !u.IsConfirmed && u.CreateAt < twoMonthAgo)
                            .ToListAsync(stoppingToken);

                        if (usersToDelete.Any())
                        {
                            foreach (var user in usersToDelete)
                            {
                                // Xóa UserRoles
                                if (user.UserRoles != null)
                                    dbContext.userRoles.RemoveRange(user.UserRoles);

                                // Xóa UserMetadata
                                if (user.UserMetaData != null)
                                    dbContext.UserMetadata.Remove(user.UserMetaData);

                                // Xóa User
                                dbContext.users.Remove(user);
                            }

                            await dbContext.SaveChangesAsync(stoppingToken);
                            _logger.LogInformation($"{usersToDelete.Count} users have been cleaned up.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while cleaning up unconfirmed users.");
                }

                // chạy lại sau mỗi 24h
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
