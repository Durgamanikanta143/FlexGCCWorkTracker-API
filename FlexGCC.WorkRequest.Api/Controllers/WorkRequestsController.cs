
using FlexGCC.WorkRequest.Api.DTOs;
using FlexGCC.WorkRequest.Api.Services;
using Microsoft.AspNetCore.Mvc;
using WorkRequestModel =
    FlexGCC.WorkRequest.Api.Models.WorkRequest;

namespace FlexGCC.WorkRequest.Api.Controllers
{
    [Route("api/work-requests")]
    [ApiController]
    public class WorkRequestsController : ControllerBase
    {
        private readonly IWorkRequestService _service;

        public WorkRequestsController( IWorkRequestService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<WorkRequestModel>>> GetAll()
        {
            var requests = await _service.GetAllAsync();

            return Ok(requests);
        }

        [HttpPost]
        public async Task<ActionResult<WorkRequestModel>> Create(
            WorkRequestModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) ||
                string.IsNullOrWhiteSpace(request.ClientName))
            {
                return BadRequest(new
                {
                    message = "Title and Client Name are required."
                });
            }

            var validPriorities = new[] { "Low", "Medium", "High" };

            var validStatuses = new[]
            {
                "New", "InProgress", "Blocked", "Completed"
            };

            if (!validPriorities.Contains(request.Priority) ||
                !validStatuses.Contains(request.Status))
            {
                return BadRequest(new
                {
                    message = "Invalid priority or status."
                });
            }

            var created = await _service.CreateAsync(request);

            return StatusCode(
                StatusCodes.Status201Created,
                created);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<WorkRequestModel>> GetById(int id)
        {
            var request = await _service.GetByIdAsync(id);

            if (request == null)
            {
                return NotFound(new
                {
                    message = $"Work request {id} not found."
                });
            }

            return Ok(request);
        }


        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus( int id,UpdateStatusDto dto)
        {
            var validStatuses = new[] { "New", "InProgress", "Blocked", "Completed" };

            if (!validStatuses.Contains(dto.Status))
            {
                return BadRequest(new
                {
                    message = "Invalid status."
                });
            }

            var updated = await _service.UpdateStatusAsync(id, dto.Status);

            if (updated == null)
            {
                return NotFound(new
                {
                    message = $"Work request {id} not found."
                });
            }

            return Ok(updated);
        }


        public class UpdateStatusDto
        {
            public string Status { get; set; } = string.Empty;
        }


        [HttpGet("search")]
        public async Task<ActionResult<List<WorkRequestModel>>> Search( [FromQuery] string? search,[FromQuery] string? status)
        {
            var validStatuses = new[]{ "New", "InProgress", "Blocked", "Completed"  };

            if (!string.IsNullOrWhiteSpace(status) &&!validStatuses.Contains(status))
            {
                return BadRequest(new
                {
                    message = "Invalid status."
                });
            }

            var requests = await _service.SearchAsync(search, status);

            return Ok(requests);
        }

        [HttpPost("{workRequestId:int}/notes")]
        public async Task<IActionResult> AddNote(int workRequestId,[FromBody] AddWorkRequestNoteDto request)
        {
        if (workRequestId <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid work request ID."
            });
        }

        if (request == null ||
            string.IsNullOrWhiteSpace(request.Note))
        {
            return BadRequest(new
            {
                message = "Note is required."
            });
        }

        var note = await _service.AddNoteAsync( workRequestId, request.Note);

        if (note == null)
        {
            return NotFound(new
            {
                message = "Work request was not found."
            });
        }

        return CreatedAtAction(  nameof(AddNote), new { workRequestId },
            note);
        }

        [HttpGet("{workRequestId:int}/notes")]
        public async Task<IActionResult> GetNotes(int workRequestId)
        {
            var notes = await _service.GetNotesAsync(workRequestId);
            return Ok(notes);
        }
        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update( int id, [FromBody] UpdateWorkRequestDto request)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid request ID." });

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var updated = await _service.UpdateAsync(id, request);

            if (updated == null)
                return NotFound(new { message = "Work request not found." });

            return Ok(updated);
        }
    }
}
