using Microsoft.AspNetCore.Mvc;
using PetGame.Models;
using PetGame.Services;

namespace PetGame.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PetController : ControllerBase
    {
        private readonly IPetService _petService;

        public PetController(IPetService petService)
        {
            _petService = petService;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(PostPetRequest request, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _petService.AdoptPetAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetAsync), new { id = response.Id }, response);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var response = await _petService.GetPetAsync(id);
            return Ok(response);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> PutAsync(int id, PutPetRequest request, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _petService.UpdatePetAsync(id, request, cancellationToken);
            return NoContent();
        }
    }
}
