using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PhotoCommentObject
    {
        [MaxLength(120)]
        public string PhotoCommentOriginal { get; set; }

        [MaxLength(120)]
        public string PhotoCommentEdited { get; set; }
    }
}
