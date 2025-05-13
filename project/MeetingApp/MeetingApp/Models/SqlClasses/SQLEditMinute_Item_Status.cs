using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;

namespace MeetingApp.Models.SqlClasses
{
    public class SQLEditMinute_Item_Status
    {
            public enum CheckRecordResult
            {
                FoundAndUpdated,
                NotFound
            }
            public static CheckRecordResult checkRecords(string connectionString, int MeetingId, int MinutesItemID, Minute_Item_Status model, DateTime DueDate, DateTime CompletedDate, string Action, int StatusID)
            {
                string query = "SELECT Minute_Item_StatusID FROM [MeetingsManagementDB].[dbo].[Minute_Item_Status] " +
                          "WHERE Meeting_Id = @MeetingId AND Minutes_ItemID = @MinutesItemID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@MeetingId", MeetingId);
                    command.Parameters.AddWithValue("@MinutesItemID", MinutesItemID);

                    connection.Open();
                    var result = command.ExecuteScalar();

                    if (result != null)
                    {
                        int minuteItemStatusID = Convert.ToInt32(result);
                        UpdateMinute_Item_StatusANDMinutes_Item(minuteItemStatusID, connection, model, Action, StatusID, DueDate, CompletedDate, MinutesItemID);
                        return CheckRecordResult.FoundAndUpdated;
                    }
                    else
                    {
                        return CheckRecordResult.NotFound;
                    }
                }
            }
        public static void UpdateMinute_Item_StatusANDMinutes_Item(int minuteItemStatusID , SqlConnection connection , Minute_Item_Status model, string Action, int StatusID, DateTime DueDate, DateTime CompletedDate, int MinutesItemID)
        {
            string updateQuery = "UPDATE [MeetingsManagementDB].[dbo].[Minute_Item_Status] " +
                                 "SET Action = @Action, Due_Date = @DueDate, completed_date = @CompletedDate ,StatusID =@StatusID WHERE Minute_Item_StatusID = @MinuteItemStatusID";

            SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
            updateCommand.Parameters.AddWithValue("@Action", Action);
            updateCommand.Parameters.AddWithValue("@DueDate", DueDate);
            updateCommand.Parameters.AddWithValue("@StatusID", StatusID);
            updateCommand.Parameters.AddWithValue("@CompletedDate", CompletedDate);
            updateCommand.Parameters.AddWithValue("@MinuteItemStatusID", minuteItemStatusID);

            updateCommand.ExecuteNonQuery();

            // Update the Minutes_Item table with new Description and Title
            string updateMinutesItemQuery = "UPDATE [MeetingsManagementDB].[dbo].[Minutes_Item] " +
                                            "SET Description = @MinutesDescription, title = @MinutesTitle " +
                                            "WHERE Minutes_ItemID = @MinutesItemID";

            SqlCommand updateMinutesItemCommand = new SqlCommand(updateMinutesItemQuery, connection);
            updateMinutesItemCommand.Parameters.AddWithValue("@MinutesDescription", model.MinutesDescription);
            updateMinutesItemCommand.Parameters.AddWithValue("@MinutesTitle", model.MinutesTitle);
            updateMinutesItemCommand.Parameters.AddWithValue("@MinutesItemID", MinutesItemID);

            updateMinutesItemCommand.ExecuteNonQuery();
        }
    }
}
