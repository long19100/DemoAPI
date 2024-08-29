using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Web_login.Controllers
{
    public class LoginRequest
    {
        public string Taikhoan { get; set; }
        public string Matkhau { get; set; }
    }

    public class RegisterRequest
    {
        public string Taikhoan { get; set; }
        public string Matkhau { get; set; }
        public string Hoten { get; set; }
        public string Diachi { get; set; }
        public string Gioitinh { get; set; }
    }




    [Route("api/[controller]")]
    [ApiController]
    public class Login : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public Login(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //Api GET
        [HttpGet]
        [Route("GetUser")]
        public JsonResult GetUser()
        {
            string query = "SELECT * FROM dbo.[User]";  
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("webLoginDBCon");

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    using (SqlDataReader myReader = myCommand.ExecuteReader())
                    {
                        table.Load(myReader);
                    }
                }
            }

            return new JsonResult(table);
        }

        //API DELETE
        [HttpDelete]
        [Route("DeleteUser/{id}")]
        public IActionResult DeleteUser(int id)
        {
            
            string query = "DELETE FROM dbo.[User] WHERE id = @id";
            string sqlDatasource = _configuration.GetConnectionString("webLoginDBCon");

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    
                    myCommand.Parameters.AddWithValue("@id", id);

                    
                    int result = myCommand.ExecuteNonQuery();

                    if (result > 0)
                    {
                        return Ok(" Successfully.");
                    }
                    else
                    {
                        return NotFound("User not found.");
                    }
                }
            }
        }

        //Xử lý đăng nhập
        [HttpPost]
        [Route("Login")]
        public IActionResult Authenticate([FromBody] LoginRequest request)
        {
            string query = "SELECT COUNT(*) FROM dbo.[User] WHERE taikhoan = @taikhoan AND matkhau = @matkhau";
            string sqlDatasource = _configuration.GetConnectionString("webLoginDBCon");

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@taikhoan", request.Taikhoan);
                    myCommand.Parameters.AddWithValue("@matkhau", request.Matkhau);

                    int count = (int)myCommand.ExecuteScalar();

                    if (count > 0)
                    {
                        return Ok(new { message = "Đăng nhập thành công." });
                    }
                    else
                    {
                        return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không đúng." });
                    }
                }
            }
        }

        //Add tk khi đăng ký
        [HttpPost]
        [Route("Register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            string sqlDatasource = _configuration.GetConnectionString("webLoginDBCon");

            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                try
                {
                    myCon.Open();

                    //Check tài khoản 
                    string checkQuery = "SELECT COUNT(*) FROM dbo.[User] WHERE taikhoan = @taikhoan";
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, myCon))
                    {
                        checkCommand.Parameters.AddWithValue("@taikhoan", request.Taikhoan ?? (object)DBNull.Value);
                        int count = (int)checkCommand.ExecuteScalar();

                        if (count > 0)
                        {
                            return Conflict(new { message = "Tài khoản đã tồn tại." });
                        }
                    }

                    // Add tài khoản vào SQL
                    string insertQuery = "INSERT INTO dbo.[User] (taikhoan, matkhau, hoten, diachi, gioitinh) VALUES (@taikhoan, @matkhau, @hoten, @diachi, @gioitinh)";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, myCon))
                    {
                        
                        insertCommand.Parameters.AddWithValue("@taikhoan", request.Taikhoan ?? (object)DBNull.Value);
                        insertCommand.Parameters.AddWithValue("@matkhau", request.Matkhau ?? (object)DBNull.Value);
                        insertCommand.Parameters.AddWithValue("@hoten", request.Hoten ?? (object)DBNull.Value);
                        insertCommand.Parameters.AddWithValue("@diachi", request.Diachi ?? (object)DBNull.Value);
                        insertCommand.Parameters.AddWithValue("@gioitinh", request.Gioitinh ?? (object)DBNull.Value);

                      
                        int rowsAffected = insertCommand.ExecuteNonQuery();

                       
                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Đăng ký thành công." });
                        }
                        else
                        {
                            return BadRequest(new { message = "Đăng ký không thành công." });
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                    return StatusCode(500, new { message = $"Có lỗi xảy ra: {ex.Message}" });
                }
            }
        }

    }







}

