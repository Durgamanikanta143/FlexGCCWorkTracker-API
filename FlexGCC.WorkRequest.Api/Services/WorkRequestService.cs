
using FlexGCC.WorkRequest.Api.Data;
using FlexGCC.WorkRequest.Api.DTOs;
using FlexGCC.WorkRequest.Api.Models;
using Microsoft.EntityFrameworkCore;
using WorkRequestModel = FlexGCC.WorkRequest.Api.Models.WorkRequest;

namespace FlexGCC.WorkRequest.Api.Services
{
    public class WorkRequestService : IWorkRequestService
    {
        private readonly AppDbContext _context;

        public WorkRequestService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<WorkRequestModel>> GetAllAsync()
        {
            return await _context.WorkRequests.AsNoTracking()
                .OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task<WorkRequestModel> CreateAsync( WorkRequestModel request)
        {
            request.Id = 0;
            request.CreatedAt = DateTime.UtcNow;
            request.UpdatedAt = null;

            _context.WorkRequests.Add(request);

            await _context.SaveChangesAsync();

            return request;
        }

        public async Task<WorkRequestModel?> GetByIdAsync(int id)
        {
            return await _context.WorkRequests.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<WorkRequestModel?> UpdateStatusAsync(int id,string status)
        {
            var request = await _context.WorkRequests.FirstOrDefaultAsync(x => x.Id == id);

            if (request == null)
            {
                return null;
            }

            request.Status = status;
            request.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return request;
        }



        public async Task<List<WorkRequestModel>> SearchAsync(  string? search,   string? status)
        {
            var query = _context.WorkRequests.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>  x.Title.Contains(search) || x.ClientName.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.Status == status);
            }

            return await query  .OrderByDescending(x => x.CreatedAt).ToListAsync();
        }



        public async Task<WorkRequestNote?> AddNoteAsync(int workRequestId,  string note)
        {
            var exists = await _context.WorkRequests.AnyAsync(x => x.Id == workRequestId);

            if (!exists)
            {
                return null;
            }

            var workRequestNote = new WorkRequestNote
            {
                WorkRequestId = workRequestId,
                Note = note.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.WorkRequestNotes.Add(workRequestNote);
            await _context.SaveChangesAsync();

            return workRequestNote;
        }

        public async Task<List<WorkRequestNote>> GetNotesAsync( int workRequestId)
        {
            return await _context.WorkRequestNotes.AsNoTracking()
                .Where(x => x.WorkRequestId == workRequestId)
                .OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task<WorkRequestModel?> UpdateAsync(  int id,UpdateWorkRequestDto request)
        {
            var item = await _context.WorkRequests.FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
                return null;

            var validPriorities = new[] { "Low", "Medium", "High" };
            var validStatuses = new[]  { "New", "InProgress", "Blocked", "Completed"  };

            if (!validPriorities.Contains(request.Priority))
                throw new ArgumentException("Invalid priority.");

            if (!validStatuses.Contains(request.Status))
                throw new ArgumentException("Invalid status.");

            item.Title = request.Title.Trim();
            item.ClientName = request.ClientName.Trim();
            item.Description = request.Description;
            item.Priority = request.Priority;
            item.Status = request.Status;
            item.DueDate = request.DueDate;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return item;
        }

    }
}
