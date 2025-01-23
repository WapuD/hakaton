using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using hakaton_API.Data;
using hakaton_API.Data.Models;
using hakaton_API.Controllers.Interface;

namespace hakaton_API.Controllers.Services
{
    public class CompetencyService : ICompetencyService
    {
        private readonly DBContext _context;

        public CompetencyService(DBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Competency>> GetAllCompetenciesAsync()
        {
            return await _context.Competency.ToListAsync();
        }

        public async Task<Competency?> GetCompetencyByIdAsync(int id)
        {
            return await _context.Competency.FindAsync(id);
        }

        public async Task<Competency> CreateCompetencyAsync(Competency competency)
        {
            _context.Competency.Add(competency);
            await _context.SaveChangesAsync();
            return competency;
        }

        public async Task<bool> UpdateCompetencyAsync(int id, Competency competency)
        {
            if (id != competency.Id)
            {
                return false; // ID не совпадают
            }

            _context.Entry(competency).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CompetencyExists(id))
                {
                    return false; // Компетенции не существует
                }
                throw; // Иначе выбрасываем исключение
            }
        }

        public async Task<bool> DeleteCompetencyAsync(int id)
        {
            var competency = await _context.Competency.FindAsync(id);
            if (competency == null)
            {
                return false; // Компетенции не существует
            }

            _context.Competency.Remove(competency);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> CompetencyExists(int id)
        {
            return await _context.Competency.AnyAsync(e => e.Id == id);
        }
    }
}
