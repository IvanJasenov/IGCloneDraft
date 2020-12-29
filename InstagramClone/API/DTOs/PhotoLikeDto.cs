using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class PhotoLikeDto
    {
        public int PhotoId { get; set; }

        public int UserId { get; set; }

        public UserDto UserDto { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
