using System;
using Fasetto.Word.Web.Server.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Fasetto.Word.Web.Server.IoC
{
    public static class Ioc
    {
        //scoped instance of appdbcontext
        public static ApplicationDbContext ApplicationDbContext => IocContainer.Provider.GetService<ApplicationDbContext>();
    }
    public static class IocContainer
    {
        //app service provider
        public static IServiceProvider Provider { get; set; }
    }
}
