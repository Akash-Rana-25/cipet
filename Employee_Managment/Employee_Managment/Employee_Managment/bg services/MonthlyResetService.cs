using Employee_Managment.Repository;

namespace Employee_Managment.bg_services
{
    public class MonthlyResetService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public MonthlyResetService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Check if it's the start of a new month
                if (DateTime.Now.Day == 1)
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var employeeRepository = scope.ServiceProvider.GetRequiredService<IEmployeeRepository>();
                        await employeeRepository.ResetPresentDaysAsync();
                    }
                }

                // Wait for the next day to check again
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }


}
