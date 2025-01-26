using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hakaton_API.Data;
using hakaton_API.Data.Models;

namespace hakaton_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestingsController : ControllerBase
    {
        private readonly DBContext _context;

        public TestingsController(DBContext context)
        {
            _context = context;
        }

        // GET: api/Testings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Testing>>> GetTesting()
        {
            return await _context.Testing.Include(i => i.Employee).Include(i => i.Competency).ToListAsync();
        }

        // GET: api/Testings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Testing>> GetTesting(int id)
        {
            var testing = await _context.Testing.FindAsync(id);

            if (testing == null)
            {
                return NotFound();
            }

            return testing;
        }

        // PUT: api/Testings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTesting(int id, Testing testing)
        {
            if (id != testing.Id)
            {
                return BadRequest();
            }

            _context.Entry(testing).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Testings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Testing>> PostTestingAsync(TestingDto testing)
        {
            var relevance = false;

            if (testing.Score > 3)
            {
                relevance = true;
            }

            var testingNew = new Testing();

            testingNew.Score = testing.Score;
            testingNew.ArticleTest = testing.ArticleTest;
            testingNew.CompetencyId = testing.CompetencyId;
            testingNew.EmployeeId = testing.EmployeeId;

            testingNew.Relevance = relevance;
            testingNew.Date = DateTimeOffset.Now.UtcDateTime;

            testingNew.Competency = await _context.Competency.FindAsync(testing.CompetencyId);
            testingNew.Employee = await _context.Employee.FindAsync(testing.EmployeeId);

            try
            {
                _context.Testing.Add(testingNew);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка при сохранении тестирования.");
            }

            return CreatedAtAction("GetTesting", new { id = testing.Id }, testing);
        }

        // DELETE: api/Testings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTesting(int id)
        {
            var testing = await _context.Testing.FindAsync(id);
            if (testing == null)
            {
                return NotFound();
            }

            _context.Testing.Remove(testing);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TestingExists(int id)
        {
            return _context.Testing.Any(e => e.Id == id);
        }
    }
}
