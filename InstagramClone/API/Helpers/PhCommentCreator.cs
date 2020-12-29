using API.DTOs;
using API.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PhCommentCreator
    {
        public string Comment { get; set; }

        public DateTime DateCreated { get; set; }
        // user who created the comment
        public AppUser AppUser { get; set; }
        //public UserDto AppUser { get; set; }

    }
}
