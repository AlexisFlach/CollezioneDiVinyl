using CollezioneDiVinyl.Entities;
using System.Collections.Generic;

namespace CollezioneDiVinyl.Repositories
{
    public interface IVinylsRepository 
    {
        IEnumerable<Vinyl> GetVinyls();
        Vinyl GetVinyl(int id);
        void UpdateVinyl(Vinyl vinyl);

        void AddVinyl(Vinyl vinyl);

        void DeleteVinyl(int id);
    }
}
