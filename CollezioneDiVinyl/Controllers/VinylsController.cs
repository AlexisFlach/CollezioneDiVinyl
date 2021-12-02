using CollezioneDiVinyl.Entities;
using CollezioneDiVinyl.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CollezioneDiVinyl.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VinylsController : ControllerBase
    {
        private readonly IVinylsRepository _repository;

        public VinylsController(IVinylsRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<Vinyl> GetVinyls()
        {
            var vinyls = _repository.GetVinyls();
            return vinyls;
        }

        [HttpGet("{id}")]
        public ActionResult<Vinyl> GetVinyl(int id)
        {
            var vinyl = _repository.GetVinyl(id);

            if(vinyl == null)
            {
                return NotFound();
            }
            return Ok(vinyl);
        }

        [HttpPut("{id}")]

        public ActionResult UpdateVinyl(Vinyl v, int id)
        {
            var existingItem = _repository.GetVinyl(id);
            if(existingItem == null)
            {
                return NotFound();
            }
            existingItem.Artist = v.Artist;
            existingItem.Title = v.Title;

            _repository.UpdateVinyl(existingItem);

            return Ok();
        }

        [HttpPost]
        public ActionResult<Vinyl> AddVinyl(Vinyl v)
        {
            Vinyl vinyl = new()
            {
                Id = v.Id,
                Artist = v.Artist,
                Title = v.Title,
            };
            _repository.AddVinyl(vinyl);
            return CreatedAtAction(nameof(GetVinyl), new { id = vinyl.Id }, vinyl);
        }
        [HttpDelete("{id}")]

        public ActionResult DeleteVinyl(int id)
        {
            var vinyl = GetVinyl(id);

            _repository.DeleteVinyl(id);

            return NoContent();
        }
    }
}
