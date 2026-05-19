using Microsoft.AspNetCore.Mvc;
using EventTicket.Data.Repositories;
using EventTicket.Data;

namespace EventTicket.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public CategoryController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpGet]
        public IActionResult GetCategories()
        {
            using var connection = _databaseService.CreateConnection();
            var repo = new CategoryRepository(connection);
            var categories = repo.GetAll();
            return Ok(categories);
        }
    }
}