using System;

namespace CollezioneDiVinyl.Entities
{
    public class Vinyl
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Artist { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
