using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using hakaton_API.Data.Models;
using hakaton_API.Data;
using hakaton_API.Controllers.Interface;


namespace hakaton_API.Controllers.Services;
public class TestService : ITestService
{
    private readonly DBContext _context;

    public TestService(DBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Test>> GetAllTestsAsync()
    {
        // Жадная загрузка Competency
        return await _context.Test
            .Include(t => t.Competency)
            .ToListAsync();
    }

    public async Task<Test?> GetTestByIdAsync(int id)
    {
        // Жадная загрузка Competency для одного теста
        return await _context.Test
            .Include(t => t.Competency)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Test> CreateTestAsync(Test test)
    {
        _context.Test.Add(test);
        await _context.SaveChangesAsync();
        return test;
    }
    public async Task<bool> UpdateTestAsync(Test test)
    {
        if (!_context.Testing.Any(t => t.Id == test.Id))
        {
            return false; // Тест не найден
        }

        _context.Entry(test).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            return true; // Успешно обновлено
        }
        catch (DbUpdateConcurrencyException)
        {
            return false; // Ошибка при обновлении
        }
    }

    public async Task<bool> DeleteTestAsync(int id)
    {
        var test = await _context.Test.FindAsync(id);
        if (test == null)
        {
            return false;
        }

        _context.Test.Remove(test);
        await _context.SaveChangesAsync();
        return true;
    }
}
