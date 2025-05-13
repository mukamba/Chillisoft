namespace MeetingApp.Models
{
    public class Minutes_Item
    {
        public int MinutesItemID { get; set; }
        public string? Description { get; set; }
        public string? Title { get; set; }
        public DateTime? date_created { get; set; }
        public DateTime? date_modified { get; set; }
        public int meetingId { get; set; }
        public string? MeetingDescription { get; set; }
        public DateTime MeetingCreatedDate { get; set; }
        public string? meetingNumber { get; set; }
    }
}
