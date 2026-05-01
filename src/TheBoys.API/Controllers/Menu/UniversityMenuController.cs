using Microsoft.AspNetCore.Mvc;
using TheBoys.Application.Abstractions.Services;
using TheBoys.Application.Dtos;

namespace TheBoys.API.Controllers.Menu
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UniversityMenuController : ControllerBase
    {
        private readonly IUniversityMenuService _menuService;

        public UniversityMenuController(IUniversityMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet("full-menu/{langId:int}")]
        public async Task<ActionResult<List<MenuDto>>> GetFullMenuAsync(
            int langId,
            CancellationToken cancellationToken = default)
        {
            var result = await _menuService.GetFullMenuAsync(langId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("colleges/{langId:int}")]
        public async Task<ActionResult<List<MenuDto>>> GetCollegesMenuAsync(
            int langId,
            CancellationToken cancellationToken = default)
        {
            var result = await _menuService.GetCollegesMenuAsync(langId, cancellationToken);

            if (result == null || !result.Any())
                return NotFound(new { message = "No colleges found" });

            return Ok(result);
        }
    }
}