# 01. Simple CRUD

###### Skapa ett nytt Web Api-projekt

Om du använder VS Code

```
dotnet new webapi -n VinylCollection
```

Tryck på f5. Vid problem:

```
dotnet dev-certs https --trust
```

**task.json**

```
"group": {
	"kind": "build",
	"isDefault": true
}
```

Istället för f5:

```
dotnet watch run
```

Om du använder VS Community

1. Create a new project
2. ASP.NET core Web Api

###### Skapa en Entity

I vår kod ser vi att vi ska skapa ett Dog-objekt av typ Animal.

```
Animal dog = new Dog("Dog-1", 1); // Ett nytt Dog-objekt skapas
```

I sammanhanget kallas detta för en **entity** eller **model** eller **domain** och kommer senare att representerar av en table i vår databas:

```
CREATE TABLE Vinyl (
Id int,
Artist varchar(255),
Titke varcha(255)
);
```

I vår applikation "räcker" det med att skapa en klass. När vi senare använder Entity Framework så kommer vår C#-kod att omvandlas till SQL-kod.

```
    public class Vinyl
    {
        public int Id { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
    }
```

###### Hämta samtliga vinyler

```
GET url/api/dogs -> samtliga dogs
```

I vår applikations design kommer vi att använda oss av **The Repository pattern**

Vi har klasser som inkapslar logiken som krävs för att interagera med vår data.

Genom att använda oss av denna design kommer vi att ha ETT ställe där vi accesar vår data vilket i sig gör det enklare för oss att hantera vår kod( vi behöver endast ändra kod på ett ställe).

Låt oss gå igenom vad som händer när vi får in en GET request till https://localhost:5001/api/vinyls

1. Vi får in en request från, och det är vår Controller som ansvarar för att ta emot **request**, och svara med **response**.

```
namespace CollezioneDiVinyl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VinylsController : ControllerBase
    {
        private readonly VinylsRepository _vinylsRepository = new VinylsRepository();

        [HttpGet]
        public IEnumerable<Vinyl> GetVinyls()
        {
            return _vinylsRepository.GetVinyls();
        }
    };
}
```

Ovanför klassen specifiser vi att klassen är en ApiController som ansvara för route "/api/vinyls"

```
[controller]
```

[controller] skalar av "controller " i klass-namner så när vi nu har döpt den till VinylsController så blir det bara "dogs" kvar.

Hade klassen hetat "CatsControllers" så hade controllern ansvarat för route "/api/cats".

Vi skapar ett fält som heter _repository.

Vi delegarar ansvaret att hämta datan vi vill skicka tillbaka till _repository. 

Repository får informationen från controllern, hämtar datan, och skickar tillbaka den.

**IEnumerable** menas att vi får tillbaka något som går att räkna på. Skulle vi senare vilja byta datastruktur till en array så kommer koden inte att gå sönder.

```
namespace CollezioneDiVinyl.Repositories
{
    public class VinylsRepository
    {

        private readonly List<Vinyl> _repository = new()
        {
            new() { Id = 1, Artist = "Bob Dylan", Title = "Hard Rain" },
            new() { Id = 2, Artist = "Bob Dylan", Title = "John Wesley Harding" },
            new() { Id = 1, Artist = "Flamingokvintetten", Title = "12" },
        };

        public IEnumerable<Vinyl> GetVinyls()
        {
            return _repository;
        }
    }
}
```

###### Hämta en vinyl

**VinylsRepository.cs**

```
 public Vinyl GetVinyl(int id)
        {
            var vinyl = _repository.Where(vinyl => vinyl.Id == id);

            return vinyl.SingleOrDefault();
        }
```

**VinylsController.cs**

```
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
```

###### Uppdatera en vinyl

**VinylsRepository.cs**

```
public void UpdateVinyl(Vinyl vinyl)
        {
            var index = _repository.FindIndex(exVinyl => exVinyl.Id == vinyl.Id);
            _repository.Insert(index, vinyl);
        }
```

**VinylsController.cs**

```
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
```

Problemet med varför det inte sparas ligger i 

```
private readonly VinylsRepository _repository = new VinylsRepository();
```

Istället:

```
   private readonly IVinylsRepository _repository;

        public VinylsController(IVinylsRepository repository)
        {
            _repository = repository;
        }
```

**Startup.cs**

```
services.AddSingleton<VinylsRepository, VinylsRepository>();
```

