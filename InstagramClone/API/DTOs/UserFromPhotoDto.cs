using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class UserFromPhotoDto
    {
        public string Username { get; set; }

        public string PhotoUrl { get; set; }

        public string KnownAs { get; set; }

        public string Gender { get; set; }
    }
}
