using Microsoft.EntityFrameworkCore;

namespace MeetingApp.Data
{
    public class MeetingAppContext : DbContext
    {
        public MeetingAppContext(DbContextOptions<MeetingAppContext> options)
            : base(options)
        {
        }

        public DbSet<MeetingApp.Models.Status> Status { get; set; } = default!;

        public DbSet<MeetingApp.Models.MeetingType>? MeetingType { get; set; }
    }
}
