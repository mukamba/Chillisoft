namespace MeetingApp.Models
{
    public class CreateMinuteItem
    {
        public List<Employee>? EmployeesList { get; set; }
        public List<Status>? StatusesList { get; set; }
        public int MeetingId { get; set; }
        public int EmployeeID { get; set; }
        public int MinutesItemID { get; set; }
        public string? MeetingTypeDescription { get; set; }
        public string? MeetingNumber { get; set; }
        public DateTime MeetingDateCreated { get; set; }
        public int StatusID { get; set; }
        public string? Action { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? Description { get; set; }
        public string? Title { get; set; }
    }
}
