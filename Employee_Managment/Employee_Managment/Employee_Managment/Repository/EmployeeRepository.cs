using Employee_Managment.Context;
using Employee_Managment.Models;
using Microsoft.EntityFrameworkCore;

namespace Employee_Managment.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees.Where(e => !e.IsDeleted).ToListAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task CreateEmployeeAsync(Employee employee)
        {
            employee.CreationDate = DateTime.Now; // Set the creation date
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            var existingEmployee = await _context.Employees.FindAsync(employee.Id);

            if (existingEmployee != null)
            {
                // Preserve the existing CreationDate when updating
                employee.CreationDate = existingEmployee.CreationDate;

                _context.Entry(existingEmployee).CurrentValues.SetValues(employee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee != null)
            {
                employee.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Employee>> GetEmployeesForDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Employees
                .Where(e => !e.IsDeleted && e.CreationDate >= startDate && e.CreationDate <= endDate)
                .ToListAsync();
        }
        public async Task UpdateEmployeeCLAsync(Employee employee)
        {
            // Assuming you have a database context or data access layer to update the employee's CL
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAbsentDaysAsync(Employee employee)
        {
            // Assuming you have a database context or data access layer to update the absent days count
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePresentDaysAsync(Employee employee)
        {
            // Assuming Employee entity has a property named PresentDays
            _context.Employees.Attach(employee);
            _context.Entry(employee).Property(e => e.PresentDays).IsModified = true;
            await _context.SaveChangesAsync();
        }
        public async Task ResetPresentDaysAsync()
        {
            var employees = await _context.Employees.ToListAsync();

            foreach (var employee in employees)
            {
                // Reset PresentDays to zero
                employee.PresentDays = 0;

                // Update the employee in the database
                _context.Employees.Attach(employee);
                _context.Entry(employee).Property(e => e.PresentDays).IsModified = true;
            }

            // Save changes to the database
            await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
