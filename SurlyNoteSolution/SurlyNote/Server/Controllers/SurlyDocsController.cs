using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using SurlyNote.Shared.Models;
using SurlyNote.Server.Repository;

namespace SurlyNote.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SurlyDocsController : ControllerBase
    {
        private readonly ILogger<SurlyDocsController> log;
        private readonly IDocRepository repo;

        private const string FAKE_USER = "foo";

        public SurlyDocsController(ILogger<SurlyDocsController> logger,
            IDocRepository docRepo)
        {
            log = logger;
            repo = docRepo;
        }
        // GET: api/surlydocs/?search=foo
        [HttpGet()]
        public async Task<IEnumerable<DocListItem>> List([FromQuery(Name ="search")] string search)
        {
            string userId = GetUser();
            //return top 100 order by ModifiedOn DESC NumViews DESC
            var results = await repo.List(userId, search);
            
            return results;
        }

        // GET api/surlydocs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SurlyDoc>> Get(string id)
        {
            var key = new DocumentKey
            {
                Id = id,
                UserId = FAKE_USER
            };
            var doc = await repo.Fetch(key);
            if (null == doc)
                return NotFound();
            
            return new ObjectResult(doc);
        }

        // POST api/<ValuesController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SurlyDoc value)
        {
            value.UserId = FAKE_USER;
            var newDoc = await repo.Create(value);
            string currentUrl = Request.GetDisplayUrl();
            UriBuilder ub = new UriBuilder(currentUrl);
            ub.Path += newDoc.Id;
            string foo = ub.Uri.ToString();
            return Created(ub.Uri, newDoc);
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id, [FromBody] SurlyDoc value)
        {
            var key = new DocumentKey
            {
                Id = id,
                UserId = FAKE_USER
            };
            await repo.Update(key, value);
            return NoContent();
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var key = new DocumentKey
            {
                Id = id,
                UserId = FAKE_USER
            };
            
            await repo.Delete(key);
            return NoContent();
        }

        private string GetUser()
        {
            //return Request.HttpContext.User.Identity.Name;
            return FAKE_USER;
        }
    }
}
