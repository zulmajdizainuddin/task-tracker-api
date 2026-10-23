using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Api.DataTransferObjects
{
    public class ProjectRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
