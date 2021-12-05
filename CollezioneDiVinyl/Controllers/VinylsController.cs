using CollezioneDiVinyl.Entities;
using CollezioneDiVinyl.Dtos;
using CollezioneDiVinyl.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public IEnumerable<VinylDto> GetVinyls()
        {
            var vinyls = _repository.GetVinyls().Select(v => v.AsDto()); 
            return vinyls;
        }

        [HttpGet("{id}")]
        public ActionResult<VinylDto> GetVinyl(int id)
        {
            var vinyl = _repository.GetVinyl(id);

            if(vinyl == null)
            {
                return NotFound();
            }
            return Ok(vinyl.AsDto());
        }

        [HttpPut("{id}")]

        public ActionResult UpdateVinyl(UpdateVinylDto v, int id)
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
        public ActionResult<Vinyl> AddVinyl(CreateVinylDto v)
        {
            Vinyl vinyl = new()
            {
                Artist = v.Artist,
                Title = v.Title,
            };
            Random random = new Random();

            vinyl.Id = random.Next(1, 255);
            vinyl.CreatedAt = DateTime.Now;
            _repository.AddVinyl(vinyl);
            return CreatedAtAction(nameof(GetVinyl), new { id = vinyl.Id }, vinyl.AsDto());
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
