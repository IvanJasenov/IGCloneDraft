﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class PhtoCommentCreation
    {
        [MaxLength(120)]
        public string PhotoComment { get; set; }
    }
}