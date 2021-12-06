﻿﻿# Nästa steg

#### Database Relationships

Relationer tillåter data att bli lagrade i olika tables, men fortfarande ha en länkning till varandra.

För detta använder vi **Primary Keys** och **Foreign Keys**.

Vi pratar här om en **one-to-many relation** där en artist kan ha flera album, men ett album endast kan ha en artist.

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

```
CREATE TABLE Artist(
Id serial PRIMARY KEY,
Name VARCHAR (50) NOT NULL,
Created_At TIMESTAMP NOT NULL
);

alter table Vinyl alter Created_At set default now();
```

```
CREATE TABLE Vinyl (
  Id serial PRIMARY KEY,
  Title VARCHAR (50) NOT NULL,
  Created_At TIMESTAMP NOT NULL,
  Artist_Id serial,
  FOREIGN KEY (Artist_Id)
  REFERENCES Artist (Id)
);
```

```
INSERT INTO Artist(Name) VALUES('Bob Marley');
INSERT INTO Artist(Name) VALUES('Bob Bob Dylan');
```

```
SELECT * FROM Artist; 
```

```
 id |    name    |         created_at
----+------------+----------------------------
  2 | Bob Marley | 2021-12-06 16:50:43.744152
  3 | Bob Dylan  | 2021-12-06 17:02:14.466517
```

```
INSERT INTO Vinyl(Title, Artist_id) VALUES('Get Up Stand Up', '2');
INSERT INTO Vinyl(Title, Artist_id) VALUES('Desire', '3');
```

```
SELECT Artist.Id, Vinyl.Title, Artist.Name
FROM Vinyl
INNER JOIN Artist ON Vinyl.Artist_Id=Artist.Id;
```

```
 id |      title      |    name    
----+-----------------+------------
  2 | Get Up Stand Up | Bob Marley
  3 | Desire          | Bob Dylan
```

#### Dtos

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
Title: Tom Jones Greatest Hits 3
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

**Dtos/VinylDtos.cs**

```
    public class VinylDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Artist { get; set; }
        public DateTime CreatedAt { get; set; }
    }
```

```
     [HttpGet]
        public IEnumerable<VinylDto> GetVinyls()
        {
            var vinyls = _repository.GetVinyls().Select(item => new VinylDto()
            {
                Id = item.Id,
                Title = item.Title,
                Artist = item.Artist,
                CreatedAt = item.CreatedAt,
            });
            return vinyls;
        }
```

Istället för att konvertera våra **Vinyl** till **VinylDto** på detta sätt på varje ställe så kan vi istället använda oss av extension methods.

**Extensions.cs**

```
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
```

```
        [HttpGet]
        public IEnumerable<VinylDto> GetVinyls()
        {
            var vinyls = _repository.GetVinyls().Select(v => v.AsDto()); 
            return vinyls;
        }
```

Extension methods enable you to "add" methods to existing types without creating a new derived type, recompiling, or otherwise modifying the original type. Extension methods are static methods, but they're called as if they were instance methods on the extended type. 

https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods

Vi kan nu göra samma sak när vi returnerar en unik Vinyl.

```
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

Nu kan vi gå tillbaka till vår Post route.

```
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
```

