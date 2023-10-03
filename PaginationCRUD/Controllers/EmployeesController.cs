using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PaginationCRUD.Models;
using Sieve.Models;
using Sieve.Services;
using System.Data;
using System.Data.SqlClient;

namespace PaginationCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {

      

        private readonly IConfiguration _config; //to read conn string 
        private readonly SieveProcessor sieveProcessor;
        public EmployeesController(IConfiguration config,SieveProcessor _sieveProcessor)
        {
            _config = config;
            sieveProcessor = _sieveProcessor;

        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetEmployees([FromQuery] SieveModel model) 
        {
            List<Employee> empList = new List<Employee>();
            SqlConnection con = new(_config.GetConnectionString("DefaultConnection").ToString());
            con.Open();
            string selQuery = "SELECT * FROM Employee";
            using (SqlCommand cmd= new SqlCommand(selQuery,con))
            {
                
                using (SqlDataReader reader= cmd.ExecuteReader())
                {
                    while(reader.Read()) 
                    {
                        empList.Add(new Employee {
                          Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["name"].ToString(),
                        Designation= reader["Designation"].ToString()
                        }) ;
                    }
                    if(empList.Count==0)
                    {
                        return NotFound("No employees exist.");
                    }

                    /*var total = empList.Count; //total count of records
                    var totalPages = (int)Math.Ceiling((decimal)total / pageSize);
                    var empPerPage = empList
                        .Skip((page - 1) * pageSize) //skipping over prec page  of data  
                        .Take(pageSize) //get only those prods to be shown 
                        .AsQueryable();*/
                    var emps = empList.AsQueryable();
                    emps = sieveProcessor.Apply(model, emps);

                    return Ok(emps);
                }
                

            }
            con.Close();

        }
        [HttpPost("Add")]
        public async Task<ActionResult<List<Employee>>> CreateEmployee(string name,string designation)
        {
            SqlConnection con = new(_config.GetConnectionString("DefaultConnection").ToString());
            con.Open();
            string insQuery = ("INSERT INTO Employee (Name,Designation) VALUES (@Value1,@Value2)");
            using(SqlCommand cmd= new SqlCommand(insQuery,con))
            {
                cmd.Parameters.AddWithValue("@Value1", name);
                cmd.Parameters.AddWithValue("@Value2",designation);
                cmd.ExecuteNonQuery();
            }
            return Ok("Added new employee details.");
            con.Close();


        }
        [HttpPut("Update/{id}")]
        public async Task<ActionResult<List<Employee>>> UpdateEmployee(int id,string name,string designation)
        {
            SqlConnection con = new(_config.GetConnectionString("DefaultConnection").ToString());
            con.Open();
            string updateQuery = ("UPDATE Employee SET NAME= @VALUE1, DESIGNATION=@VALUE2 WHERE Id=@VALUE3");
            using(SqlCommand cmd= new SqlCommand(@updateQuery,con))
            {
                cmd.Parameters.AddWithValue("@VALUE1",name);
                cmd.Parameters.AddWithValue("@VALUE2",designation);
                cmd.Parameters.AddWithValue("@VALUE3",id);
                cmd.ExecuteNonQuery();  
            }
            return Ok($"Employee number {id} updated.");
            con.Close();
        }
        [HttpDelete("Delete/{id}")]  
        public async Task<ActionResult<List<Employee>>> DeleteEmployee(int id)
        {
            SqlConnection con = new(_config.GetConnectionString("DefaultConnection").ToString());
            con.Open();
            string delQuery = ("DELETE FROM Employee WHERE Id=@VALUE1");
            using(SqlCommand cmd= new SqlCommand(delQuery,con))
            {
                cmd.Parameters.AddWithValue("@VALUE1", id);
                cmd.ExecuteNonQuery();
            }
            return Ok($"Employee number {id} deleted.");
        }

        [HttpDelete("DeleteAll")]
        public async Task<ActionResult<List<Employee>>> DeleteAll()
        {
            SqlConnection con = new(_config.GetConnectionString("DefaultConnection").ToString());
            con.Open();
            string delAll = ("TRUNCATE TABLE Employee");
            using (SqlCommand cmd = new SqlCommand(delAll, con))
            {
                cmd.ExecuteNonQuery();
            }
            return Ok("Deleted all records from table");
        }

       
        
    }
}
