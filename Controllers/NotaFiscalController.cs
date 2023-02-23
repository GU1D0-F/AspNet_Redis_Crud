using Microsoft.AspNetCore.Mvc;
using Redis.OM.Searching;
using Redis.OM.Skeleton.Models;
using System.Collections;

namespace Redis.OM.Skeleton.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotaFiscalController : ControllerBase
    {
        private readonly RedisCollection<SyncNfe> _syncNfe;
        private readonly RedisConnectionProvider _provider;
        public NotaFiscalController(RedisConnectionProvider provider)
        {
            _provider = provider;
            _syncNfe = (RedisCollection<SyncNfe>)provider.RedisCollection<SyncNfe>();
        }

        [HttpGet("GetAll")]
        public IList<SyncNfe> GetAll() =>
            _syncNfe.AsEnumerable().OrderBy(x => x.TimeStamp).ToList();
        
        [HttpGet("GetAllDescending")]
        public IList<SyncNfe> GetAllDescending() =>
            _syncNfe.AsEnumerable().OrderByDescending(x => x.TimeStamp).ToList();




        [HttpGet("GetByKeyNfe")]
        public async Task<SyncNfe> GetByKey(string key) =>
            await _syncNfe.Where(x => x.ChaveNfe == key).FirstOrDefaultAsync();

        [HttpGet("GetMoreOlder")]
        public SyncNfe GetMoreOlder(string chaveNfe) =>
            _syncNfe.AsEnumerable().OrderBy(x => x.TimeStamp).FirstOrDefault();
        

        [HttpGet("TimeStamp")]
        public IActionResult TimeStampTest()
        {
            DateTimeOffset now = DateTimeOffset.Now;
            var unixTimeSeconds = now.ToUnixTimeSeconds();
            var unixMiliSeconds = now.ToUnixTimeMilliseconds();

            DateTime dateTime = DateTime.Now;
            return Ok(new
            {
                DateTimeOffset = now,
                UniversalTime = now.ToUniversalTime(),
                UnixTimeSeconds = unixTimeSeconds,
                UnixMiliSeconds = unixMiliSeconds,
                ConverTimeSeconds = DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds),
                ConvertMiliSeconds = DateTimeOffset.FromUnixTimeMilliseconds(unixMiliSeconds),
                DateTimeMiliSeconds = dateTime.Millisecond
            });
        }

        [HttpGet("TimeStampConverter")]
        public IActionResult TimeStampConverter(long miliSecondsTimeStamp) =>
            Ok(new { Date = DateTimeOffset.FromUnixTimeMilliseconds(miliSecondsTimeStamp) });

        [HttpPost]
        public async Task<SyncNfe> AddNfe(SyncNfeModel nfe)
        {
            SyncNfe syncNfe = new()
            {
                ChaveNfe = nfe.ChaveNfe,
                TimeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };
            await _syncNfe.InsertAsync(syncNfe);
            return syncNfe;
        }

    }
}
