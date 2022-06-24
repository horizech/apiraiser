using System;
using System.Web;
using Microsoft.AspNetCore.Http;

using System.Collections.Generic;

using Apiraiser.Services;
using Apiraiser.Enums;
using Apiraiser.Models;

namespace Apiraiser.Models
{
    public class FileRowUploadRequest
    {
        public IFormFile File { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }

}