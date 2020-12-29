using API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PhCommentCreatorDto
    {
        public string Comment { get; set; }

        public DateTime DateCreated { get; set; }
        // user who created the comment
        public UserDto CommentCreator { get; set; }
    }
}
