namespace hakaton_WEB.Data
{
    using hakaton_API.Data.Models;
    using Refit;
    public interface IApiClient
    {
        [Get("/roles")]
        Task<IEnumerable<Role>> GetRoleAsync();
        [Get("/roles/{id}")]
        Task<Role> GetRoleAsync(int id);

        [Post("/role")]
        Task<Role> CreateRoleAsync([Body] Role newRole);


        [Get("/tests")]
        Task<IEnumerable<Test>> GetTestAsync();
        [Get("/tests/{id}")]
        Task<Test> GetTestAsync(int id);

        [Get("/employee/{id}")]
        Task<Employee> GetEmployeeByIdAsync(int id);


        [Post("/testings")]
        Task<Testing> PostTestingAsync(TestingDto testing);


        [Post("/surveys")]
        Task<Survey> PostSurveyAsync(Survey survey);
    }
}
