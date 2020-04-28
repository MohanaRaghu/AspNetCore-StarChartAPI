using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;


namespace StarChart.Controllers
{
    [ApiController]
    [Route("")]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext ctx)
        {
            _context = ctx;
        }
        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            if (!_context.CelestialObjects.Any(c => c.Id == id))
                return NotFound();

            var celestialObject = _context.CelestialObjects.Where(c => c.Id == id).FirstOrDefault();

            if (_context.CelestialObjects.Any(c => c.OrbitedObjectId == id))
            {

                var celestialObjectList = _context.CelestialObjects.Where(c => c.OrbitedObjectId == id).ToList();

                celestialObject.Satellites = celestialObjectList;
                _context.SaveChanges();

            }
            return Ok(celestialObject);
        }

        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            if (!_context.CelestialObjects.Any(c => c.Name == name))
            {
                return NotFound();
            }
            var celestialobjects = _context.CelestialObjects.Where(c => c.Name == name).ToList();
            foreach (var c in celestialobjects)
            {
                c.Satellites = _context.CelestialObjects.Where(cel => cel.OrbitedObjectId == c.Id).ToList();
            }
            _context.SaveChanges();
            return Ok(celestialobjects);
        }

        [HttpGet()]
        public IActionResult GetAll()
        {
            var celestialobjects = _context.CelestialObjects.ToList();
            celestialobjects.ForEach(cel => cel.Satellites =
            _context.CelestialObjects.Where(c => c.OrbitedObjectId == cel.Id).ToList());
            _context.SaveChanges();
            return Ok(celestialobjects);

        }

        [HttpPost()]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            if (celestialObject == null)
                return BadRequest();
            _context.CelestialObjects.AddAsync(celestialObject);
            _context.SaveChangesAsync();

            return CreatedAtRoute("GetById", new { celestialObject.Id }, celestialObject);
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]CelestialObject celestialObject)
        {
            if (celestialObject == null)
            {
                return BadRequest();
            }
            if (!_context.CelestialObjects.Any(c => c.Id == id))
            {
                return NotFound();
            }
            var celestialObjectEntity = _context.CelestialObjects.Where(c => c.Id == id).FirstOrDefault();
            celestialObjectEntity.Name = celestialObject.Name;
            celestialObjectEntity.OrbitalPeriod = celestialObject.OrbitalPeriod;
            celestialObject.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            if (!_context.CelestialObjects.Any(c => c.Id == id))
            {
                return NotFound();
            }
            var celestialObjectEntity = _context.CelestialObjects.Where(c => c.Id == id).FirstOrDefault();
            celestialObjectEntity.Name = name;
            _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if(!_context.CelestialObjects.Any(c=>c.Id == id))
            {
                return NotFound();
            }
            var celestialObjectsList = _context.CelestialObjects.Where(c => c.Id == id || c.OrbitedObjectId == id).ToList();
            _context.CelestialObjects.RemoveRange(celestialObjectsList);
            _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
