using Microsoft.AspNetCore.Mvc;
using SpotifyRoast.Models;
using SpotifyRoast.Services;
using System.Collections.Generic;

namespace SpotifyRoast.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoastPersonalityApiController : ControllerBase
    {
        private readonly IGeneric<RoastPersonality> _repository;

        // Kashish: Injected generic repository for the API controller
        public RoastPersonalityApiController(IGeneric<RoastPersonality> repository)
        {
            _repository = repository;
        }

        // GET: api/RoastPersonalityApi
        [HttpGet]
        public ActionResult<IEnumerable<RoastPersonality>> GetRoastPersonalities()
        {
            var response = _repository.GetAll();
            if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Datas != null)
            {
                return Ok(response.Datas);
            }
            
            // Return empty list or error if something goes wrong
            return StatusCode((int)response.StatusCode, response.Message ?? "Error retrieving data.");
        }

        // GET: api/RoastPersonalityApi/5
        [HttpGet("{id}")]
        public ActionResult<RoastPersonality> GetRoastPersonality(int id)
        {
            var response = _repository.GetById(id);
            if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Data != null)
            {
                return Ok(response.Data);
            }
            
            return NotFound();
        }

        // POST: api/RoastPersonalityApi
        [HttpPost]
        public ActionResult<RoastPersonality> CreateRoastPersonality([FromBody] RoastPersonality roastPersonality)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = _repository.Insert(roastPersonality);
            if (response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return CreatedAtAction(nameof(GetRoastPersonality), new { id = roastPersonality.Id }, roastPersonality);
            }

            return StatusCode((int)response.StatusCode, response.Message ?? "Error creating data.");
        }

        // PUT: api/RoastPersonalityApi/5
        [HttpPut("{id}")]
        public IActionResult UpdateRoastPersonality(int id, [FromBody] RoastPersonality roastPersonality)
        {
            if (id != roastPersonality.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = _repository.Update(roastPersonality);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return NoContent();
            }

            return StatusCode((int)response.StatusCode, response.Message ?? "Error updating data.");
        }

        // DELETE: api/RoastPersonalityApi/5
        [HttpDelete("{id}")]
        public IActionResult DeleteRoastPersonality(int id)
        {
            var response = _repository.GetById(id);
            if (response.StatusCode != System.Net.HttpStatusCode.OK || response.Data == null)
            {
                return NotFound();
            }

            // Soft delete
            var personality = response.Data;
            personality.IsDeleted = true;
            
            var updateResponse = _repository.Update(personality);
            if (updateResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return NoContent();
            }

            return StatusCode((int)updateResponse.StatusCode, updateResponse.Message ?? "Error performing soft delete.");
        }
    }
}
