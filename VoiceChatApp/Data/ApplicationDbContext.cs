using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using VoiceChatApp.Models;

namespace VoiceChatApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CallRecord> CallRecords { get; set; }
    }
}
