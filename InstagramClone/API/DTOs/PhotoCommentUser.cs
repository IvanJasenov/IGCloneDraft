using API.Entities;
using API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class PhotoCommentUser
    {
        
        public int Id { get; set; }

        public string Country { get; set; }

        public bool IsMain { get; set; }

        public string Url { get; set; }

        public string ImageDescription { get; set; }

        //public List<string> PhotoComments { get; set; }

        public List<PhCommentCreator> PhCommentCreators { get; set; }

        public UserDto UserDto { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
