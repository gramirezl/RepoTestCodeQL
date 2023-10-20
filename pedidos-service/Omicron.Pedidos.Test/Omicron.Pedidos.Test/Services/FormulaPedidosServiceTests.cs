﻿// <summary>
// <copyright file="FormulaPedidosServiceTests.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Context;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Pedidos;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class FormulaPedidosServiceTests : BaseTest
    {
        private IFormulaPedidosService formulaPedidosService;

        private IPedidosDao pedidosDao;

        private DatabaseContext context;

        private string userId = "abc";

        /// <summary>
        /// Get component lists.
        /// </summary>
        /// <returns>the user.</returns>
        public List<CustomComponentListModel> GetCustomListForTests()
        {
            return new List<CustomComponentListModel>
            {
                new CustomComponentListModel { Id = 1, Name = "001 PRES 20 ML USER IDENTIFIER I", ProductId = "001", CreationUserId = this.userId },
                new CustomComponentListModel { Id = 2, Name = "001 PRES 20 ML USER IDENTIFIER III", ProductId = "001", CreationUserId = this.userId },
                new CustomComponentListModel { Id = 3, Name = "002 PRES 20 ML USER IDENTIFIER I", ProductId = "002", CreationUserId = this.userId },
            };
        }

        /// <summary>
        /// Get component lists.
        /// </summary>
        /// <returns>the user.</returns>
        public List<ComponentCustomComponentListModel> GetComponentsCustomListsForTests()
        {
            return new List<ComponentCustomComponentListModel>
            {
                new ComponentCustomComponentListModel { Id = 1, CustomListId = 1, ProductId = "003", Description = "COMP 003", BaseQuantity = 10 },
                new ComponentCustomComponentListModel { Id = 2, CustomListId = 1, ProductId = "004", Description = "COMP 004", BaseQuantity = 11.123456M },
                new ComponentCustomComponentListModel { Id = 3, CustomListId = 2, ProductId = "005", Description = "COMP 005", BaseQuantity = 11 },
                new ComponentCustomComponentListModel { Id = 4, CustomListId = 3, ProductId = "006", Description = "COMP 006", BaseQuantity = 11 },
            };
        }

        /// <summary>
        /// The set up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            var options = CreateNewContextOptions("formulaPedidosServiceDb");
            this.context = new DatabaseContext(options);
            this.context.CustomComponentLists.AddRange(this.GetCustomListForTests());
            this.context.ComponentsCustomComponentLists.AddRange(this.GetComponentsCustomListsForTests());
            this.context.SaveChanges();
            this.pedidosDao = new PedidosDao(this.context);
            this.formulaPedidosService = new FormulaPedidosService(this.pedidosDao);
        }

        /// <summary>
        /// Create a custom list.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task CreateCustomComponentList_InserCorrectly_SuccessResults()
        {
            // arrange
            var customList = this.GetComponentList();

            // act
            var response = await this.formulaPedidosService.CreateCustomComponentList(this.userId, customList);

            // assert
            Assert.IsTrue(this.CheckAction(response, true, 4, 6));
            Assert.IsTrue(response.Code == 200);
            Assert.IsInstanceOf<int>(response.Response);
            Assert.IsNotNull(response.Response);
            Assert.IsNull(response.ExceptionMessage);
            Assert.IsNull(response.Comments);
        }

        /// <summary>
        /// Create a custom list.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task CreateCustomComponentList_AlreadyExists_FailedResults()
        {
            // arrange
            var customList = this.GetComponentList();
            customList.Name = "001 PRES 20 ML USER IDENTIFIER I";

            // act
            var response = await this.formulaPedidosService.CreateCustomComponentList(this.userId, customList);

            // assert
            Assert.IsTrue(this.CheckAction(response, true, 3, 4));
            Assert.Zero(int.Parse(response.Response.ToString()));
        }

        /// <summary>
        /// Get related custom list to product code.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task GetCustomComponentListByProductId_Exists_SuccessResults()
        {
            // arrange
            var customList = this.GetComponentList();

            // act
            var response = await this.formulaPedidosService.GetCustomComponentListByProductId(customList.ProductId);

            // assert
            var listsInResponse = (List<CustomComponentListModel>)response.Response;
            Assert.IsTrue(response.Success);
            Assert.AreEqual(listsInResponse.Count, 2);
            Assert.AreEqual(listsInResponse[0].Components.Count, 2);
            Assert.AreEqual(listsInResponse[1].Components.Count, 1);
            Assert.IsInstanceOf<List<CustomComponentListModel>>(response.Response);
            Assert.IsNotNull(response.Response);
            Assert.IsNull(response.ExceptionMessage);
            Assert.IsNull(response.Comments);
        }

        /// <summary>
        /// Delete a custom list.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task DeleteCustomComponentList_DeleteCorrectly_SuccessResults()
        {
            // arrange
            var dic = new Dictionary<string, string>
            {
                { ServiceConstants.Name, "001 PRES 20 ML USER IDENTIFIER I" },
                { ServiceConstants.ProductId, "001" },
            };

            // act
            var response = await this.formulaPedidosService.DeleteCustomComponentList(dic);

            // assert
            Assert.IsTrue(this.CheckAction(response, true, 2, 2));
        }

        /// <summary>
        /// Delete a custom list.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task DeleteCustomComponentList_notExist_FailedResults()
        {
            // arrange
            var dic = new Dictionary<string, string>
            {
                { ServiceConstants.Name, "001 PRES 20 ML USER IDENTIFIER IIIIII" },
                { ServiceConstants.ProductId, "001" },
            };

            // act
            var response = await this.formulaPedidosService.DeleteCustomComponentList(dic);

            // assert
            Assert.IsTrue(this.CheckAction(response, false, 3, 4));
            Assert.Zero(int.Parse(response.Response.ToString()));
        }

        /// <summary>
        /// Get component list for test.
        /// </summary>
        /// <returns>Component list.</returns>
        private CustomComponentListModel GetComponentList()
        {
            return new CustomComponentListModel
            {
                Name = "001 PRES 20 ML USER IDENTIFIER II",
                ProductId = "001",
                Components = new List<ComponentCustomComponentListModel>()
                {
                    new ComponentCustomComponentListModel
                    {
                        ProductId = "003",
                        Description = "COMP 003",
                        BaseQuantity = 10.123456M,
                    },
                    new ComponentCustomComponentListModel
                    {
                        ProductId = "004",
                        Description = "COMP 004",
                        BaseQuantity = 11,
                    },
                },
            };
        }

        /// <summary>
        /// Check response results.
        /// </summary>
        /// <param name="result">Result.</param>
        /// <param name="success">Expected success.</param>
        /// <param name="numberCustomFormulas">Expected number of formulas.</param>
        /// <param name="numberOfComponents">Expected number of components.</param>
        /// <returns>Validation flag.</returns>
        private bool CheckAction(ResultModel result, bool success, int numberCustomFormulas, int numberOfComponents)
        {
            return result.Success.Equals(success) &&
                    this.context.CustomComponentLists.Count().Equals(numberCustomFormulas) &&
                    this.context.ComponentsCustomComponentLists.Count().Equals(numberOfComponents);
        }
    }
}
