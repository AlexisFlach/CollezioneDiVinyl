using CollezioneDiVinyl.Dtos;
using CollezioneDiVinyl.Entities;

namespace CollezioneDiVinyl
{
        public static class Extensions
        {
        public static VinylDto AsDto(this Vinyl vinyl)
        {
            return new VinylDto()
            {
                Id = vinyl.Id,
                Title = vinyl.Title,
                Artist = vinyl.Artist,
                CreatedAt = vinyl.CreatedAt,
            };
        }
    }
}
