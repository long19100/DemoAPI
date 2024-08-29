using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Dapper; // Đảm bảo bạn đã cài đặt thư viện Dapper

namespace Web_login.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagerController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UserManagerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // API GET
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            const string query = "SELECT * FROM dbo.UserManager";
            var sqlDatasource = _configuration.GetConnectionString("webLoginDBCon");

            try
            {
                using (var connection = new SqlConnection(sqlDatasource))
                {
                    var users = await connection.QueryAsync(query);
                    return Ok(users);
                }
            }
            catch (SqlException sqlEx)
            {
                // Log the SQL exception (optional)
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                // Log the general exception (optional)
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        // API DELETE
        [HttpDelete("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            const string query = "DELETE FROM dbo.UserManager WHERE userId = @Id";
            var sqlDatasource = _configuration.GetConnectionString("webLoginDBCon");

            try
            {
                using (var connection = new SqlConnection(sqlDatasource))
                {
                    Console.WriteLine($"Attempting to delete user with ID: {userId}"); // Log ID người dùng
                    var affectedRows = await connection.ExecuteAsync(query, new { Id = userId });

                    Console.WriteLine($"Number of rows affected: {affectedRows}"); // Log số dòng bị ảnh hưởng

                    if (affectedRows > 0)
                    {
                        return NoContent(); // Successfully deleted, no content to return
                    }
                    else
                    {
                        return NotFound($"User with ID {userId} not found.");
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Log the SQL exception (optional)
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                // Log the general exception (optional)
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

    }
}
