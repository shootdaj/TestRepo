using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using ZLClientTest.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZLClientTest.API.Controllers
{
	[Route("api/[controller]")]
    public class ProgramSetsController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<ProgramSet> Get()
        {
            return new ProgramSet[]
            {
	            new ProgramSet() { Name = "RainbowSet"},
				new ProgramSet() { Name = "CylonSet"},
				new ProgramSet() { Name = "N00bSet"},
			};
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

}
