using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Entities
{
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public string ImageDescription { get; set; }

        public bool IsMain { get; set; }

        // for the Cloudinary 
        public string PublicId { get; set; }

        public bool IsApproved { get; set; } = false;

        // fully defining a relationship between AppUser and Photo and not relly on convenction by EF
        // this one is just for navigation purposes, circular reference
        public AppUser AppUser { get; set; }

        public int AppUserId { get; set; }
        //[JsonIgnore]
        public List<PhotoComment> PhotoComments { get; set; }

        public List<PhotoLikes> PhotoLikes { get; set; }

    }
}