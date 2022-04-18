using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Audio.Models
{
    public class Audio
    {
        public string AudioPath { get; set; }

        public IFormFile File { get; set; }
    }
}
