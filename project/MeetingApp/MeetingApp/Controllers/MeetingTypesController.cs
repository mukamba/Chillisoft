using MeetingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace MeetingApp.Controllers
{
    public class MeetingTypesController : Controller
    {
        private readonly IConfiguration _configuration;

        public MeetingTypesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // List all meeting types
        public IActionResult Index()
        {
            var meetingTypes = new List<MeetingType>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            string query = "SELECT * FROM [MeetingsManagementDB].[dbo].[meeting_type]";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
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
                catch (Exception ex)
                {
                    return StatusCode(500, "Internal server error: " + ex.Message);
                }
            }

            return View(meetingTypes);
        }

        // Show create form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Create a new meeting type
        [HttpPost]
        public IActionResult Create(MeetingType meetingType)
        {
            if (meetingType == null || string.IsNullOrEmpty(meetingType.Description))
            {
                return BadRequest("Invalid meeting type data. Description must be provided.");
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            string query = "INSERT INTO [MeetingsManagementDB].[dbo].[meeting_type] (Description) VALUES (@Description)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Description", meetingType.Description);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Internal server error: " + ex.Message);
                }
            }

            return RedirectToAction("Index");
        }

        // Show edit form
        [HttpGet]
        public IActionResult Edit(int id)
        {
            MeetingType? meetingType = null;
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            string query = "SELECT [meetingTypeID], [Description] FROM [MeetingsManagementDB].[dbo].[meeting_type] WHERE meetingTypeID = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                meetingType = new MeetingType
                                {
                                    MeetingTypeID = reader.GetInt32(0),
                                    Description = reader.GetString(1)
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Internal server error: " + ex.Message);
                }
            }

            return View(meetingType);
        }

        // Update an existing meeting type
        [HttpPost]
        public IActionResult Edit(MeetingType meetingType)
        {
            if (meetingType == null || string.IsNullOrEmpty(meetingType.Description))
            {
                return BadRequest("Invalid meeting type data. Description must be provided.");
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            string query = "UPDATE [MeetingsManagementDB].[dbo].[meeting_type] SET Description = @Description WHERE meetingTypeID = @MeetingTypeID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MeetingTypeID", meetingType.MeetingTypeID);
                        command.Parameters.AddWithValue("@Description", meetingType.Description);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Internal server error: " + ex.Message);
                }
            }

            return RedirectToAction("Index");
        }

        // Delete a meeting type
        public IActionResult Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            string query = "DELETE FROM [MeetingsManagementDB].[dbo].[meeting_type] WHERE meetingTypeID = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Internal server error: " + ex.Message);
                }
            }

            return RedirectToAction("Index");
        }
    }
}