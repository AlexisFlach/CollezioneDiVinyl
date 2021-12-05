using CollezioneDiVinyl.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CollezioneDiVinyl.Repositories
{
    public class VinylsRepository : IVinylsRepository
    {

        private readonly List<Vinyl> _repository = new()
        {
            new() { Id = 1, Artist = 1, Title = "Hard Rain", CreatedAt=DateTime.Now },
            new() { Id = 2, Artist = 1, Title = "John Wesley Harding", CreatedAt = DateTime.Now },
            new() { Id = 3, Artist = 2, Title = "12", CreatedAt = DateTime.Now  },
        };

        public IEnumerable<Vinyl> GetVinyls()
        {
            return _repository;
        }

        public Vinyl GetVinyl(int id)
        {
            var vinyl = _repository.Where(vinyl => vinyl.Id == id);

            return vinyl.SingleOrDefault();
        }

        public void UpdateVinyl(Vinyl vinyl)
        {
            var index = _repository.FindIndex(existingVinyl => existingVinyl.Id == vinyl.Id);
            _repository[index] = vinyl;
        }

        public void AddVinyl(Vinyl vinyl)
        {
            _repository.Add(vinyl);
        }

        public void DeleteVinyl(int id)
        {
            var index = _repository.FindIndex(exVinyl => exVinyl.Id == id);
            _repository.RemoveAt(index);
        }
    }
    
}
