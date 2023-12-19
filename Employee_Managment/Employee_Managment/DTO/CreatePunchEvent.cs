using System.ComponentModel.DataAnnotations;

namespace Employee_Managment.DTO
{
    public class CreatePunchEvent
    {
      
        public int EmployeeId { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime PunchIn { get; set; }

        //[DataType(DataType.DateTime)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        //public DateTime PunchOut { get; set; }
    }
}
