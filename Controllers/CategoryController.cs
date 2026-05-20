using Microsoft.AspNetCore.Mvc;
using EventTicket.Data.Repositories;

namespace EventTicket.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly EventTicketModule _module;

        public CategoryController(EventTicketModule module)
        {
            _module = module;
        }

        [HttpGet]
        public IActionResult GetCategories()
        {
            using var connection = _module.CreateConnection();
            var repo = new CategoryRepository(connection);
            return Ok(repo.GetAll());
        }
    }
}