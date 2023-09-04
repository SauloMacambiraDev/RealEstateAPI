using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using RealStateAPI.DTOs;
using System.Security.Claims;

namespace RealStateAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private RealEStateDbContext _dbContext;

        public PropertiesController(RealEStateDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("[action]/{categoryId}")]
        public async Task<IEnumerable<Property>> GetPropertiesByCategoryId(int categoryId)
        {
            var properties = await _dbContext.Properties
                                            .Where(p => p.CategoryId == categoryId)
                                            .Include(p => p.Category)
                                            .Include(p => p.User)
                                            .Select(p => new Property
                                            {
                                                Id = p.Id,
                                                Name = p.Name,
                                                Detail = p.Detail,
                                                Address = p.Address,
                                                ImageUrl = p.ImageUrl,
                                                IsTrending = p.IsTrending,
                                                Price = p.Price,
                                                Category = p.Category,
                                                User = p.User
                                            })
                                            .ToListAsync();

            if (properties == null) return null;

            return properties;
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetPropertyDetail(int id)
        {
            return Ok(await _dbContext.Properties.FirstOrDefaultAsync(p => p.Id == id));
        }

        
        [HttpGet("[action]")]
        public async Task<IActionResult> GetTrendingProperties()
        {
            return Ok(await _dbContext.Properties.Where(p => p.IsTrending).OrderByDescending(p => p.Id).Take(10).ToListAsync());
        }
        
        
        [HttpGet("[action]")]
        public async Task<IActionResult> GetSearchProperties(string address)
        {
            return Ok(await _dbContext.Properties.Where(p => p.Address.ToLower().Contains(address.ToLower())).ToListAsync());
        }

        
        [HttpPost]
        public async Task<IActionResult> Post(Property property)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            _dbContext.Properties.Add(property);
            await _dbContext.SaveChangesAsync(Convert.ToInt32(userId));
            return StatusCode(StatusCodes.Status201Created);
        }

        [Authorize(Roles = "Admin,Default,Real Estate Broker")]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreatePropertyByOwner(Property property)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var authenticatedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (authenticatedUser == null) return Unauthorized("You're not authorized to use this resource.");

            property.UserId = authenticatedUser.Id;
            _dbContext.Properties.Add(property);
            await _dbContext.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, property);
        }

        [Authorize(Roles = "Admin,Default,Real Estate Broker")]
        [HttpPost("[action]/{propertyId}")]
        public async Task<IActionResult> UpdatePropertyByOwner(int propertyId, [FromBody] Property property)
        {
            var propertyToBeUpdated = await _dbContext.Properties.FirstOrDefaultAsync(p => p.Id == propertyId);
            if (propertyToBeUpdated == null) return NotFound();

            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var authenticatedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (authenticatedUser == null) return Unauthorized("You're not authorized to use this resource.");

            if(property.UserId == 0 || property.UserId != authenticatedUser.Id)
            {
                return Unauthorized("You're not authorized to use this resource");
            }

            propertyToBeUpdated.Name = property.Name;
            propertyToBeUpdated.Address = property.Address;
            propertyToBeUpdated.ImageUrl = property.ImageUrl;
            propertyToBeUpdated.IsTrending = property.IsTrending;
            propertyToBeUpdated.Price = property.Price;
            propertyToBeUpdated.CategoryId = property.CategoryId;
            propertyToBeUpdated.Detail = property.Detail;

            _dbContext.Properties.Update(propertyToBeUpdated);
            await _dbContext.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, property);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Property property)
        {
            //var propertyToBeUpdated = await _dbContext.Properties.FirstOrDefaultAsync(p => p.Id == id);
            var propertyToBeUpdated = await _dbContext.Properties.CountAsync(p => p.Id == id);
            if (propertyToBeUpdated == null || propertyToBeUpdated == 0) return NotFound();

            property.Id = id;
            _dbContext.Properties.Update(property);

            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var propertyToBeDeleted = await _dbContext.Properties.FirstOrDefaultAsync(p => p.Id == id);
            if (propertyToBeDeleted == null) return NotFound();

            _dbContext.Properties.Remove(propertyToBeDeleted);

            await _dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status204NoContent);
        }



    }
}
