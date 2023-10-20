﻿// <summary>
// <copyright file="ProductivityServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using NUnit.Framework;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Context;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Pedidos;
    using Omicron.Pedidos.Services.User;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class ProductivityServiceTest : BaseTest
    {
        private IProductivityService pedidosService;

        private IPedidosDao pedidosDao;

        private Mock<IUsersService> usersService;

        private DatabaseContext context;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalProductivity")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.UserOrderModel.AddRange(this.GetUserOrderModel());
            this.context.UserOrderSignatureModel.AddRange(this.GetSignature());
            this.context.SaveChanges();

            this.usersService = new Mock<IUsersService>();

            this.usersService
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel()));

            this.usersService
                .Setup(m => m.SimpleGetUsers(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel()));

            this.pedidosDao = new PedidosDao(this.context);
            this.pedidosService = new ProductivityService(this.pedidosDao, this.usersService.Object);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetProductivityData()
        {
            // arrange
            var assign = new Dictionary<string, string>
            {
                { ServiceConstants.FechaFin, "01/07/2020-30/08/2020" },
            };

            var pedidosServiceLocal = new ProductivityService(this.pedidosDao, this.usersService.Object);

            // act
            var response = await pedidosServiceLocal.GetProductivityData(assign);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetWorkLoad()
        {
            // arrange
            var assign = new Dictionary<string, string>
            {
                { ServiceConstants.FechaFin, "01/07/2020-30/08/2020" },
            };

            var pedidosServiceLocal = new ProductivityService(this.pedidosDao, this.usersService.Object);

            // act
            var response = await pedidosServiceLocal.GetWorkLoad(assign);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetWorkLoadByUser()
        {
            // arrange
            var assign = new Dictionary<string, string>
            {
                { ServiceConstants.FechaFin, "01/07/2020-30/08/2020" },
                { ServiceConstants.Qfb, "abc" },
            };

            var pedidosServiceLocal = new ProductivityService(this.pedidosDao, this.usersService.Object);

            // act
            var response = await pedidosServiceLocal.GetWorkLoad(assign);

            // assert
            Assert.IsNotNull(response);
        }
    }
}
