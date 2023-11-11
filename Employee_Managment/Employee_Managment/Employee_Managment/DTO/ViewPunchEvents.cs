using System.ComponentModel.DataAnnotations;

namespace Employee_Managment.DTO
{
    public class ViewPunchEvents
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        [DataType(DataType.Time)]
        public DateTime PunchIn { get; set; }

        [DataType(DataType.Time)]
        public DateTime PunchOut { get; set; }
        public string? EmployeeName { get; set; }

        public string PunchDuration
        {
            get
            {
                // Calculate the duration between PunchIn and PunchOut
                if (PunchIn != DateTime.MinValue && PunchOut != DateTime.MinValue)
                {
                    TimeSpan duration = PunchOut - PunchIn;
                    return $"{(int)duration.TotalHours} hours {duration.Minutes} minutes";
                }

                return "N/A"; // Default if either PunchIn or PunchOut is not set
            }
        }

    }

}
