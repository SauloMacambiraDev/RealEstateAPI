using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace RealStateAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {

        private RealEStateDbContext _dbContext;

        public CategoriesController(RealEStateDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        // GET - api/categories
        [HttpGet]
        public async Task<IEnumerable<Category>> Get()
        {
            //return categories;
            return await _dbContext.Categories.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id));
        }

        [HttpGet("[action]")]
        public IActionResult GetSortCategories(string sortBy =  "Name")
        {
            switch (sortBy) { 
                case "Name":
                    return Ok(_dbContext.Categories.OrderBy(c => c.Name).ToList());
                case "ImageUrl":
                    return Ok(_dbContext.Categories.OrderBy(c => c.ImageUrl).ToList());
                case "Description":
                    return Ok(_dbContext.Categories.OrderBy(c => c.Description).ToList());
                default:
                    return Ok(_dbContext.Categories.OrderBy(c => c.Name).ToList());
            }
        }

        // POST - api/categories
        [HttpPost]
        //public void Post([FromBody] Category category)
        public async Task<IActionResult> Post(Category category)
        {
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
            return Created($"Category '{category.Name}' created successfully", category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Category category)
        {
            if (id == 0) return NotFound();

            var categoryFound = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if(categoryFound == null) return NotFound();

            categoryFound.Name = category.Name;
            categoryFound.Description = category.Description;
            categoryFound.ImageUrl = category.ImageUrl;

            await _dbContext.SaveChangesAsync();

            return Ok("Category successfully updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0) return NotFound();

            var categoryFound = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (categoryFound == null) return NotFound();

            _dbContext.Remove(categoryFound);
            await _dbContext.SaveChangesAsync();
                
            return Ok("Category successfully deleted");
        }
    }

    
}
