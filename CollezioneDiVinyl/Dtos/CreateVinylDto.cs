using CollezioneDiVinyl.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CollezioneDiVinyl.Dtos
{
    public class CreateVinylDto
    {   
        [Required]
        public string Title { get; set; }
        public int Artist { get; set; }
    }
}
