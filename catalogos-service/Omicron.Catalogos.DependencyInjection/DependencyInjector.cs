// <summary>
// <copyright file="DependencyInjector.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.DependencyInjection
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Omicron.Catalogos.DataAccess.DAO.Catalog;
    using Omicron.Catalogos.DataAccess.DAO.User;
    using Omicron.Catalogos.Entities.Context;
    using Omicron.Catalogos.Facade.Catalogs;
    using Omicron.Catalogos.Facade.Catalogs.Users;
    using Omicron.Catalogos.Services.Catalogs;
    using Omicron.Catalogos.Services.Mapping;
    using Omicron.Catalogos.Services.User;

    /// <summary>
    /// Class for DependencyInjector.
    /// </summary>
    public static class DependencyInjector
    {
        private static IServiceCollection Services { get; set; }

        /// <summary>
        /// Method to register Services.
        /// </summary>
        /// <param name="services">Service Collection.</param>
        /// <returns>Interface Service Collection.</returns>
        public static IServiceCollection RegisterServices(IServiceCollection services)
        {
            Services = services;
            Services.AddTransient<IUserFacade, UserFacade>();
            Services.AddTransient<IUsersService, UsersService>();
            Services.AddTransient<IUserDao, UserDao>();

            Services.AddTransient<ICatalogFacade, CatalogFacade>();
            Services.AddTransient<ICatalogService, CatalogService>();
            Services.AddTransient<ICatalogDao, CatalogDao>();

            Services.AddTransient<IDatabaseContext, DatabaseContext>();
            return Services;
        }

        /// <summary>
        /// Method to add Db Context.
        /// </summary>
        /// <param name="configuration">Configuration Options.</param>
        public static void AddDbContext(IConfiguration configuration)
        {
            Services.AddDbContextPool<DatabaseContext>(options => options.UseNpgsql(configuration.GetConnectionString(nameof(DatabaseContext))));
        }

        /// <summary>
        /// Add configuration Auto Mapper.
        /// </summary>
        public static void AddAutoMapper()
        {
            var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new AutoMapperProfile()); });
            Services.AddSingleton(mappingConfig.CreateMapper());
        }
    }
}
