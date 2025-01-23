using hakaton_API.Data.Models;

namespace hakaton_API.Controllers.Interface
{
    public interface ITestService
    {
        Task<IEnumerable<Test>> GetAllTestsAsync();
        Task<Test?> GetTestByIdAsync(int id);
        Task<Test> CreateTestAsync(Test test);
        Task<bool> UpdateTestAsync(Test test);
        Task<bool> DeleteTestAsync(int id);
    }

}
