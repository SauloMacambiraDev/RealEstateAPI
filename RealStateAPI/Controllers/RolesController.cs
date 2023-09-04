using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace RealStateAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private RealEStateDbContext _dbContext;

        public RolesController(RealEStateDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _dbContext.Roles.ToListAsync());

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            return Ok(await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id));

        }

        [HttpPost]
        public async Task<IActionResult> Post(Role role)
        {

            _dbContext.Roles.Add(role);
            await _dbContext.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, role);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Role role)
        {
            var existingRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (existingRole == null) return NotFound();

            existingRole.Description = role.Description;
            _dbContext.Roles.Update(existingRole);

            await _dbContext.SaveChangesAsync();

            return Ok("Role Updated!");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (existingRole == null) return NotFound();

            _dbContext.Roles.Remove(existingRole);

            await _dbContext.SaveChangesAsync();

            return StatusCode(StatusCodes.Status204NoContent);
        }

        [HttpGet("GetAllUserByRoleId/{roleID}")]
        public async Task<IActionResult> GetAllUserByRoleId(int roleId)
        {
            var users = await _dbContext.Roles.Where(r => r.Id == roleId).Include(r => r.Users).Select(r => r.Users).ToListAsync();
            return Ok(users);
        }

    }
}
