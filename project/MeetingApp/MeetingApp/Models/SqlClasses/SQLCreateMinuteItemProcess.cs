using Microsoft.Data.SqlClient;

namespace MeetingApp.Models.SqlClasses
{
    public class SQLCreateMinuteItemProcess
    {
        public static void InsertMinutes_ItemANDMinute_Item_Status(string connectionString, CreateMinuteItem model)
        {
            string insertMinutesItemQuery = @"
                                            INSERT INTO [MeetingsManagementDB].[dbo].[Minutes_Item] 
                                            ([Description], [Title], [Date_Created]) 
                                            OUTPUT INSERTED.[Minutes_ItemID] 
                                            VALUES (@Description, @Title, GETDATE())";

            string insertMinuteItemStatusQuery = @"
                                                INSERT INTO [MeetingsManagementDB].[dbo].[Minute_Item_Status] 
                                                ( [meeting_Id], [EmployeeID], [Minutes_ItemID], [StatusID], [Action], [due_date], [completed_date]) 
                                                VALUES ( @MeetingId, @EmployeeID, @Minutes_ItemID, @StatusID, @Action, @DueDate, @CompletedDate)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Insert into Minutes_Item and get the generated Minutes_ItemID
                int minutesItemId;
                using (SqlCommand command = new SqlCommand(insertMinutesItemQuery, connection))
                {
                    command.Parameters.AddWithValue("@Description", model.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Title", model.Title ?? (object)DBNull.Value);

                    minutesItemId = (int)command.ExecuteScalar();  // Retrieves the newly generated Minutes_ItemID
                }

                // Insert into Minute_Item_Status with the retrieved Minutes_ItemID
                using (SqlCommand command = new SqlCommand(insertMinuteItemStatusQuery, connection))
                {
                    command.Parameters.AddWithValue("@MeetingId", model.MeetingId);
                    command.Parameters.AddWithValue("@EmployeeID", model.EmployeeID);
                    command.Parameters.AddWithValue("@StatusID", model.StatusID);
                    command.Parameters.AddWithValue("@Action", model.Action ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DueDate", model.DueDate);
                    command.Parameters.AddWithValue("@CompletedDate", model.CompletedDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Minutes_ItemID", minutesItemId);

                    command.ExecuteNonQuery();
                }

            }
        }

        public static void UpdateMinuteItem(string connectionString, int id, Minutes_Item model)
        {
            string query = "UPDATE [MeetingsManagementDB].[dbo].[Minutes_Item] SET [Title] = @Title, [Description] = @Description, [date_modified] = @DateModified WHERE [minutes_ItemID] = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to avoid SQL injection
                    command.Parameters.AddWithValue("@id", model.MinutesItemID);
                    command.Parameters.AddWithValue("@Title", model.Title);
                    command.Parameters.AddWithValue("@Description", model.Description);
                    command.Parameters.AddWithValue("@DateModified", DateTime.Now);
                    // Execute the update query
                    command.ExecuteNonQuery();
                }
            }
        }



    }
}
