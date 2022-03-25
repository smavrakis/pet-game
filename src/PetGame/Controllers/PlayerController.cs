using Microsoft.AspNetCore.Mvc;
using PetGame.Common.Constants;
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

        [HttpPost(Name = RouteNames.CreatePlayer)]
        public async Task<IActionResult> PostAsync(PostPlayerRequest player)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _playerService.CreatePlayerAsync(player);
            return CreatedAtRoute(RouteNames.CreatePlayer, response);
        }
    }
}