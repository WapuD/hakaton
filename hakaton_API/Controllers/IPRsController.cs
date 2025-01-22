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
    public class IPRsController : ControllerBase
    {
        private readonly DBContext _context;

        public IPRsController(DBContext context)
        {
            _context = context;
        }

        // GET: api/IPRs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IPR>>> GetIPR()
        {
            return await _context.IPR.ToListAsync();
        }

        // GET: api/IPRs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IPR>> GetIPR(int id)
        {
            var iPR = await _context.IPR.FindAsync(id);

            if (iPR == null)
            {
                return NotFound();
            }

            return iPR;
        }

        // PUT: api/IPRs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIPR(int id, IPR iPR)
        {
            if (id != iPR.Id)
            {
                return BadRequest();
            }

            _context.Entry(iPR).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IPRExists(id))
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

        // POST: api/IPRs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<IPR>> PostIPR(IPR iPR)
        {
            _context.IPR.Add(iPR);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetIPR", new { id = iPR.Id }, iPR);
        }

        // DELETE: api/IPRs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIPR(int id)
        {
            var iPR = await _context.IPR.FindAsync(id);
            if (iPR == null)
            {
                return NotFound();
            }

            _context.IPR.Remove(iPR);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IPRExists(int id)
        {
            return _context.IPR.Any(e => e.Id == id);
        }
    }
}
