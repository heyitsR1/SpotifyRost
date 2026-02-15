using Microsoft.AspNetCore.Mvc;
using SpotifyRoast.Models;
using SpotifyRoast.Services;

namespace SpotifyRoast.Controllers
{
    // [Authorize(Roles = "Admin")] // Uncomment when Roles are fully working
    public class RoastPersonalityController : Controller
    {
        private readonly IGeneric<RoastPersonality> _repository;

        // Rijan: Injecting the generic repository
        public RoastPersonalityController(IGeneric<RoastPersonality> repository)
        {
            _repository = repository;
        }

        // GET: RoastPersonality
        public IActionResult Index()
        {
            var response = _repository.GetAll();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return View(response.Datas);
            }
            return View(new List<RoastPersonality>());
        }

        // GET: RoastPersonality/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RoastPersonality/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,SystemPrompt,Icon")] RoastPersonality roastPersonality)
        {
            if (ModelState.IsValid)
            {
                _repository.Insert(roastPersonality);
                return RedirectToAction(nameof(Index));
            }
            return View(roastPersonality);
        }

        // GET: RoastPersonality/Edit/5
        public IActionResult Edit(int id)
        {
            var response = _repository.GetById(id);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return View(response.Data);
            }
            return NotFound();
        }

        // POST: RoastPersonality/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,SystemPrompt,Icon")] RoastPersonality roastPersonality)
        {
            if (id != roastPersonality.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _repository.Update(roastPersonality);
                return RedirectToAction(nameof(Index));
            }
            return View(roastPersonality);
        }

        // GET: RoastPersonality/Delete/5
        public IActionResult Delete(int id)
        {
            var response = _repository.GetById(id);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return View(response.Data);
            }
            return NotFound();
        }

        // POST: RoastPersonality/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var response = _repository.GetById(id);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _repository.Delete(response.Data);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
