using System;
using System.Collections.Generic;
using System.Linq;

namespace Apiraiser.Services
{
    public class ServiceManager
    {

        private Dictionary<Type, BaseService> services { get; set; }
        private static ServiceManager instance;
        private static readonly object instanceLock = new object();

        private ServiceManager()
        {
            services = new Dictionary<Type, BaseService>();
        }

        public static ServiceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                        {
                            instance = new ServiceManager();
                        }
                    }
                }
                return instance;
            }
        }

        public T GetService<T>() where T : BaseService
        {
            return (T)services[typeof(T)];
        }

        public void RegisterService<T>(T service) where T : BaseService
        {
            services[service.GetType()] = service;
        }
    }

}