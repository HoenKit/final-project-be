using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Service.BackGroundService
{
    public class PremiumExpirationService : BackgroundService
    {

        private readonly IServiceScopeFactory _scopeFactory;
        public PremiumExpirationService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // Lấy tất cả user đang là Premium
                    var premiumUsers = await dbContext.users
                        .Where(u => u.IsPremium)
                        .ToListAsync(stoppingToken);

                    foreach (var user in premiumUsers)
                    {
                        // Lấy PaymentPlan mới nhất của user
                        var latestPlan = await dbContext.PaymentPlans
                            .Include(pp => pp.Payment)
                            .Where(pp => pp.Payment.UserId == user.UserId)
                            .OrderByDescending(pp => pp.ExpiredAt)
                            .FirstOrDefaultAsync(stoppingToken);

                        // Nếu không còn plan nào hoặc hết hạn -> hạ Premium
                        if (latestPlan == null || latestPlan.ExpiredAt <= DateTime.UtcNow)
                        {
                            user.IsPremium = false;
                        }
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in PremiumExpirationService: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}

