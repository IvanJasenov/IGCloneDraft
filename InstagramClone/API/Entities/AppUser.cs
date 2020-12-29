using API.Extensions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public  DateTime DateOfBirth { get; set; }

        public string KnownAs { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public DateTime LastActive { get; set; } = DateTime.Now;

        public string Gender { get; set; }

        public string Introduction { get; set; }

        public string LookingFor { get; set; }
        
        public string Interests { get; set; }
        
        public string City { get; set; }

        public string Country { get; set; }

        // comment this when seeding data
        // uncommnet when updating user
        ////// https://stackoverflow.com/a/59200054/2989167
        [JsonIgnore] // dont map Photos from AppUser class, prevents depth problem
        //public ICollection<Photo> Photos { get; set; }
        public List<Photo> Photos { get; set; }

        [JsonIgnore]
        public List<PhotoComment> PhotoComments { get; set; }
        
        [JsonIgnore]
        public List<PhotoLikes> PhotoLikes { get; set; }

        // will be mapped to Age in MemeberDto by AutoMapper convenction
        public int GetAge()
        {
            return DateOfBirth.CalculateAge();
        }

        // add two collections for the likes
        public ICollection<UserLike> LikedByUsers { get; set; }

        public ICollection<UserLike> LikedUsers { get; set; }

        #region Messages
        public ICollection<Message> MessagesSend { get; set; }

        public ICollection<Message> MessagesRecieved { get; set; }
        #endregion

        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
