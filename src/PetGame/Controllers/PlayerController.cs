using Microsoft.AspNetCore.Mvc;
using PetGame.Models;
using PetGame.Services;

namespace PetGame.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(PostPlayerRequest request, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _playerService.CreatePlayerAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetAsync), new { id = response.Id }, response);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _playerService.GetPlayerAsync(id, cancellationToken);
            return Ok(response);
        }
    }
}