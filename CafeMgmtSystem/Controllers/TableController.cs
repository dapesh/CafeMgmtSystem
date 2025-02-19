using CafeMgmtSystem.Models;
using CafeMgmtSystem.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CafeMgmtSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ITableRepository _tableRepository;
        public TableController(ITableRepository tableRepository)
        {
            _tableRepository = tableRepository;
        }
        [HttpGet]
        public IActionResult GetAllTables()
        {
            var tables =  _tableRepository.GetAllTables();
            return Ok(tables);
        }
        [HttpGet("{id}")]
        public  IActionResult GetTableById(int id)
        {
            var table =  _tableRepository.GetTableById(id);
            if (table == null)
            {
                return NotFound();
            }
            return Ok(table);
        }
        [HttpPost]
        public  IActionResult CreateTable([FromBody] Table table)
        {
            var result =  _tableRepository.CreateTable(table);
            if (result == 0)
            {
                return BadRequest("Failed to create table.");
            }
            return CreatedAtAction(nameof(GetTableById), new { id = table.Id }, table);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateTable(int id, [FromBody] Table table)
        {
            table.Id = id;
            var result =  _tableRepository.UpdateTable(table);
            if (result > 0)
            {
                return Ok(new { Code = "200", Message = "Table Updated Successfully" });
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public  IActionResult DeleteTable(int id)
        {
            var result =  _tableRepository.DeleteTable(id);
            if (result == 0)
            {
                return Ok(new { Code = "200", Message = "Table Deleted Successfully" });
            }
            return NoContent();
        }
        [HttpPost("book/{id}")]
        public IActionResult BookTable(int id, [FromBody] TableBookingRequest request)
        {
            var success = _tableRepository.BookTable(id, request.ReservedUntil);
            if (!success) return BadRequest("Table booking failed.");
            return Ok(new { Code = "200", Message = "Table booked successfully." });
        }
        [HttpPost("release/{id}")]
        public IActionResult ReleaseTable(int id)
        {
            var success = _tableRepository.ReleaseTable(id);
            if (!success) return BadRequest("Table release failed.");
            return Ok(new { Code = "200", Message = "Table released successfully." });
        }
    }
}
