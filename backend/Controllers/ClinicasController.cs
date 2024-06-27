using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClinicasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Clinicas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Clinica>>> GetClinicas()
        {
            return await _context.Clinicas.ToListAsync();
        }

        // GET: api/Clinicas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Clinica>> GetClinica(int id)
        {
            var clinica = await _context.Clinicas.FindAsync(id);

            if (clinica == null)
            {
                return NotFound();
            }

            return clinica;
        }

        // PUT: api/Clinicas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClinica(int id, Clinica clinica)
        {
            if (id != clinica.Id)
            {
                return BadRequest();
            }

            _context.Entry(clinica).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClinicaExists(id))
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

        // POST: api/Clinicas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Clinica>> PostClinica(Clinica clinica)
        {
            _context.Clinicas.Add(clinica);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClinica", new { id = clinica.Id }, clinica);
        }

        // DELETE: api/Clinicas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClinica(int id)
        {
            var clinica = await _context.Clinicas.FindAsync(id);
            if (clinica == null)
            {
                return NotFound();
            }

            _context.Clinicas.Remove(clinica);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClinicaExists(int id)
        {
            return _context.Clinicas.Any(e => e.Id == id);
        }
    }
}
