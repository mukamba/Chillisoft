using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;

namespace MeetingApp.Models.SqlClasses
{
    public class SqlDataCollection
    {

        public static List<Employee> getEmployeeList(string connectionString, List<Employee> EmployeeList)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Fetch Employee List
                var employeeQuery = "SELECT * FROM [MeetingsManagementDB].[dbo].[Employee]";
                using (var command = new SqlCommand(employeeQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EmployeeList.Add(new Employee
                            {
                                EmployeeID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                FirstName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                LastName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                Email = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                Title = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                DateCreated = (DateTime)(reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5)),
                                DateModified = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6)
                            });
                        }
                    }
                }

            }


            return EmployeeList;
        }
        public static List<Meeting> getMeetingList(string connectionString, List<Meeting> meetingList)
        {

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var meetingQuery = "SELECT * FROM [MeetingsManagementDB].[dbo].[meeting]";
                using (var command = new SqlCommand(meetingQuery, connection)) // Corrected here, using the existing 'connection'
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            meetingList.Add(new Meeting
                            {
                                MeetingId = reader.GetInt32(0),
                                MeetingTypeId = reader.GetInt32(1),
                                MeetingNumber = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                date_created = reader.GetDateTime(3),
                                date_modified = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }

            return meetingList;
        }
        public static List<Status> getStatusList(string connectionString, List<Status> statusList)
        {

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Fetch Status List
                var statusQuery = "SELECT * FROM [MeetingsManagementDB].[dbo].[status]";
                using (var command = new SqlCommand(statusQuery, connection)) // Corrected here, using the existing 'connection'
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            statusList.Add(new Status
                            {
                                StatusID = reader.GetInt32(0),
                                Description = reader.GetString(1)
                            });
                        }
                    }
                }

            }
            return statusList;

        }
        public static List<Minute_Item_Status> getMinute_Item_StatusList(string connectionString, List<Minute_Item_Status> minutes_Item_StatusList)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var minutesItemQuery = "SELECT * FROM [MeetingsManagementDB].[dbo].[Minute_Item_Status]";
                using (var command = new SqlCommand(minutesItemQuery, connection)) // Corrected here, using the existing 'connection'
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            minutes_Item_StatusList.Add(new Minute_Item_Status
                            {
                                MinuteItemStatusID = reader.GetInt32(0),
                                MeetingId = reader.GetInt32(1),
                                EmployeeID = reader.GetInt32(2),
                                MinutesItemID = reader.GetInt32(3),
                                StatusID = reader.GetInt32(4),
                                Action = reader.GetString(5),
                                DueDate = reader.GetDateTime(6),
                                CompletedDate = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7)
                            });
                        }
                    }
                }

            }
            return minutes_Item_StatusList;

        }
        public static List<Minutes_Item> getMinutes_ItemList(string connectionString, List<Minutes_Item> minutes_ItemList)
        {
            using (var connection = new SqlConnection(connectionString))
            {

                connection.Open();
                var minutesItemQ = "SELECT * FROM [MeetingsManagementDB].[dbo].[Minutes_Item]";
                using (var command = new SqlCommand(minutesItemQ, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            minutes_ItemList.Add(new Minutes_Item
                            {
                                MinutesItemID = reader.GetInt32(0),
                                Description = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                Title = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                date_created = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                                date_modified = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4)
                            });
                        }
                    }
                }

            }
            return minutes_ItemList;
        }

        public static List<MeetingType> getMeetingTypeList(string connectionString, List<MeetingType> meetingTypes)
        {
            string queryMeetingType = "SELECT * FROM [MeetingsManagementDB].[dbo].[meeting_type]";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Fetch Meeting Types
                using (SqlCommand command = new SqlCommand(queryMeetingType, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var meetingType = new MeetingType
                            {
                                MeetingTypeID = reader.GetInt32(0),
                                Description = reader.GetString(1)
                            };
                            meetingTypes.Add(meetingType);
                        }
                    }
                }
            }
            return meetingTypes;
        }

        public static Minutes_Item? getMinutes_ItemByID(string connectionString, int id, Minutes_Item minuteItem)
        {
            string query = "SELECT*  FROM [MeetingsManagementDB].[dbo].[Minutes_Item] WHERE [minutes_ItemID] = @id ";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            minuteItem = new Minutes_Item
                            {
                                MinutesItemID = reader.GetInt32(0),
                                Description = reader.GetString(1),
                                Title = reader.GetString(2),
                                date_created = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                                date_modified = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4)
                            };
                        }
                    }
                }
            }
            return minuteItem;
        } 
  
    }
    
}
