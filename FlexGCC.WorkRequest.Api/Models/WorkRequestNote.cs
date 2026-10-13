using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlexGCC.WorkRequest.Api.Models
{
    [Table("WorkRequestNotes")]
    public class WorkRequestNote
    {
        public int Id { get; set; }

        public int WorkRequestId { get; set; }

        [Required]
        [StringLength(1000)]
        [Column("NoteText")]
        public string Note { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}