using Microsoft.AspNetCore.Mvc;
using VoiceChatApp.Models;
using VoiceChatApp.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace VoiceChatApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CallLogApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CallLogApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        

        [HttpGet]
        public async Task<IActionResult> GetCallLogs()
        {
            var logs = await _context.CallRecords
                .OrderByDescending(c => c.CallDate)
                .ThenByDescending(c => c.StartTime)
                .Select(c => new
                {
                    user = c.User,
                    date = c.CallDate,
                    startTime = c.StartTime,
                    endTime = c.EndTime,
                    duration = c.Duration.HasValue ? $"{c.Duration.Value.TotalMinutes:F1} دقائق" : "جارية"
                })
                .ToListAsync();

            return Ok(logs);
        }



    }
}
