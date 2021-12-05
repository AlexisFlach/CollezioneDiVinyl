using System;

namespace CollezioneDiVinyl.Dtos
{
    public class VinylDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Artist { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
