using Employee_Managment.Models;

namespace Employee_Managment.Repository
{
    public interface IPunchEventRepository
    {
        Task<PunchEvent> GetPunchEventById(int id);
        Task<IEnumerable<PunchEvent>> GetPunchEventAsync();
        Task CreatePunchEventAsync(PunchEvent punchEvent);
        Task<IEnumerable<PunchEvent>> GetPunchEventsForMonthAsync(int month, int year);
        Task<bool> CheckPunchExistsForEmployeeAsync(int employeeId, DateTime date);
        Task<DateTime?> GetPunchInTimeForEmployeeAsync(int employeeId, DateTime date);
        Task<IEnumerable<PunchEvent>> GetPunchEventsForEmployeeAndDateAsync(int employeeId, DateTime date);
        Task UpdatePunchEventAsync(PunchEvent punchEvent);
    }
}
