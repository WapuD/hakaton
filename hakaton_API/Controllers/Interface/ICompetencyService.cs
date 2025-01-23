using hakaton_API.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hakaton_API.Controllers.Interface
{
    public interface ICompetencyService
    {
        Task<IEnumerable<Competency>> GetAllCompetenciesAsync();
        Task<Competency?> GetCompetencyByIdAsync(int id);
        Task<Competency> CreateCompetencyAsync(Competency competency);
        Task<bool> UpdateCompetencyAsync(int id, Competency competency);
        Task<bool> DeleteCompetencyAsync(int id);
    }
}
