using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }
        public required string Url { get; set; }
        public bool IsMain { get; set; }
        public bool IsApproved { get; set; } = false;
        public string? PublicId { get; set; }
        public int AppUserId { get; set; }
        public AppUser? AppUser { get; set; } = null!; // Navigation property for the user who uploaded the photo.
    }
}