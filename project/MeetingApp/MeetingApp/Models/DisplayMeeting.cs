namespace MeetingApp.Models
{
    public class DisplayMeeting
    {
        public List<Status>? StatusesList { get; set; }
        public List<Employee>? EmployeesList { get; set; }
        public List<Minute_Item_Status>? Minute_Item_Status_List { get; set; }
        public List<Minutes_Item>? Minutes_Item_List { get; set; }
        public int MeetingId { get; set; }
        public string? MeetingTypeDescription { get; set; }
        public string? MeetingNumber { get; set; }
        public DateTime Meeting_date_created { get; set; }

    }
}
