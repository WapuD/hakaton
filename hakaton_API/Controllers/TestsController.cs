using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hakaton_API.Data;
using hakaton_API.Data.Models;
using hakaton_API.Controllers.Services;
using hakaton_API.Controllers.Interface;

namespace hakaton_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly ITestService _testService;
        private readonly ICompetencyService _competencyService;

        public TestsController(DBContext context, ITestService testService, ICompetencyService competencyService)
        {
            _context = context;
            _testService = testService;
            _competencyService = competencyService;
        }

        // GET: api/Tests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Test>>> GetTests()
        {
            var tests = await _testService.GetAllTestsAsync();
            return Ok(tests);
        }

        // GET: api/Tests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Test>> GetTest(int id)
        {
            var test = await _testService.GetTestByIdAsync(id);

            if (test == null)
            {
                return NotFound();
            }

            return Ok(test);
        }

        // PUT: api/Tests/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTest(int id, Test test)
        {
            if (id != test.Id)
            {
                return BadRequest("ID mismatch");
            }

            var success = await _testService.UpdateTestAsync(test);

            if (!success)
            {
                return NotFound("Test not found");
            }

            return NoContent(); // Возвращаем 204 No Content при успешном обновлении
        }

        // POST: api/Tests
        [HttpPost]
        public async Task<ActionResult<TestDto>> PostTest(TestDto testDto)
        {
            // Используем сервис для получения роли по RoleId
            var competency = await _competencyService.GetCompetencyByIdAsync(testDto.CompetencyId);
            if (competency == null)
            {
                return NotFound($"Test with ID {competency.Id} not found.");
            }

            // Преобразование DTO в сущность test
            var test = new Test
            {
                ArticleTest = testDto.ArticleTest,
                CompetencyId = testDto.CompetencyId,
                Question = testDto.Question,
                Answer0 = testDto.Answer0,
                Answer1 = testDto.Answer1,
                Answer2 = testDto.Answer2,
                Answer3 = testDto.Answer3,
                Competency = competency
            };

            _context.Test.Add(test);
            await _context.SaveChangesAsync();

            var createdTestDto = new TestDto
            {
                Id = test.Id,
                ArticleTest = test.ArticleTest,
                CompetencyId = test.CompetencyId,
                Question = test.Question,
                Answer0 = test.Answer0,
                Answer1 = test.Answer1,
                Answer2 = test.Answer2,
                Answer3 = test.Answer3
            };


            return CreatedAtAction("GetTest", new { id = createdTestDto.Id }, createdTestDto);
        }

        // DELETE: api/Tests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTest(int id)
        {
            var success = await _testService.DeleteTestAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        private bool TestExists(int id)
        {
            return _context.Test.Any(e => e.Id == id);
        }
    }
}
