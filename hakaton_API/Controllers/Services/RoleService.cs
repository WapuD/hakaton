using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using hakaton_API.Data;
using hakaton_API.Data.Models;
using hakaton_API.Controllers.Interface;

namespace hakaton_API.Controllers.Services;
public class RoleService : IRoleService
{
    private readonly DBContext _context;

    public RoleService(DBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Role>> GetAllRolesAsync()
    {
        return await _context.Role.ToListAsync();
    }

    public async Task<Role?> GetRoleByIdAsync(int id)
    {
        return await _context.Role.FindAsync(id);
    }

    public async Task<Role> CreateRoleAsync(Role role)
    {
        _context.Role.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<bool> UpdateRoleAsync(int id, Role role)
    {
        if (id != role.Id)
        {
            return false; // ID не совпадают
        }

        _context.Entry(role).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await RoleExists(id))
            {
                return false; // Роли не существует
            }
            throw; // Иначе выбрасываем исключение
        }
    }

    public async Task<bool> DeleteRoleAsync(int id)
    {
        var role = await _context.Role.FindAsync(id);
        if (role == null)
        {
            return false; // Роли не существует
        }

        _context.Role.Remove(role);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<bool> RoleExists(int id)
    {
        return await _context.Role.AnyAsync(e => e.Id == id);
    }
}
