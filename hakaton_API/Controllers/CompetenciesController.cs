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
    public class CompetenciesController : ControllerBase
    {
        private readonly DBContext _context;

        public CompetenciesController(DBContext context)
        {
            _context = context;
        }

        // GET: api/Competencies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Competency>>> GetCompetency()
        {
            return await _context.Competency.ToListAsync();
        }

        // GET: api/Competencies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Competency>> GetCompetency(int id)
        {
            var competency = await _context.Competency.FindAsync(id);

            if (competency == null)
            {
                return NotFound();
            }

            return competency;
        }

        // PUT: api/Competencies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompetency(int id, Competency competency)
        {
            if (id != competency.Id)
            {
                return BadRequest();
            }

            _context.Entry(competency).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompetencyExists(id))
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

        // POST: api/Competencies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Competency>> PostCompetency(Competency competency)
        {
            _context.Competency.Add(competency);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompetency", new { id = competency.Id }, competency);
        }

        // DELETE: api/Competencies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompetency(int id)
        {
            var competency = await _context.Competency.FindAsync(id);
            if (competency == null)
            {
                return NotFound();
            }

            _context.Competency.Remove(competency);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CompetencyExists(int id)
        {
            return _context.Competency.Any(e => e.Id == id);
        }
    }
}
