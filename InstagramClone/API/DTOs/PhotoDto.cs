using API.Entities;
using System.Collections.Generic;

namespace API.DTOs
{
    public class PhotoDto
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public bool IsMain { get; set; }

        public bool IsApproved { get; set; }

        public List<PhotoCommentDto> PhotoCommentsDto { get; set; }
        public List<PhotoLikeDto> PhotoLikesDto { get; set; }

    }
}