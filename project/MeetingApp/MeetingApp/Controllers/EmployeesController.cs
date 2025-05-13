using MeetingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace MeetingApp.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _configuration;


        public EmployeesController(IConfiguration configuration)
        {

            _configuration = configuration;
        }
        public IActionResult Index()
        {
            // Create a list to hold the employee data
            var employees = new List<Employee>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            string query = "SELECT * FROM [MeetingsManagementDB].[dbo].[Employee]";

            // Use ADO.NET to connect to the database and retrieve data
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Execute the query and get the data reader
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Read the data from the SQL DataReader and add it to the employee list
                            while (reader.Read())
                            {
                                var employee = new Employee
                                {
                                    EmployeeID = reader.GetInt32(0),
                                    FirstName = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                    Email = reader.GetString(3),
                                    Title = reader.GetString(4),
                                    DateCreated = reader.GetDateTime(5),
                                    DateModified = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6)
                                };

                                employees.Add(employee);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    return StatusCode(500, "Internal server error" + ex.Message);
                }
            }

            // Return the list of employees to the view
            return View(employees);
        }



        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            if (employee == null || string.IsNullOrEmpty(employee.FirstName) || string.IsNullOrEmpty(employee.LastName) || string.IsNullOrEmpty(employee.Email) || string.IsNullOrEmpty(employee.Title))
            {
                return BadRequest("Invalid employee data. All fields must be provided.");
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            string query = "INSERT INTO [MeetingsManagementDB].[dbo].[Employee] (FirstName, LastName, Email, Title, date_created) " +
                           "VALUES (@FirstName, @LastName, @Email, @Title, GETDATE())";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                        command.Parameters.AddWithValue("@LastName", employee.LastName);
                        command.Parameters.AddWithValue("@Email", employee.Email);
                        command.Parameters.AddWithValue("@Title", employee.Title);
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
        public IActionResult Edit(int id)
        {
            Employee? employee = null;
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            string query = "SELECT * FROM [MeetingsManagementDB].[dbo].[Employee] WHERE EmployeeID = @id";

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
                                employee = new Employee
                                {
                                    EmployeeID = reader.GetInt32(0),
                                    FirstName = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                    Email = reader.GetString(3),
                                    Title = reader.GetString(4),
                                    DateCreated = reader.GetDateTime(5),
                                    //DateModified = reader.GetDateTime(6)
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
            return View(employee);
        }

        [HttpPost]
        public IActionResult Edit(Employee employee)
        {
            if (employee == null) return BadRequest("Invalid employee data.");

            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            string query = "UPDATE [MeetingsManagementDB].[dbo].[Employee] SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Title = @Title, date_modified= GETDATE() WHERE EmployeeID = @EmployeeID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);
                        command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                        command.Parameters.AddWithValue("@LastName", employee.LastName);
                        command.Parameters.AddWithValue("@Email", employee.Email);
                        command.Parameters.AddWithValue("@Title", employee.Title);
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

        public IActionResult Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            string query = "DELETE FROM [MeetingsManagementDB].[dbo].[Employee] WHERE EmployeeID = @id";

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

