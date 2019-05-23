using System;
using Common.Interfaces;
using Common.IO;
using Common.Logging;
using Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Application;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Config;
using StackExchange.Redis.Extensions.Core.Configuration;
using Tests.Models;

//using Business.Interfaces;
//using Business.Service.EntityServices.Interfaces;

namespace Tests
{
    [TestClass]
    public class TestBase
    {
        public IOptions<AppSettings> AppSettings { get; private set; }
        public Microsoft.Extensions.Logging.ILogger Logger { get; private set; }
        public ILoggerFactory LoggerFactory { get; private set; }
        public TestContext TestContext { get; set; }
        public IServiceProvider ServiceProvider { get; private set; }

        public string TestName
        {
            get
            {
                return this.TestContext.TestName;
            }
        }

        public void Init(bool useStandardAppCacheService = true)
        {
            var services = PreInit(useStandardAppCacheService);
            Init(services);
        }

        public void Init(IServiceCollection services)
        {
            this.ServiceProvider = services.BuildServiceProvider();
            this.LoggerFactory = this.ServiceProvider.GetService<ILoggerFactory>();
            this.Logger = LogUtility.GetLogger(this.ServiceProvider, this.GetType());
            this.AppSettings = this.ServiceProvider.GetService<IOptions<AppSettings>>();
        }

        public IServiceCollection PreInit(bool useStandardAppCacheService = true)
        {
            return setup(useStandardAppCacheService);
        }

        private IServiceCollection setup(bool useStandardAppCacheService)
        {
            var pathToNLogConfig = FileUtility.GetFullPath_FromRelativePath<TestBase>("nlog.config");
            var pathToAppSettingsConfig = FileUtility.GetFullPath_FromRelativePath<TestBase>("appsettings.json");
            var provider = new PhysicalFileProvider(pathToNLogConfig.path);
            LogManager.Configuration = new XmlLoggingConfiguration(pathToNLogConfig.filePath);

            var config = new ConfigurationBuilder().AddJsonFile(pathToAppSettingsConfig.filePath).Build();
            var services = new ServiceCollection();
            services.AddLogging();
            services.Configure<AppSettings>(config.GetSection("AppSettings"));

            var redisConfiguration = config.GetSection("Redis").Get<RedisConfiguration>();

            var serviceProvider = services.BuildServiceProvider();
            var appSettings = serviceProvider.GetService<IOptions<AppSettings>>();

            services.AddSingleton<IFileProvider>(provider);

            //AppCache
            services.AddMemoryCache();

            //SessionCache
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
            });
            services.AddMvc(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            }).AddJsonOptions(options =>
                options.SerializerSettings.ContractResolver = new DefaultContractResolver()
            );
            services.AddSingleton<IHttpContextAccessor, FakeHttpContextAccessor>();

            if (useStandardAppCacheService)
            {
                services.AddTransient<IAppCacheService, AppCacheService>();
            }
            else
            {
                services.AddTransient<IAppCacheService, RedisCacheService>();
                services.AddDistributedRedisCache(option =>
                {
                    option.Configuration = appSettings.Value.RedisSettings.Configuration;
                    option.InstanceName = appSettings.Value.RedisSettings.InstanceName;
                });
            }

            return services;
        }
    }
}