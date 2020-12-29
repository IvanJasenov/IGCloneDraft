using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class UserLike
    {
        // Liker, the user that is liking another user
        public AppUser SourceUser { get; set; }
        public int SourceUserId { get; set; }

        // Likee, to whom the like is send
        public AppUser LikedUser { get; set; }
        public int LikedUserId { get; set; }

        // I added this property
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
