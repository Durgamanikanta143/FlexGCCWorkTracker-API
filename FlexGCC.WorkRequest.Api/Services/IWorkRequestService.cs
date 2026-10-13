
using FlexGCC.WorkRequest.Api.DTOs;
using FlexGCC.WorkRequest.Api.Models;
using WorkRequestModel =
    FlexGCC.WorkRequest.Api.Models.WorkRequest;

namespace FlexGCC.WorkRequest.Api.Services
{
    public interface IWorkRequestService
    {
        Task<List<WorkRequestModel>> GetAllAsync();

        Task<WorkRequestModel> CreateAsync(WorkRequestModel request);
        Task<WorkRequestModel?> GetByIdAsync(int id);

        Task<WorkRequestModel?> UpdateStatusAsync(int id,string status);

        Task<List<WorkRequestModel>> SearchAsync(string? search,string? status);

        Task<WorkRequestNote?> AddNoteAsync(int workRequestId,string note);

        Task<List<WorkRequestNote>> GetNotesAsync( int workRequestId);
        Task<WorkRequestModel?> UpdateAsync(  int id,UpdateWorkRequestDto request);

    }
}
