using Employee_Managment.DTO;
using Employee_Managment.Models;
using Employee_Managment.Repository;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;


namespace Employee_Managment.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _employeeRepository.GetAllEmployeesAsync();
            return View(employees);
        }


        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetEmployeeByIdAsync(id.Value);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, BasicAndDA, PerDay, TotalDays, PresentDays, AbsentDays, SundayHoliday, CL, TotalPayableDays, PayableAmount, ProfessionalTax, PF, ESIC, ExtraDeduction, NetAmountPayable")] Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    employee.CreationDate = DateTime.Now; // Set the creation date
                    await _employeeRepository.CreateEmployeeAsync(employee);
                    TempData["Message"] = "Employee created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while creating the employee: " + ex.Message;
                return View(employee);
            }
        }


        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetEmployeeByIdAsync(id.Value);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Name, BasicAndDA, PerDay, TotalDays, PresentDays, AbsentDays, SundayHoliday, CL, TotalPayableDays, PayableAmount, ProfessionalTax, PF, ESIC, ExtraDeduction, NetAmountPayable, CreationDate")] Employee employee)
        {
            try
            {
                if (id != employee.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var existingEmployee = await _employeeRepository.GetEmployeeByIdAsync(id);

                    if (existingEmployee != null)
                    {
                        // Preserve the existing CreationDate when updating
                        employee.CreationDate = existingEmployee.CreationDate;

                        await _employeeRepository.UpdateEmployeeAsync(employee);
                        TempData["Message"] = "Employee updated successfully!";
                    }

                    return RedirectToAction(nameof(Index));
                }

                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the employee: " + ex.Message;
                return View(employee);
            }
        }

        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetEmployeeByIdAsync(id.Value);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _employeeRepository.DeleteEmployeeAsync(id);
                TempData["Message"] = "Employee deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the employee: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        private bool EmployeeExists(int id)
        {
            // Check if an employee with the given ID exists
            var employee = _employeeRepository.GetEmployeeByIdAsync(id).Result;
            return employee != null;
        }
        public async Task<IActionResult> ExportToExcel()
        {
            var employees = await _employeeRepository.GetAllEmployeesAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employees");

                // Header row
                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Basic + DA";
                worksheet.Cells[1, 3].Value = "Total Days";
                worksheet.Cells[1, 4].Value = "Present Days";
                worksheet.Cells[1, 5].Value = "Absent Days";
                worksheet.Cells[1, 6].Value = "Sunday Holiday";
                worksheet.Cells[1, 7].Value = "CL+";
                worksheet.Cells[1, 8].Value = "Total Payable Days";
                worksheet.Cells[1, 9].Value = "Payable Amount";
                worksheet.Cells[1, 10].Value = "Professional Tax";
                worksheet.Cells[1, 11].Value = "P.F@12 %";
                worksheet.Cells[1, 12].Value = "ESIC@0.75 %";
                worksheet.Cells[1, 13].Value = "Extra Deduction/Arrears";
                worksheet.Cells[1, 14].Value = "Net Amount Payable";

                // Data rows
                for (int i = 0; i < employees.Count(); i++)
                {
                    var employee = employees.ElementAt(i);
                    worksheet.Cells[i + 2, 1].Value = employee.Name;
                    worksheet.Cells[i + 2, 2].Value = employee.BasicAndDA;
                    worksheet.Cells[i + 2, 3].Value = employee.TotalDays;
                    worksheet.Cells[i + 2, 4].Value = employee.PresentDays;
                    worksheet.Cells[i + 2, 5].Value = employee.AbsentDays;
                    worksheet.Cells[i + 2, 6].Value = employee.SundayHoliday;
                    worksheet.Cells[i + 2, 7].Value = employee.CL;
                    worksheet.Cells[i + 2, 8].Value = employee.TotalPayableDays;
                    worksheet.Cells[i + 2, 9].Value = employee.PayableAmount;
                    worksheet.Cells[i + 2, 10].Value = employee.ProfessionalTax;
                    worksheet.Cells[i + 2, 11].Value = employee.PF;
                    worksheet.Cells[i + 2, 12].Value = employee.ESIC;
                    worksheet.Cells[i + 2, 13].Value = employee.ExtraDeduction;
                    worksheet.Cells[i + 2, 14].Value = employee.NetAmountPayable;
                }

                // AutoFit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set response headers for downloading
                Response.Headers.Add("Content-Disposition", "attachment; filename=EmployeeList.xlsx");
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

    }
}