﻿using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace QuartzHostedService
{
    public static class IServiceCollectionExtensions
    {
        public static IJobRegistrator UseQuartzHostedService(this IServiceCollection services)
        {
            return UseQuartzHostedService(services, null);
        }

        public static IJobRegistrator UseQuartzHostedService(this IServiceCollection services,
        Action<NameValueCollection> stdSchedulerFactoryOptions)
        {
            services.AddHostedService<QuartzHostedService>();

            services.AddTransient<ISchedulerFactory>(provider =>
            {
                var options = new NameValueCollection();
                stdSchedulerFactoryOptions?.Invoke(options);
                var result = new StdSchedulerFactory();
                if (options.Count > 0)
                    result.Initialize(options);
                return result;
            });
            services.AddTransient<IJobFactory, ServiceCollectionJobFactory>();

            return new JobRegistrator(services);
        }
    }
}