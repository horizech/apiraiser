using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Configuration;

namespace Apiraiser.Services
{
    public class BaseService
    {
        protected readonly IConfiguration _configuration;

        public BaseService(IConfiguration configuration)
        {
            _configuration = configuration;
            ServiceManager.Instance.RegisterService(this);
        }
    }
}