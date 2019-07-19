using Microsoft.AspNetCore.Mvc;

namespace WebExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : Controller
    {
        [HttpGet("{key}")]
        public ActionResult<string> Get(string key)
        {
            return "";
        }

        [HttpPut("{key}")]
        public ActionResult<bool> Put(string key, [FromBody] object value)
        {
            return true;
        }
    }
}