using FlexGCC.WorkRequest.Api.Models;
using Microsoft.EntityFrameworkCore;
using WorkRequestModel = FlexGCC.WorkRequest.Api.Models.WorkRequest;

namespace FlexGCC.WorkRequest.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<WorkRequestModel> WorkRequests  => Set<WorkRequestModel>();
        public DbSet<WorkRequestNote> WorkRequestNotes  => Set<WorkRequestNote>();
    }
}