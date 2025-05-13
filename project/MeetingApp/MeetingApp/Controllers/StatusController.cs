using MeetingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace MeetingApp.Controllers
{
    public class StatusController : Controller
    {
        private readonly IConfiguration _configuration;

        public StatusController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // List all statuses
        public IActionResult Index()
        {
            var statuses = new List<Status>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            string query = "SELECT StatusID, Description FROM [MeetingsManagementDB].[dbo].[Status]";

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
                                var status = new Status
                                {
                                    StatusID = reader.GetInt32(0),
                                    Description = reader.GetString(1)
                                };
                                statuses.Add(status);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Internal server error: " + ex.Message);
                }
            }

            return View(statuses);
        }

        // Show create form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Create a new status
        [HttpPost]
        public IActionResult Create(Status status)
        {
            if (status == null || string.IsNullOrEmpty(status.Description))
            {
                return BadRequest("Invalid status data. Description must be provided.");
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            string query = "INSERT INTO [MeetingsManagementDB].[dbo].[Status] (Description) VALUES (@Description)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Description", status.Description);
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
            Status? status = null;
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            string query = "SELECT StatusID, Description FROM [MeetingsManagementDB].[dbo].[Status] WHERE StatusID = @id";

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
                                status = new Status
                                {
                                    StatusID = reader.GetInt32(0),
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

            return View(status);
        }

        // Update an existing status
        [HttpPost]
        public IActionResult Edit(Status status)
        {
            if (status == null || string.IsNullOrEmpty(status.Description))
            {
                return BadRequest("Invalid status data. Description must be provided.");
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            string query = "UPDATE [MeetingsManagementDB].[dbo].[Status] SET Description = @Description WHERE StatusID = @StatusID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@StatusID", status.StatusID);
                        command.Parameters.AddWithValue("@Description", status.Description);
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

        // Delete a status
        public IActionResult Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            string query = "DELETE FROM [MeetingsManagementDB].[dbo].[Status] WHERE StatusID = @id";

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