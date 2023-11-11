using Microsoft.AspNetCore.Mvc;
using Employee_Managment.Models;
using Employee_Managment.Repository;
using Employee_Managment.DTO;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;

namespace Employee_Managment.Controllers
{
    public class PunchEventsController : Controller
    {
        private readonly IPunchEventRepository _punchEventRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public PunchEventsController(IPunchEventRepository punchEventRepository, IEmployeeRepository employeeRepository)
        {
            _punchEventRepository = punchEventRepository;
            _employeeRepository = employeeRepository;
        }
        public async Task<IActionResult> CurrentMonthPunches(string selectedDate)
        {
            IEnumerable<ViewPunchEvents> viewPunchEvents;

            DateTime filterDate;

            if (DateTime.TryParse(selectedDate, out filterDate))
            {
                var punchingEvents = await _punchEventRepository.GetPunchEventsForMonthAsync(filterDate.Month, filterDate.Year);
                viewPunchEvents = punchingEvents.Select(p => new ViewPunchEvents
                {
                    Id = p.Id,
                    EmployeeId = p.EmployeeId,
                    PunchIn = p.PunchIn,
                    PunchOut = p.PunchOut,
                    EmployeeName = p.Employee != null ? p.Employee.Name : "N/A" // Handle null Employee

                });

            }
            else
            {
                var currentDate = DateTime.Now;
                var currentMonth = currentDate.Month;
                var currentYear = currentDate.Year;

                var allPunchingEvents = await _punchEventRepository.GetPunchEventsForMonthAsync(currentMonth, currentYear);
                viewPunchEvents = allPunchingEvents.Select(p => new ViewPunchEvents
                {
                    Id = p.Id,
                    EmployeeId = p.EmployeeId,
                    PunchIn = p.PunchIn,
                    PunchOut = p.PunchOut,
                    EmployeeName = p.Employee.Name
                });
            }

            return View(viewPunchEvents);
        }


        [HttpPost]
        public async Task<IActionResult> CreatePunch(CreatePunchEvent punchEvent)
        {
            // Check if the punch record already exists for the specified employee and date
            var existingPunchEvents = await _punchEventRepository.GetPunchEventsForEmployeeAndDateAsync(punchEvent.EmployeeId, punchEvent.PunchIn.Date);

            if (existingPunchEvents.Any())
            {
                // If punch records already exist, update each one
                foreach (var existingPunchEvent in existingPunchEvents)
                {
                    existingPunchEvent.PunchIn = punchEvent.PunchIn;
                }
            }
            else
            {
                // If no punch records exist, create a new one
                var punchEventobj = new PunchEvent
                {
                    EmployeeId = punchEvent.EmployeeId,
                    PunchIn = punchEvent.PunchIn
                    //PunchOut = punchEvent.PunchOut
                };

                if (ModelState.IsValid)
                {
                    await _punchEventRepository.CreatePunchEventAsync(punchEventobj);
                }
            }

            return RedirectToAction("Index", "Employee");
        }

        [HttpGet]
        public async Task<IActionResult> CheckPunchExists(int employeeId)
        {
            var today = DateTime.Today;
            var punchExists = await _punchEventRepository.CheckPunchExistsForEmployeeAsync(employeeId, today);

            if (punchExists)
            {
                var punchInTime = await _punchEventRepository.GetPunchInTimeForEmployeeAsync(employeeId, today);

                // Check if punchInTime is not null
                if (punchInTime.HasValue)
                {
                    var formattedPunchInTime = punchInTime.Value.ToString("yyyy-MM-ddTHH:mm:ss");

                    return Json(new { punchExists = true, formattedPunchInTime });
                }
                else
                {
                    // Log or debug here to see if punchInTime is null
                    return Json(new { punchExists = true, formattedPunchInTime = "NULL" });
                }
            }
            else
            {
                return Json(new { punchExists = false });
            }
        }


        public async Task DeductLeaveBasedOnPunchDurationAsync(Employee employee, PunchEvent punchEvent)
        {
            // Calculate punch duration in hours
            TimeSpan duration = punchEvent.PunchOut - punchEvent.PunchIn;
            double durationHours = duration.TotalHours;

            // Check if it's a Saturday (assuming Saturday is the only special case)
            if (punchEvent.PunchIn.DayOfWeek == DayOfWeek.Saturday)
            {
                if (durationHours >= 4 && durationHours < 8)
                {
                    // Deduct 0.75 CL for 4 to 8 hours on a Saturday
                    await IncrementPresentDaysAsync(employee);
                }
                else if (durationHours >= 8)
                {
                    await IncrementPresentDaysAsync(employee);
                }
                else if (durationHours < 4)
                {
                    await DeductLeaveAsync(employee, 1);
                    await IncrementAbsentDaysAsync(employee);
                }
            }
            else // For regular days
            {
                if (durationHours >= 4 && durationHours < 8)
                {
                    // Deduct 0.5 CL for 4 to 8 hours
                    await DeductLeaveAsync(employee, (int)0.5);
                }
                else if (durationHours >= 8)
                {
                    await IncrementPresentDaysAsync(employee);
                }
                else if (durationHours < 4)
                {
                    await DeductLeaveAsync(employee, 1);
                    await IncrementAbsentDaysAsync(employee);
                }
            }

           
         
        }

        // Deduct leave from the employee's CL count and update the database.
        public async Task DeductLeaveAsync(Employee employee, int leaveCount)
        {
            if (employee.CL >= leaveCount)
            {
                // Deduct the specified leave count from the employee's CL
                employee.CL -= leaveCount;
                // Update the employee's CL count in your database asynchronously
                await _employeeRepository.UpdateEmployeeCLAsync(employee);
            }
            else
            {
                // Handle insufficient leave balance (e.g., show an error message).
            }
        }

        // Increment the absent days count and update the database.
        public async Task IncrementAbsentDaysAsync(Employee employee)
        {
            employee.AbsentDays++;
            // Update the absent days count in your database asynchronously
            await _employeeRepository.UpdateAbsentDaysAsync(employee);
        }
        public async Task IncrementPresentDaysAsync(Employee employee)
        {
            employee.PresentDays++;
            // Update the present days count in your database asynchronously
            await _employeeRepository.UpdatePresentDaysAsync(employee);
        }
        public IActionResult Edit(int id)
        {
            // Retrieve the punch event from the repository based on the ID
            var punchEvent = _punchEventRepository.GetPunchEventById(id);

            if (punchEvent == null)
            {
                return NotFound();
            }

            // Pass the ViewPunchEvents model to the view
            var viewPunchEvent = new ViewPunchEvents
            {
                Id = punchEvent.Id
            };

            return View(viewPunchEvent);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, [FromBody] ViewPunchEvents punchEvent)
        {
            if (id != punchEvent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var punchEventobj = new PunchEvent
                    {
                        Id = punchEvent.Id,
                        PunchIn = punchEvent.PunchIn,
                        PunchOut = punchEvent.PunchOut,
                        EmployeeId=punchEvent.EmployeeId
                    };

                    await _punchEventRepository.UpdatePunchEventAsync(punchEventobj);

                    var employee = await _employeeRepository.GetEmployeeByIdAsync(punchEvent.EmployeeId);
                    await DeductLeaveBasedOnPunchDurationAsync(employee, punchEventobj);
                }
                catch (Exception)
                {
                    // Handle exception, e.g., log the error
                    return BadRequest("An error occurred while editing the punch event.");
                }
            }

            return Ok("Punch event edited successfully.");
        }

        public async Task<IActionResult> ExportToExcel(string selectedDate)
        {
            IEnumerable<ViewPunchEvents> viewPunchEvents;
            DateTime filterDate;

            if (DateTime.TryParse(selectedDate, out filterDate))
            {
                var punchingEvents = await _punchEventRepository.GetPunchEventsForMonthAsync(filterDate.Month, filterDate.Year);
                viewPunchEvents = punchingEvents.Select(p => new ViewPunchEvents
                {
                    EmployeeId = p.EmployeeId,
                    PunchIn = p.PunchIn,
                    PunchOut = p.PunchOut,
                    EmployeeName = p.Employee != null ? p.Employee.Name : "N/A" // Handle null Employee
                });
            }
            else
            {
                var currentDate = DateTime.Now;
                var currentMonth = currentDate.Month;
                var currentYear = currentDate.Year;

                var allPunchingEvents = await _punchEventRepository.GetPunchEventsForMonthAsync(currentMonth, currentYear);
                viewPunchEvents = allPunchingEvents.Select(p => new ViewPunchEvents
                {
                    EmployeeId = p.EmployeeId,
                    PunchIn = p.PunchIn,
                    PunchOut = p.PunchOut,
                    EmployeeName = p.Employee.Name
                });
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("PunchEvents");

                // Add headers
                var headers = new string[] { "Employee Name", "Date", "Punch In", "Punch Out", "Punch Duration (hours)" };
                for (var i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // Add punch events data
                for (var i = 0; i < viewPunchEvents.Count(); i++)
                {
                    var punchEvent = viewPunchEvents.ElementAt(i);
                    worksheet.Cells[i + 2, 1].Value = punchEvent.EmployeeName;
                    worksheet.Cells[i + 2, 2].Value = punchEvent.PunchIn.Date;
                    worksheet.Cells[i + 2, 3].Value = punchEvent.PunchIn.ToShortTimeString();
                    worksheet.Cells[i + 2, 4].Value = punchEvent.PunchOut.ToShortTimeString();
                    worksheet.Cells[i + 2, 5].Value = double.Parse(punchEvent.PunchDuration) / 60.0;

                }

                // Auto-fit columns for better appearance
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set style for header row
                using (var range = worksheet.Cells[1, 1, 1, headers.Length])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }

                // Convert the package to a byte array
                var fileBytes = package.GetAsByteArray();

                // Set response headers for downloading
                Response.Headers.Add("Content-Disposition", "attachment; filename=PunchEvents.xlsx");
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }


    }
}
