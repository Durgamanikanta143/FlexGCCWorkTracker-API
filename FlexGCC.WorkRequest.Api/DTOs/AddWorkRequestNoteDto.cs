
using System.ComponentModel.DataAnnotations;

namespace FlexGCC.WorkRequest.Api.DTOs
{
    public class AddWorkRequestNoteDto
    {
        [Required]
        [StringLength(1000)]
        public string Note { get; set; } = string.Empty;
    }
}
