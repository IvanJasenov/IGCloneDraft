using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class PhotoComment
    {
        public PhotoComment(string comment, int appUserId, int photoId)
        {
            Comment = comment;
            AppUserId = appUserId;
            PhotoId = photoId;
        }

        public int Id { get; set; }

        [Required]
        public string Comment { get; set; }
        // to which user photo belong
        public int AppUserId { get; set; }

        public AppUser AppUser { get; set; }
       
        public int PhotoId { get; set; }

        public Photo Photo { get; set; }

        // readonly prop, getter
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
