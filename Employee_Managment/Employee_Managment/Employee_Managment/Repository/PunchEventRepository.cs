using Employee_Managment.Context;
using Employee_Managment.Models;
using Microsoft.EntityFrameworkCore;

namespace Employee_Managment.Repository
{
    public class PunchEventRepository : IPunchEventRepository
    {
        private readonly ApplicationDbContext _context;

        public PunchEventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PunchEvent>> GetPunchEventAsync()
        {
            return await _context.PunchEvents.ToListAsync();
        }

        public async Task CreatePunchEventAsync(PunchEvent punchEvent)
        {
            _context.PunchEvents.Add(punchEvent);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<PunchEvent>> GetPunchEventsForMonthAsync(int month, int year)
        {
            return await _context.PunchEvents
                .Where(p => p.PunchIn.Month == month && p.PunchIn.Year == year)
                .Include(p => p.Employee) // Include the Employee
                .OrderByDescending(p => p.PunchIn) // Order by PunchIn property in descending order
                .ToListAsync();
        }

        public async Task<DateTime?> GetPunchInTimeForEmployeeAsync(int employeeId, DateTime date)
        {
            // Assuming you have a Punch model with properties like EmployeeId and PunchIn
            var punchEntry = await _context.PunchEvents
                .OrderByDescending(p => p.PunchIn)
                .FirstOrDefaultAsync(p => p.EmployeeId == employeeId && p.PunchIn.Date == date.Date);

            if (punchEntry != null)
            {
                return punchEntry.PunchIn;
            }

            // Return null or a default value if no punch entry is found for the employee on the given date.
            return null;
        }

        public async Task<bool> CheckPunchExistsForEmployeeAsync(int employeeId, DateTime date)
        {
          
            var punchExists = await _context.PunchEvents
                .AnyAsync(p => p.EmployeeId == employeeId && p.PunchIn.Date == date.Date);

            return punchExists;
        }

        public async Task UpdatePunchEventAsync(PunchEvent punchEvent)
        {
            _context.Entry(punchEvent).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<PunchEvent>> GetPunchEventsForEmployeeAndDateAsync(int employeeId, DateTime date)
        {
            return await _context.PunchEvents
                .Where(p => p.EmployeeId == employeeId && p.PunchIn.Date == date.Date)
                .ToListAsync();
        }
        public async Task<PunchEvent> GetPunchEventById(int id)
        {
            // Implementation for GetPunchEventById
            var punchEvent = await _context.PunchEvents.FindAsync(id);
            return punchEvent;
        }

    }
}
