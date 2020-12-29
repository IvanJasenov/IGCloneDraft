using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class PhotoLikes
    {
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public int PhotoId { get; set; }
        public Photo Photo { get; set; }

        public DateTime DateCreated { get; set; } =  DateTime.Now;
    }
}
