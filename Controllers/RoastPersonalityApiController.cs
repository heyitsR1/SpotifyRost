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
            return StatusCode((int)response.StatusCode, response.ErrorMessage ?? "Error retrieving data.");
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
    }
}
