using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class LikesParams : PaginationParms
    {
        // made it optional
        public int? UserId { get; set; }

        public string Predicates { get; set; }
    }
}
