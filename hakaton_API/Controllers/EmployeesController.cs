using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hakaton_API.Data;
using hakaton_API.Data.Models;
using hakaton_API.Controllers.Interface;
using hakaton_API.Controllers.Services;

namespace hakaton_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IRoleService _roleService;

        public EmployeesController(DBContext context, IRoleService roleService)
        {
            _context = context;
            _roleService = roleService;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee()
        {
            return await _context.Employee
                .Include(e => e.Role) // Жадная загрузка роли
                .ToListAsync();
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployeeByIdAsync(int id)
        {
            var employee = await _context.Employee.FindAsync(id);

            if (employee == null)
            {
                employee = await _context.Employee.FindAsync(1);
                return employee;
            }

            return employee;
        }

        [HttpGet("auth/{login},{password}")]
        public async Task<ActionResult<Employee>> GetAuthEmployee(string login, string password)
        {
            

            var employee = await _context.Employee
                .FirstOrDefaultAsync(u => u.Login == login && u.Password == password);

            if (employee == null)
            {
                return Unauthorized(); // Возвращаем 401 Unauthorized
            }

            return Ok(employee); // Возвращаем 200 OK с данными сотрудника
        }


        // Define a model for login request
        public class LoginRequest
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }


        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> PostEmployee(EmployeeDto employeeDto)
        {
            // Используем сервис для получения роли по RoleId
            var role = await _roleService.GetRoleByIdAsync(employeeDto.RoleId);
            if (role == null)
            {
                return NotFound($"Role with ID {employeeDto.RoleId} not found.");
            }

            // Преобразование DTO в сущность Employee
            var employee = new Employee
            {
                Surname = employeeDto.Surname,
                Name = employeeDto.Name,
                Patronymic = employeeDto.Patronymic,
                RoleId = employeeDto.RoleId,
                Role = role
            };

            _context.Employee.Add(employee);
            await _context.SaveChangesAsync();

            var createdEmployeeDto = new EmployeeDto
            {
                Id = employee.Id,
                Surname = employee.Surname,
                Name = employee.Name,
                Patronymic = employee.Patronymic,
                RoleId = employee.RoleId
            };

            return CreatedAtAction("GetEmployee", new { id = createdEmployeeDto.Id }, createdEmployeeDto);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.Id == id);
        }
    }
}
