using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTDR.Models;
using RESTDR.Repos;
using System.Globalization;

namespace RESTDR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordsController : ControllerBase
    {
        private readonly IRecordRepo _repo;

        public RecordsController(IRecordRepo repo)
        {
            _repo = repo;
        }

        // GET: api/<RecordsController>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public ActionResult<IEnumerable<Record>> Get([FromQuery] string? artist, [FromQuery] string? titel)
        {
            return _repo.GetAllRecords(artist, titel).ToList() is List<Record> records && records.Count > 0
                ? Ok(records)
                : NoContent();

        }

        // GET api/<RecordsController>/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public ActionResult<Record> Get(int id)
        {
            Record? record = _repo.GetRecordById(id);
            if (record == null)
            {
                return NotFound();
            }
            return Ok(record);
        }

        // POST api/<RecordsController>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize]
        [HttpPost]
        public ActionResult<Record> Post([FromBody] Record newRecord)
        {
            try
            {
                _repo.AddRecord(newRecord);
                return Created($"api/records/{newRecord.Id}", newRecord);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<RecordsController>/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        [HttpPut("{id}")]
        public ActionResult<Record> Put(int id, [FromBody] Record value)
        {
            Record? record = _repo.UpdateRecord(id, value);
            if (record == null)
            {
                return NotFound();
            }
            return Ok(record);
        }

        // DELETE api/<RecordsController>/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        [HttpDelete("{id}")]
        public ActionResult<Record> Delete(int id)
        {
            Record? record = _repo.DeleteRecord(id);
            if (record == null)
            {
                return NotFound();
            }
            return Ok(record);
        }

        [HttpOptions]
        public void Options()
        {
        }
    }
}
