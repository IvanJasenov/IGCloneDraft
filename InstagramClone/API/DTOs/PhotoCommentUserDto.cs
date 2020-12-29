using API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class PhotoCommentUserDto
    {
        public int Id { get; set; }

        public bool IsMain { get; set; }

        public string Url { get; set; }

        public UserDto PhotoOwner { get; set; }

        public DateTime DateCreated { get; set; }

        //public List<string> PhotoComments { get; set; }

        public List<PhCommentCreatorDto> PhCommentCreatorDtos { get; set; }

    }
}
