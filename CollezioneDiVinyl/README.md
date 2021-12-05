# Nästa steg

Börja med att ändra i **Vinyl.cs**

```
    public class Vinyl
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Artist { get; set; }
        public DateTime CreatedAt { get; set; }
    }
```

Lägg märke till property Artist. Det ska representera en **Foreign key** till vår Artist Entity, som vi inte har skapat ännu.

I **VinylsController.cs**

```
private readonly List<Vinyl> _repository = new()
        {
            new() { Id = 1, Artist = 1, Title = "Hard Rain" },
            new() { Id = 2, Artist = 1, Title = "John Wesley Harding" },
            new() { Id = 3, Artist = 2, Title = "12" },
        };
```

Tänk nu att vi befinner oss i vår webbläsare och vill skapa en ny Vinyl. Vi har ett formulär och skriver in informationen vi vill associera med vår entity:

```
Id: 1
Title: Tom Jones Greatest Hits 3,
Artist: (Lista på artister, där vi kommer att ta id:t på den artist vi väljer)
CreatedAt: Här väljer vi datum så behöver vara av typ DateTime.
```

Behöver vi verkligen ge ett id och ett datum? Vore det inte bättre om det kunde lösas automatiskt? 

```
Title: Tom Jones Greatest Hits 3,
Artist: Tom Jones
```

Problemet är att i vår controller ser det ut såhär:

```
[HttpPost]
	public ActionResult<Vinyl> AddVinyl(Vinyl v)
    {
    Vinyl vinyl = new()
    {
    Id = v.Id,
    Artist = v.Artist,
   	Title = v.Title,
   	CreatedAt=v.CreatedAt
    };
    _repository.AddVinyl(vinyl);
    return CreatedAtAction(nameof(GetVinyl), new { id = vinyl.Id }, vinyl);
```

Genom att använda oss av DTO(Data Transfer Object) så kan vi lösa detta. 

**Dtos/CreateVinylDto.cs**

```
public class CreateVinylDto
    { 	
    	[Required]
        public string Title { get; set; }
        
        public int Artist { get; set; }
    }
```

```
[HttpPost]
	public ActionResult<Vinyl> AddVinyl(Vinyl v)
    {
    Vinyl vinyl = new()
    {
    Id = v.Id,
    Artist = v.Artist,
   	Title = v.Title,
   	CreatedAt=v.CreatedAt
    };
    _repository.AddVinyl(vinyl);
    return CreatedAtAction(nameof(GetVinyl), new { id = vinyl.Id }, vinyl);
```

**Dtos/UpdateVinylDto.cs**

```
  public class UpdateVinylDto
    {
        public string Title { get; set; }
        public int Artist { get; set; }
    }
```

```
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

```

