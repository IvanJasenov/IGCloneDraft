using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class MessageParams : PaginationParms
    {
        public string Username { get; set; }
        // send the unread by default
        public string Container { get; set; } = "Unread";
    }
}
