namespace MeetingApp.Models
{
    public class Minute_Item_Status
    {
        public List<Minute_Item_Status> Minute_Item_StatusList { get; set; }
        public List<Meeting> MeetingList { get; set; }
        public List<Minutes_Item> MinutesList { get; set; }
        public List<Employee> EmployeeList { get; set; }
        public List<Status> StatusList { get; set; }
        public List<int> MeetingIdList { get; set; }
        public List<int> MinutesItemIDList { get; set; }
        public List<int> StatusIDList { get; set; }
        public int MinuteItemStatusID { get; set; }
        public int MeetingId { get; set; }
        public int EmployeeID { get; set; }
        public int MinutesItemID { get; set; }
        public string MinutesDescription { get; set; }
        public string MinutesTitle { get; set; }
        public int StatusID { get; set; }
        public string? Action { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
    }
}
