using System.ComponentModel.DataAnnotations;

namespace MeetingApp.Models
{
    public class Meeting
    {

        public List<Meeting> MeetingList { get; set; }
        public List<Minutes_Item> MinutesList { get; set; }
        public List<Employee> EmployeeList { get; set; }
        public List<MeetingType>? MeetingTypeList { get; set; }
        public List<Minute_Item_Status> Minute_Item_StatusList { get; set; }
        public int MeetingId { get; set; }
        public int MeetingTypeId { get; set; }

        public string? MeetingNumber { get; set; }
        public DateTime date_created { get; set; }
        public DateTime? date_modified { get; set; }
        public List<int>? MinutesIDList { get; set; }

    }
}
