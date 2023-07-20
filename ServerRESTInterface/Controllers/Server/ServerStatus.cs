﻿using Microsoft.AspNetCore.Mvc;
using ServerRESTInterface.Models.ServerStatus;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ServerRESTInterface.Controllers.Server
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerStatus : ControllerBase
    {
        // GET: api/<ServerStatus>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ServerStatus>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ServerStatus>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ServerStatus>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ServerStatus>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        //public SystemInfoModel getSystemInfoModel()
        //{
            

        //    return SystemInfoModel;
        //}
    }
}
