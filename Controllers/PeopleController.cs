using Microsoft.AspNetCore.Mvc;
using Redis.OM.Searching;
using Redis.OM.Skeleton.Models;

namespace Redis.OM.Skeleton.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PeopleController : ControllerBase
    {
        private readonly RedisCollection<Person> _people;
        private readonly RedisConnectionProvider _provider;
        public PeopleController(RedisConnectionProvider provider)
        {
            _provider = provider;
            _people = (RedisCollection<Person>)provider.RedisCollection<Person>();
        }

        [HttpGet("filterAge")]
        public IList<Person> FilterByAge([FromQuery] int minAge, [FromQuery] int maxAge) =>
            _people.Where(x => x.Age >= minAge && x.Age <= maxAge).ToList();


        [HttpGet("filterGeo")]
        public IList<Person> FilterByGeo([FromQuery] double lon, [FromQuery] double lat, [FromQuery] double radius, [FromQuery] string unit) =>
            _people.GeoFilter(x => x.Address!.Location, lon, lat, radius, Enum.Parse<GeoLocDistanceUnit>(unit)).ToList();


        [HttpGet("filterName")]
        public IList<Person> FilterByName([FromQuery] string firstName, [FromQuery] string lastName) =>
            _people.Where(x => x.FirstName == firstName && x.LastName == lastName).ToList();

        [HttpGet("postalCode")]
        public IList<Person> FilterByPostalCode([FromQuery] string postalCode) =>
            _people.Where(x => x.Address!.PostalCode == postalCode).ToList();

        [HttpGet("fullText")]
        public IList<Person> FilterByPersonalStatement([FromQuery] string text) =>
            _people.Where(x => x.PersonalStatement == text).ToList();

        [HttpGet("streetName")]
        public IList<Person> FilterByStreetName([FromQuery] string streetName) =>
            _people.Where(x => x.Address!.StreetName == streetName).ToList();

        [HttpGet("skill")]
        public IList<Person> FilterBySkill([FromQuery] string skill) =>
            _people.Where(x => x.Skills.Contains(skill)).ToList();


        [HttpPost]
        public async Task<Person> AddPerson([FromBody] Person person)
        {
            await _people.InsertAsync(person);
            return person;
        }

        [HttpPatch("updateAge/{id}")]
        public IActionResult UpdateAge([FromRoute] string id, [FromBody] int newAge)
        {
            foreach (var person in _people.Where(x => x.Id == id))
            {
                person.Age = newAge;
            }
            _people.Save();
            return Accepted();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePerson([FromRoute] string id)
        {
            _provider.Connection.Unlink($"Person:{id}");
            return NoContent();
        }

    }
}
