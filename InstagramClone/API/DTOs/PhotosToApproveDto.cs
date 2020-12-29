using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class PhotosToApproveDto
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public bool IsApproved { get; set; }

        public int AppUserId { get; set; }

        public string Username { get; set; }

    }
}
