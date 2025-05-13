using Microsoft.Data.SqlClient;
using NuGet.Protocol.Plugins;


namespace MeetingApp.Models.SqlClasses
{
    public class SqlProcessing
    {

        public static void InsertNewMeeting(string connectionString, Meeting model)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
            INSERT INTO [MeetingsManagementDB].[dbo].[meeting] 
            ([meetingTypeID], [meeting_number], [date_created]) 
            VALUES (@meetingTypeID, @meeting_number, @date_created)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    TimeZoneInfo southAfricaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("South Africa Standard Time");
                    DateTime southAfricaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, southAfricaTimeZone);
                    // Add parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@meetingTypeID", model.MeetingTypeId);
                    command.Parameters.AddWithValue("@meeting_number", model.MeetingNumber);
                    command.Parameters.AddWithValue("@date_created", southAfricaTime);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Handle or log the error

                    }
                }
            }

        }

        public static List<int> getMeetingIdsList(string connectionString, Meeting model)
        {
            string selectQuery = "SELECT meeting_Id FROM [MeetingsManagementDB].[dbo].[meeting] WHERE meetingTypeID = @MeetingTypeId;";

            // Initialize lists to hold meeting IDs if necessary
            List<int> meetingIds = new List<int>();

            using (SqlConnection connection = new SqlConnection(connectionString))


            {
                // Open the connection
                connection.Open();

                using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@MeetingTypeId", model.MeetingTypeId);

                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            meetingIds.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
            return meetingIds;
        }
        public static List<int> getMeetingIdsListByMinuteID(string connectionString, Meeting model)
        {

            List<int> meetingIds1 = new List<int>();

            if (model.MinutesIDList != null && model.MinutesIDList.Count > 0)

            {
                // Create the SQL query with placeholders for each MinutesID
                string query = "SELECT [meeting_Id] FROM [MeetingsManagementDB].[dbo].[Minute_Item_Status] WHERE minutes_ItemID IN (";

                // Add parameters dynamically
                for (int i = 0; i < model.MinutesIDList.Count; i++)
                {
                    query += "@MinutesID" + i + (i < model.MinutesIDList.Count - 1 ? ", " : ")");
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Prepare the command with the query
                    using (var command = new SqlCommand(query, connection))
                    {
                        // Add each MinutesID as a parameter
                        for (int i = 0; i < model.MinutesIDList.Count; i++)
                        {
                            command.Parameters.AddWithValue("@MinutesID" + i, model.MinutesIDList[i]);
                        }

                        // Execute the query and read the results
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Assuming that the meeting_Id is in the first column (index 0)
                                meetingIds1.Add(reader.GetInt32(0));
                            }
                        }
                    }
                }

            }
            return meetingIds1;
        }
        public static List<Minute_Item_Status> getMinute_Item_StatusBYmeeting_IdANDminutes_ItemID(string connectionString, string commonMeetingIdsStr, string minutesIDListStr)
        {
            var minuteItemStatusList = new List<Minute_Item_Status>();
            string queryMinute_Item_Status = @"SELECT * FROM [MeetingsManagementDB].[dbo].[Minute_Item_Status]  WHERE meeting_Id IN (" + commonMeetingIdsStr + @")AND minutes_ItemID IN (" + minutesIDListStr + @")";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(queryMinute_Item_Status, connection))
                {
                    // Execute the query and process the results
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var status = new Minute_Item_Status
                            {
                                EmployeeID = reader.GetInt32(reader.GetOrdinal("EmployeeID")),
                                MinutesItemID = reader.GetInt32(reader.GetOrdinal("minutes_ItemID")),

                                DueDate = reader.GetDateTime(reader.GetOrdinal("due_date")),
                                StatusID = reader.GetInt32(reader.GetOrdinal("StatusID")),
                                Action = reader.GetString(reader.GetOrdinal("Action")),
                                CompletedDate = reader.IsDBNull(reader.GetOrdinal("completed_date"))
                                    ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("completed_date"))
                            };

                            minuteItemStatusList.Add(status);
                        }
                    }
                }
            }

            return minuteItemStatusList; 

        }
        public static void InsertIntoMeetingANDMinuteItemStatus(string connectionString , Meeting model, List<Minute_Item_Status> minuteItemStatusList)
        {
            string meetingInsertQuery = "INSERT INTO [MeetingsManagementDB].[dbo].[meeting] (meeting_number, meetingTypeID) " +
                                            "VALUES (@MeetingNumber, @MeetingTypeId); " +
                                            "SELECT SCOPE_IDENTITY();"; // Retrieve the newly inserted meeting_Id.

            string minuteItemStatusInsertQuery = "INSERT INTO [MeetingsManagementDB].[dbo].[Minute_Item_Status] " +
                                                 "(meeting_Id, EmployeeID, minutes_ItemID, StatusID, Action, due_date, completed_date) " +
                                                 "VALUES (@MeetingId, @EmployeeID, @MinutesItemID, @StatusID, @Action, @DueDate, @CompletedDate);";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Insert the meeting and get the new meeting_Id
                    int newMeetingId = 0;
                    using (SqlCommand command = new SqlCommand(meetingInsertQuery, connection))
                    {
                        TimeZoneInfo southAfricaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("South Africa Standard Time");
                        DateTime southAfricaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, southAfricaTimeZone);
                        // Add parameters for the insert query
                        command.Parameters.AddWithValue("@MeetingNumber", model.MeetingNumber);
                        command.Parameters.AddWithValue("@MeetingTypeId", model.MeetingTypeId);
                        command.Parameters.AddWithValue("@date_created", southAfricaTime);

                        // Execute the query and retrieve the new meeting_Id
                        var result = command.ExecuteScalar();  // This will return the new meeting_Id.
                        if (result != null)
                        {
                            newMeetingId = Convert.ToInt32(result);
                        }
                    }

                    // Now insert the Minute_Item_Status records using the new meeting_Id
                    using (SqlCommand command = new SqlCommand(minuteItemStatusInsertQuery, connection))
                    {
                        foreach (var item in minuteItemStatusList)
                        {
                            // Set the MeetingId to the new value
                            item.MeetingId = newMeetingId;

                            // Add parameters for the insert query
                            command.Parameters.Clear(); // Clear previous parameters
                            command.Parameters.AddWithValue("@MeetingId", item.MeetingId);
                            command.Parameters.AddWithValue("@EmployeeID", item.EmployeeID);
                            command.Parameters.AddWithValue("@MinutesItemID", item.MinutesItemID);
                            command.Parameters.AddWithValue("@StatusID", item.StatusID);
                            command.Parameters.AddWithValue("@Action", item.Action);
                            command.Parameters.AddWithValue("@DueDate", item.DueDate);
                            command.Parameters.AddWithValue("@CompletedDate", item.CompletedDate ?? (object)DBNull.Value);

                            // Execute the insert for each item in the list
                            command.ExecuteNonQuery();
                        }
                    }
                   
                }


            }
            catch (Exception ex)
            {
                
            }
        }

        public static void DeleteMeeting(string connectionString , int id )
        {
            string query = "DELETE FROM [MeetingsManagementDB].[dbo].[meeting] WHERE meeting_Id = @meetingId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
              
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@meetingId", id);
                        command.ExecuteNonQuery();
                    }     
            }
        }

        public static void DeleteMinutes_Item(string connectionString, int id)
        {
            string query = "DELETE FROM [MeetingsManagementDB].[dbo].[Minutes_Item] WHERE minutes_ItemID = @minutes_ItemID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@minutes_ItemID", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
