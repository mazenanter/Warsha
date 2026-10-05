using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Jobs
{
    public class ExpirePointsJob : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<ExpirePointsJob> _logger;

        public ExpirePointsJob(IServiceProvider services, ILogger<ExpirePointsJob> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var next2Am = now.Date.AddDays(1).AddHours(2);
                await Task.Delay(next2Am - now, ct);

                await RunExpirationAsync(ct);
            }
        }

        private async Task RunExpirationAsync(CancellationToken ct)
        {
            using var scope = _services.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            try
            {
                var config = await unitOfWork.LoyaltyPointRules
                    .FindAsync(_ => true);

                var expirationConfig = await unitOfWork.LoyaltyExpirationConfigs
                    .GetCurrentAsync(ct);

                if (expirationConfig is null || !expirationConfig.IsActive) return;

                var clientsToExpire = await unitOfWork.LoyaltyTransactions
                    .GetClientsForExpirationAsync(expirationConfig.InactivityMonths, ct);

                foreach (var (clientId, pendingPts, availablePts) in clientsToExpire)
                {
                    var wallet = await unitOfWork.LoyaltyWallets
                        .GetByClientIdAsync(clientId, ct);

                    if (wallet is null) continue;

                    var totalToExpire = availablePts + pendingPts;
                    if (totalToExpire <= 0) continue;

                    wallet.ExpirePoints(totalToExpire);

                    var transaction = LoyaltyTransaction.CreateExpiration(
                        clientId, totalToExpire, 0,
                        $"Points expired due to {expirationConfig.InactivityMonths} months of inactivity");

                    await unitOfWork.LoyaltyTransactions.AddAsync(transaction, ct);

                    _logger.LogInformation(
                        "Expired {Points} WP for client {ClientId}", totalToExpire, clientId);
                }

                await unitOfWork.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during points expiration job");
            }
        }
    }
}
