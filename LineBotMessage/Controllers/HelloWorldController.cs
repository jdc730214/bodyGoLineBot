using LineBotMessage.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LineBotMessage.Controllers
{
    [Route("api2/[controller]")]
    [ApiController]
    public class HelloWorldController : ControllerBase
    {
        // GET: api/<HelloWorldController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            Trace.WriteLine("Get");
            return new string[] { "value1", "value2" };
        }

       
        // GET api/<HelloWorldController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<HelloWorldController>
        [HttpPost]
        public string Post([FromBody] string value)
        {
            Trace.WriteLine("Post");
            return "Post【" + value + "】";
        }
        // PUT api/<HelloWorldController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<HelloWorldController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
