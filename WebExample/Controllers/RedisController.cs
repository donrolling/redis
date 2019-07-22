using System;
using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace WebExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : Controller
    {
        private readonly IAppCacheService _appCacheService;

        public RedisController(IAppCacheService appCacheService)
        {
            _appCacheService = appCacheService;
        }


        [HttpGet("{key}")]
        public ActionResult<string> Get(string key)
        {
            return _appCacheService.Get(key);
        }

        [HttpPut("{key}")]
        public ActionResult<bool> Put(string key, [FromBody] object value)
        {
            try
            {
                var str = JsonConvert.SerializeObject(value);
                _appCacheService.Set(key, str);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
    }
}