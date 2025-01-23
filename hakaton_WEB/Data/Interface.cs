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

        [Post("/roles")]
        Task<Role> CreateRoleAsync([Body] Role newRole);


        [Get("/tests")]
        Task<IEnumerable<Test>> GetTestAsync();
    }
}
