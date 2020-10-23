using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System;

namespace ConsulUtility
{
    public static class ConsulExtention
    {
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime, ConsulOption consulOption)
        {
            var consulClient = new ConsulClient(c => 
            {
                c.Address = new Uri(consulOption.Address); 
            });

            var registration = new AgentServiceRegistration()
            {
                ID = $"{Guid.NewGuid()}",
                Name = consulOption.ServiceName,
                Address = consulOption.ServiceIP,
                Port = consulOption.ServicePort,
                Tags = new[] { $"urlprefix-/{consulOption.ServiceName}" },
                Check = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    Interval = TimeSpan.FromSeconds(10),
                    HTTP = consulOption.ServiceHealthCheck,
                    Timeout = TimeSpan.FromSeconds(5)
                }
            };

            //register service
            consulClient.Agent.ServiceRegister(registration).Wait();

            //Deregister service when application stop
            lifetime.ApplicationStopped.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });

            return app;
        }
    }
}
