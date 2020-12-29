using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class PhotoCommentDto
    {
        public int PhotoId { get; set; }

        public int AppUserId { get; set; }

        public int CommentId { get; set; }

        public string PhotoComentatoUsername { get; set; }

        [MaxLength(120)]
        public string PhotoComment { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
