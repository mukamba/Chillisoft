
using Microsoft.Data.SqlClient;

namespace MeetingApp.Models.SqlClasses
{
    public class SQLGetMinutesItemsByMeetingType
    {
        public static List<int> getMeetingIds(string connectionString, int meetingTypeId)
        {
            // Query to get the MeetingIds based on the meetingTypeId
            string queryMeetings = "SELECT meeting_Id FROM [MeetingsManagementDB].[dbo].[meeting] WHERE meetingTypeID = @meetingTypeId";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Get all MeetingIds for the given meetingTypeId
                List<int> meetingIds = new List<int>();
                using (var command = new SqlCommand(queryMeetings, connection))
                {
                    command.Parameters.AddWithValue("@meetingTypeId", meetingTypeId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            meetingIds.Add(reader.GetInt32(0));
                        }
                    }
                }
 
                return meetingIds;
            } 
        }

        public static List<Minute_Item_Status> getMinuteItemStatusesList(string connectionString, List<int> meetingIds, List<Minute_Item_Status> minuteItemStatuses) 
        {
            string queryMinuteItemStatus = "SELECT * FROM [MeetingsManagementDB].[dbo].[Minute_Item_Status] WHERE meeting_Id = @meetingId";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (var meetingId in meetingIds)
                {
                   
                    using (var command = new SqlCommand(queryMinuteItemStatus, connection))
                    {
                        command.Parameters.AddWithValue("@meetingId", meetingId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var minuteItemStatus = new Minute_Item_Status
                                {
                                    MinuteItemStatusID = reader.GetInt32(0),
                                    MeetingId = reader.GetInt32(1),
                                    EmployeeID = reader.GetInt32(2),
                                    MinutesItemID = reader.GetInt32(3),
                                    StatusID = reader.GetInt32(4),
                                    Action = reader.GetString(5),
                                    DueDate = reader.GetDateTime(6),
                                    CompletedDate = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7)
                                };
                                minuteItemStatuses.Add(minuteItemStatus);
                            }
                        }
                    }
                }
            }
            return minuteItemStatuses;
        }
        public static List<Minutes_Item> getMinute_ItemsList(string connectionString, List<Minute_Item_Status> minuteItemStatuses, List<Minutes_Item> minute_Items)
        {

            string queryMinutesItem = "SELECT * FROM [MeetingsManagementDB].[dbo].[Minutes_Item] WHERE Minutes_ItemID = @minutesItemId";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Retrieve Minutes_Items based on the Minutes_ItemID
                foreach (var minuteItemStatus in minuteItemStatuses)
                {
                    using (var command = new SqlCommand(queryMinutesItem, connection))
                    {
                        command.Parameters.AddWithValue("@minutesItemId", minuteItemStatus.MinutesItemID);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var minuteItem = new Minutes_Item
                                {
                                    MinutesItemID = reader.GetInt32(0),
                                    Description = reader.IsDBNull(1) ? null : reader.GetString(1),
                                    Title = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    date_created = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                                    date_modified = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                                };
                                minute_Items.Add(minuteItem);
                            }
                        }
                    }
                }
                return minute_Items; 
            }
        }

    }
}
