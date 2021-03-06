using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using Xunit;

namespace QuartzHostedService.Test
{
    public class IServiceCollectionExtensionsUnitTest
    {
        [Fact(DisplayName = "Зарегистрировали HostedService")]
        public void IServiceCollectionExtensions_Register_HostedService()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            IServiceCollectionExtensions.UseQuartzHostedService(serviceCollection, null);

            var testClass = serviceCollection.BuildServiceProvider().GetRequiredService<IHostedService>();
            testClass.Should()
                .NotBeNull()
                .And.BeOfType<QuartzHostedService>();
        }

        [Fact(DisplayName = "Зарегистрировали IJobFactory (для DI в Job'ах)")]
        public void IServiceCollectionExtensions_Register_IJobFactory()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            IServiceCollectionExtensions.UseQuartzHostedService(serviceCollection, null);

            var testClass = serviceCollection.BuildServiceProvider().GetRequiredService<IJobFactory>();
            testClass.Should()
                .NotBeNull()
                .And.BeOfType<ServiceCollectionJobFactory>();
        }

        [Fact(DisplayName = "Зарегистрировали ISchedulerFactory (не передали параметры для инициализации)")]
        public void IServiceCollectionExtensions_Register_ISchedulerFactory()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            IServiceCollectionExtensions.UseQuartzHostedService(serviceCollection, null);

            var testClass = serviceCollection.BuildServiceProvider().GetRequiredService<ISchedulerFactory>();
            testClass.Should()
                .NotBeNull()
                .And.BeOfType<StdSchedulerFactory>();
        }

        [Fact(DisplayName = "Зарегистрировали ISchedulerFactory (передали параметры для инициализации)")]
        public void IServiceCollectionExtensions_Register_ISchedulerFactory_WithParams()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            IServiceCollectionExtensions.UseQuartzHostedService(serviceCollection, options => { options.Add("quartz.threadPool.threadCount", "1"); });

            // TODO: Проверить что параметры передались в конструктор
            var testClass = serviceCollection.BuildServiceProvider().GetRequiredService<ISchedulerFactory>();
            testClass.Should()
                .NotBeNull()
                .And.BeOfType<StdSchedulerFactory>();
        }

        [Fact(DisplayName = "После регистрации зависимосте возвращается IJobRegistrator")]
        public void IServiceCollectionExtensions_Return_IJobRegistrator()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            var result = IServiceCollectionExtensions.UseQuartzHostedService(serviceCollection, null);

            result.Should()
                .NotBeNull()
                .And.BeAssignableTo<IJobRegistrator>()
                .Subject.Services.Should().Equal(serviceCollection);
        }
    }
}
