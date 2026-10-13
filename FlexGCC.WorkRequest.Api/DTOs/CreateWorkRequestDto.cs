
using System.ComponentModel.DataAnnotations;

namespace FlexGCC.WorkRequest.Api.DTOs
{
    public class CreateWorkRequestDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ClientName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [RegularExpression("^(Low|Medium|High)$")]
        public string Priority { get; set; } = "Medium";

        [Required]
        [RegularExpression("^(New|InProgress|Blocked|Completed)$")]
        public string Status { get; set; } = "New";

        public DateTime DueDate { get; set; }
    }
}
