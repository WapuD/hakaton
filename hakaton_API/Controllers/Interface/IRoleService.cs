using hakaton_API.Data.Models;

namespace hakaton_API.Controllers.Interface
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role?> GetRoleByIdAsync(int id);
        Task<Role> CreateRoleAsync(Role role);
        Task<bool> UpdateRoleAsync(int id, Role role);
        Task<bool> DeleteRoleAsync(int id);
    }
}
