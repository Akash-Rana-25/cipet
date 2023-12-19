using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee_Managment.Models
{
    public class PunchEvent
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Display(Name = "Punch In")]
        [DataType(DataType.Time)]
        public DateTime PunchIn { get; set; }

        [Display(Name = "Punch Out")]
        [DataType(DataType.Time)]
        public DateTime PunchOut { get; set; }

        [NotMapped] // Exclude this property from the database
        public int PunchDuration
        {
            get
            {
                // Calculate the duration between PunchIn and PunchOut
                if (PunchIn != DateTime.MinValue && PunchOut != DateTime.MinValue)
                {
                    TimeSpan duration = PunchOut - PunchIn;
                    return (int)duration.TotalMinutes; // Store the duration in minutes
                }

                return 0; // Default if either PunchIn or PunchOut is not set
            }
        }
        public virtual Employee Employee { get; set; }
    }


}
