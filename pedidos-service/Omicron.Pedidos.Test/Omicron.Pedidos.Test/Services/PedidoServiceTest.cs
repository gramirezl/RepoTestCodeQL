﻿// <summary>
// <copyright file="PedidoServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Context;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Resources.Enums;
    using Omicron.Pedidos.Resources.Exceptions;
    using Omicron.Pedidos.Services.Broker;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Pedidos;
    using Omicron.Pedidos.Services.Redis;
    using Omicron.Pedidos.Services.Reporting;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapDiApi;
    using Omicron.Pedidos.Services.SapFile;
    using Omicron.Pedidos.Services.User;
    using Omicron.Pedidos.Services.Utils;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class PedidoServiceTest : BaseTest
    {
        private IPedidosService pedidosService;

        private IPedidosDao pedidosDao;

        private Mock<ISapAdapter> sapAdapter;

        private Mock<IUsersService> usersService;

        private DatabaseContext context;

        private Mock<IConfiguration> configuration;

        private Mock<IReportingService> reportingService;

        private Mock<IRedisService> redisService;

        private Mock<IKafkaConnector> kafkaConnector;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Temporal")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.UserOrderModel.AddRange(this.GetUserOrderModel());
            this.context.UserOrderSignatureModel.AddRange(this.GetSignature());
            this.context.SaveChanges();

            this.reportingService = new Mock<IReportingService>();
            this.sapAdapter = new Mock<ISapAdapter>();
            this.sapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelGetFabricacionModel()));

            this.sapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetFormulaDetalle()));

            var mockSaDiApi = new Mock<ISapDiApi>();
            mockSaDiApi
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            mockSaDiApi
                .Setup(x => x.GetSapDiApi(It.IsAny<string>()))
                .Returns(Task.FromResult(new ResultModel()));

            this.usersService = new Mock<IUsersService>();

            this.usersService
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel()));

            this.kafkaConnector = new Mock<IKafkaConnector>();
            this.kafkaConnector
                .Setup(m => m.PushMessage(It.IsAny<object>()))
                .Returns(Task.FromResult(true));

            var mockSapFile = new Mock<ISapFileService>();
            this.redisService = new Mock<IRedisService>();

            this.configuration = new Mock<IConfiguration>();
            this.configuration.SetupGet(x => x[It.Is<string>(s => s == "OmicronFilesAddress")]).Returns("http://localhost:5002/");

            this.pedidosDao = new PedidosDao(this.context);
            this.pedidosService = new PedidosService(this.sapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, this.usersService.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetUserOrderBySalesOrder()
        {
            // arrange
            var listIds = new List<int> { 1, 2, 3 };

            // act
            var response = await this.pedidosService.GetUserOrderBySalesOrder(listIds);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetUserOrderByFabOrder()
        {
            // arrange
            var listIds = new List<int> { 100, 101, 102 };

            // act
            var response = await this.pedidosService.GetUserOrderByFabOrder(listIds);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetFabOrderByUserID()
        {
            // arrange
            var id = "abc";

            var localSapAdapter = new Mock<ISapAdapter>();

            localSapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetListFormulaDetalle()));

            var mockSaDiApi = new Mock<ISapDiApi>();
            mockSaDiApi
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            var mockSapFile = new Mock<ISapFileService>();

            var pedidosServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, this.usersService.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var response = await pedidosServiceLocal.GetFabOrderByUserId(id);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetFabOrderByTcnicID()
        {
            // arrange
            var id = "tecnic";

            var localSapAdapter = new Mock<ISapAdapter>();

            localSapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetListFormulaDetalle()));

            var mockSaDiApi = new Mock<ISapDiApi>();
            mockSaDiApi
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            this.usersService = new Mock<IUsersService>();

            this.usersService
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel(true)));

            var mockSapFile = new Mock<ISapFileService>();

            var pedidosServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, this.usersService.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var response = await pedidosServiceLocal.GetFabOrderByUserId(id);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task GetUserOrdersByUserId()
        {
            // arrange
            var id = new List<string> { "abc" };

            // act
            var response = await this.pedidosService.GetUserOrdersByUserId(id);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <param name="iduser">User Id.</param>
        /// <returns>return nothing.</returns>
        [Test]
        [TestCase("abcquimico")]
        [TestCase("abcquimicocd")]
        [TestCase("tecnicoqfb")]
        [TestCase("tecnicoqfb2")]
        public async Task GetQfbOrdersByStatus(string iduser)
        {
            // arrange
            var status = "Asignado";

            // act
            var response = await this.pedidosService.GetQfbOrdersByStatus(status, iduser);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task UpdateComponents()
        {
            // arrange
            var components = new List<CompleteDetalleFormulaModel>
            {
                new CompleteDetalleFormulaModel { Available = 1, BaseQuantity = 1, Consumed = 1, Description = "Des", OrderFabId = 2, PendingQuantity = 1, ProductId = "Aspirina", RequiredQuantity = 1, Stock = 1, Unit = "Unit", Warehouse = "wh", WarehouseQuantity = 1 },
            };

            var asignar = new UpdateFormulaModel
            {
                Comments = "Comments",
                Components = components,
                FabOrderId = 1,
                FechaFin = DateTime.Now,
                PlannedQuantity = 1,
            };

            var mockSaDiApi = new Mock<ISapDiApi>();
            mockSaDiApi
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUpdateOrder()));

            var mockSapFile = new Mock<ISapFileService>();

            var pedidosServiceLocal = new PedidosService(this.sapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, this.usersService.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var response = await pedidosServiceLocal.UpdateComponents(asignar);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <param name="userRoleType">User role type.</param>
        /// <param name="isValidtecnic">Is valid tecnic.</param>
        /// <returns>return nothing.</returns>
        [Test]
        [TestCase(2, false)]
        [TestCase(2, true)]
        [TestCase(9, true)]
        public async Task UpdateUserOrderStatus(int userRoleType, bool isValidtecnic)
        {
            // arrange
            var components = new List<UpdateStatusOrderModel>
            {
                new UpdateStatusOrderModel { UserId = "abcc", OrderId = 100, Status = "Proceso", UserRoleType = userRoleType },
            };

            var mockUsers = new Mock<IUsersService>();
            var mockSapFile = new Mock<ISapFileService>();
            var mockSaDiApi = new Mock<ISapDiApi>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetQfbInfoDto(isValidtecnic)));

            var pedidosService = new PedidosService(this.sapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var response = await pedidosService.UpdateStatusOrder(components);

            // assert
            Assert.IsNotNull(response);
            Assert.IsNull(response.Comments);

            if (isValidtecnic)
            {
                Assert.IsNull(response.UserError);
                Assert.IsTrue(response.Success);
                Assert.AreEqual(200, response.Code);
                Assert.IsNotNull(response.Response);
            }
            else
            {
                Assert.IsNotNull(response.UserError);
                Assert.IsFalse(response.Success);
                Assert.AreEqual(400, response.Code);
                Assert.IsNull(response.Response);
            }
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <param name="userRoleType">User role type.</param>
        /// <param name="isValidtecnic">Is valid tecnic.</param>
        /// <returns>return nothing.</returns>
        [Test]
        [TestCase(2, false)]
        [TestCase(2, true)]
        [TestCase(9, true)]
        public async Task UpdateUserOrderStatusEntregado(int userRoleType, bool isValidtecnic)
        {
            // arrange
            var components = new List<UpdateStatusOrderModel>
            {
                new UpdateStatusOrderModel { UserId = "abcc", OrderId = 301, Status = "Entregado", UserRoleType = userRoleType },
            };

            var mockUsers = new Mock<IUsersService>();
            var mockSapFile = new Mock<ISapFileService>();
            var mockSaDiApi = new Mock<ISapDiApi>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetQfbInfoDto(isValidtecnic)));

            var pedidosService = new PedidosService(this.sapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var response = await pedidosService.UpdateStatusOrder(components);

            // assert
            Assert.IsNotNull(response);
            Assert.IsNull(response.Comments);

            if (isValidtecnic)
            {
                Assert.IsNull(response.UserError);
                Assert.IsTrue(response.Success);
                Assert.AreEqual(200, response.Code);
                Assert.IsNotNull(response.Response);
            }
            else
            {
                Assert.IsNotNull(response.UserError);
                Assert.IsFalse(response.Success);
                Assert.AreEqual(400, response.Code);
                Assert.IsNull(response.Response);
            }
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task UpdateUserOrderStatusEntregadoAndAddNotCompletSalesLogs()
        {
            // arrange
            var components = new List<UpdateStatusOrderModel>
            {
                new UpdateStatusOrderModel { UserId = "abcc", OrderId = 501, Status = "Entregado" },
            };

            // act
            var mockUsers = new Mock<IUsersService>();
            var mockSapFile = new Mock<ISapFileService>();
            var mockSaDiApi = new Mock<ISapDiApi>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetQfbInfoDto(true)));

            var pedidosService = new PedidosService(this.sapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var response = await pedidosService.UpdateStatusOrder(components);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task UpdateUserOrderStatusEntregadoAndAddCompletSalesLogs()
        {
            // arrange
            var components = new List<UpdateStatusOrderModel>
            {
                new UpdateStatusOrderModel { UserId = "abcc", OrderId = 501, Status = "Entregado" },
                new UpdateStatusOrderModel { UserId = "abcc", OrderId = 502, Status = "Entregado" },
            };

            // act
            var mockUsers = new Mock<IUsersService>();
            var mockSapFile = new Mock<ISapFileService>();
            var mockSaDiApi = new Mock<ISapDiApi>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetQfbInfoDto(true)));

            var pedidosService = new PedidosService(this.sapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var response = await pedidosService.UpdateStatusOrder(components);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task ConnectDiApi()
        {
            // act
            var response = await this.pedidosService.ConnectDiApi();

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// Update order signatures.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task UpdateOrderSignature()
        {
            // arrange
            var imageData = File.ReadAllText("SignatureBase64.txt");

            var signatures = new UpdateOrderSignatureModel
            {
                UserId = "abc",
                FabricationOrderId = 101,
                Signature = imageData,
            };

            // act
            var response = await this.pedidosService.UpdateOrderSignature(SignatureType.LOGISTICS, signatures);

            // assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// Update fabrication order comments.
        /// </summary>
        /// <param name="orderId">Order to update.</param>
        /// <returns>Nothing.</returns>
        [Test]
        [TestCase(103)]
        public async Task UpdateFabOrderComments(int orderId)
        {
            // arrange
            var userId = "abc";

            var orderToUpdate = new List<UpdateOrderCommentsModel>
            {
                new UpdateOrderCommentsModel { UserId = userId, OrderId = orderId, Comments = "Hello" },
            };

            // act
            var response = await this.pedidosService.UpdateFabOrderComments(orderToUpdate);

            // assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// Get order signatures.
        /// </summary>
        /// <returns>Nothing.</returns>
        [Test]
        public async Task GetOrderSignatures()
        {
            // arrange
            int productionOrderId = 102;

            // act
            var response = await this.pedidosService.GetOrderSignatures(productionOrderId);

            // assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task UpdateBatches()
        {
            // arrange
            var update = new AssignBatchModel
            {
                Action = "insert",
                AssignedQty = 10,
                BatchNumber = "ABC",
                ItemCode = "102",
                OrderId = 10,
            };

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            mockSaDiApiLocal
                .Setup(m => m.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultAssignBatch()));

            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();
            var mockSapFile = new Mock<ISapFileService>();
            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var response = await pedidoServiceLocal.UpdateBatches(new List<AssignBatchModel> { update });

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <param name="qfbSignature">Qfb signature.</param>
        /// <param name="technicalSignature">Technical Signature.</param>
        /// <returns>return nothing.</returns>
        [Test]
        [TestCase("QXhpdHkyMDIw", null)]
        [TestCase("QXhpdHkyMDIw", "QXhpdHkyMDIw")]
        public async Task FinishOrder(string qfbSignature, string technicalSignature)
        {
            // arrange
            var update = new FinishOrderModel
            {
                FabricationOrderId = new List<int> { 100 },
                TechnicalSignature = technicalSignature,
                QfbSignature = qfbSignature,
                UserId = "abc",
            };

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();

            localSapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetBatches()));

            localSapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()));

            var mockSapFile = new Mock<ISapFileService>();

            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var response = await pedidoServiceLocal.FinishOrder(update);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task FinishOrderNewSignature()
        {
            // arrange
            var update = new FinishOrderModel
            {
                FabricationOrderId = new List<int> { 101 },
                TechnicalSignature = "QXhpdHkyMDIw",
                QfbSignature = "QXhpdHkyMDIw",
                UserId = "abc",
            };

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();

            localSapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetBatches()));

            localSapAdapter
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()));

            var mockSapFile = new Mock<ISapFileService>();

            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var response = await pedidoServiceLocal.FinishOrder(update);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        [Test]
        public void CompletedBatchesError()
        {
            // arrange
            var orderId = 101;

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();

            localSapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetMissingBatches()));

            var mockSapFile = new Mock<ISapFileService>();
            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            Assert.ThrowsAsync<CustomServiceException>(async () => await pedidoServiceLocal.CompletedBatches(orderId));
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task CloseSalesOrders()
        {
            // arrange
            var salesOrders = new List<OrderIdModel>
            {
                new OrderIdModel { OrderId = 104, UserId = "abc", },
            };
            var mockContent = new Dictionary<int, string> { { 0, "Ok" } };
            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            var mockSapAdapter = new Mock<ISapAdapter>();

            var componentsInvalids = new List<CompleteDetalleFormulaModel>()
            {
                new CompleteDetalleFormulaModel { OrderFabId = 107, Consumed = 2, RequiredQuantity = 2 },
            };
            mockSapAdapter
                .SetupSequence(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GenerateResultModel(componentsInvalids)))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()))
                .Returns(Task.FromResult(this.GetRecipes()));

            var mockSapFile = new Mock<ISapFileService>();

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel()));

            var mockResult = new ResultModel();
            mockResult.Success = true;
            mockResult.Code = 200;
            mockResult.Response = JsonConvert.SerializeObject(mockContent);

            mockSaDiApiLocal
                .Setup(m => m.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(mockResult));

            var pedidoServiceLocal = new PedidosService(mockSapAdapter.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var response = await pedidoServiceLocal.CloseSalesOrders(salesOrders);

            // assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task RejectSalesOrders()
        {
            // arrange
            var salesOrders = new RejectOrdersModel();
            salesOrders.Comments = "comentatios";
            salesOrders.OrdersId = new List<int>
            {
                123,
                456,
            };

            var mockContent = new Dictionary<int, string> { { 0, "Ok" } };
            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            var mockSapAdapter = new Mock<ISapAdapter>();

            mockSapAdapter
                .SetupSequence(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()))
                .Returns(Task.FromResult(this.GetRecipes()));

            var mockSapFile = new Mock<ISapFileService>();

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel()));

            var mockResult = new ResultModel();
            mockResult.Success = true;
            mockResult.Code = 200;
            mockResult.Response = JsonConvert.SerializeObject(mockContent);

            mockSaDiApiLocal
                .Setup(m => m.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(mockResult));

            var pedidoServiceLocal = new PedidosService(mockSapAdapter.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var response = await pedidoServiceLocal.RejectSalesOrders(salesOrders);

            // assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task CloseSalesOrders_SalesOrderWithPreProductionOrders_FailedResult()
        {
            // arrange
            var salesOrders = new List<OrderIdModel>
            {
                new OrderIdModel { OrderId = 104, UserId = "abc", },
            };

            var mockContent = new Dictionary<int, string> { { 0, "Ok" } };
            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            var mockUsers = new Mock<IUsersService>();
            var mockSapAdapter = new Mock<ISapAdapter>();

            var mockResult = new ResultModel();
            mockResult.Success = true;
            mockResult.Code = 200;
            mockResult.Response = JsonConvert.SerializeObject(mockContent);

            mockSaDiApiLocal
                .Setup(m => m.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(mockResult));

            var result = this.GetResultModelCompleteDetailModel();
            var responseOrders = (List<OrderWithDetailModel>)result.Response;
            responseOrders[0].Detalle.Add(new CompleteDetailOrderModel { CodigoProducto = "Aspirina", DescripcionProducto = "dec", OrdenFabricacionId = 101, QtyPlanned = 1, QtyPlannedDetalle = 1, Status = null });
            responseOrders[0].Detalle.Add(new CompleteDetailOrderModel { CodigoProducto = "Aspirina", DescripcionProducto = "dec", OrdenFabricacionId = 102, QtyPlanned = 1, QtyPlannedDetalle = 1, Status = string.Empty });
            result.Response = responseOrders;

            var componentsInvalids = new List<CompleteDetalleFormulaModel>();
            mockSapAdapter
                .SetupSequence(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GenerateResultModel(componentsInvalids)))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()))
                .Returns(Task.FromResult(this.GetRecipes()));

            var mockSapFile = new Mock<ISapFileService>();

            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel()));

            var pedidoServiceLocal = new PedidosService(mockSapAdapter.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var response = await pedidoServiceLocal.CloseSalesOrders(salesOrders);

            // assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task CloseFabOrders()
        {
            // arrange
            var salesOrders = new List<CloseProductionOrderModel>
            {
                new CloseProductionOrderModel { OrderId = 107, UserId = "abc", },
            };
            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            var mockSapAdapter = new Mock<ISapAdapter>();

            var mockContent = new Dictionary<int, string> { { 0, "Ok" } };
            var mockResult = new ResultModel();
            mockResult.Success = true;
            mockResult.Code = 200;
            mockResult.Response = JsonConvert.SerializeObject(mockContent);

            mockSaDiApiLocal
                .Setup(m => m.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(mockResult));

            var result = this.GetResultModelCompleteDetailModel();
            var responseOrders = (List<OrderWithDetailModel>)result.Response;
            responseOrders[0].Detalle.Add(new CompleteDetailOrderModel { CodigoProducto = "Aspirina", DescripcionProducto = "dec", OrdenFabricacionId = 101, QtyPlanned = 1, QtyPlannedDetalle = 1, Status = null });
            responseOrders[0].Detalle.Add(new CompleteDetailOrderModel { CodigoProducto = "Aspirina", DescripcionProducto = "dec", OrdenFabricacionId = 102, QtyPlanned = 1, QtyPlannedDetalle = 1, Status = string.Empty });
            result.Response = responseOrders;

            var componentsInvalids = new List<CompleteDetalleFormulaModel>()
            {
                new CompleteDetalleFormulaModel { OrderFabId = 107, Consumed = 2, RequiredQuantity = 2 },
            };
            mockSapAdapter
                .SetupSequence(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(result))
                .Returns(Task.FromResult(this.GenerateResultModel(componentsInvalids)))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()))
                .Returns(Task.FromResult(this.GetRecipes()));

            var mockSapFile = new Mock<ISapFileService>();

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel()));

            var pedidoServiceLocal = new PedidosService(mockSapAdapter.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var response = await pedidoServiceLocal.CloseFabOrders(salesOrders);

            // assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task CreateIsolatedProductionOrder()
        {
            // arrange
            var order = new CreateIsolatedFabOrderModel { ProductCode = "EN-1234", UserId = "abc", };

            var mockContent = new KeyValuePair<string, string>("token", "Ok");
            var mockSapDiApiLocal = new Mock<ISapDiApi>();
            var mockUsers = new Mock<IUsersService>();
            var mockSapAdapter = new Mock<ISapAdapter>();

            var mockResultDiApi = new ResultModel();
            mockResultDiApi.Success = true;
            mockResultDiApi.Code = 200;
            mockResultDiApi.Response = JsonConvert.SerializeObject(mockContent);

            var mockResultSapAdapter = new ResultModel();
            mockResultSapAdapter.Success = true;
            mockResultSapAdapter.Code = 200;
            mockResultSapAdapter.Response = "12345";

            mockSapDiApiLocal
                .Setup(m => m.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(mockResultDiApi));

            mockSapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(mockResultSapAdapter));

            var mockSapFile = new Mock<ISapFileService>();
            var pedidoServiceLocal = new PedidosService(mockSapAdapter.Object, this.pedidosDao, mockSapDiApiLocal.Object, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var response = await pedidoServiceLocal.CreateIsolatedProductionOrder(order);

            // assert
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersByDocNum()
        {
            // arrange
            var dic = new Dictionary<string, string>
            {
                { ServiceConstants.DocNum, "100-100" },
            };

            // act
            var result = await this.pedidosService.GetFabOrders(dic);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersByQfb()
        {
            // arrange
            var dic = new Dictionary<string, string>
            {
                { ServiceConstants.Qfb, "abc" },
            };

            // act
            var result = await this.pedidosService.GetFabOrders(dic);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersByStatus()
        {
            // arrange
            var dic = new Dictionary<string, string>
            {
                { ServiceConstants.Status, "Asignado" },
            };

            // act
            var result = await this.pedidosService.GetFabOrders(dic);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersByDate()
        {
            var dates = new DateTime(2020, 08, 29).ToString("dd/MM/yyyy");
            var dateFinal = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.FechaFin, string.Format("{0}-{1}", dates, dateFinal) },
            };

            // act
            var result = await this.pedidosService.GetFabOrders(dicParams);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task GetFabOrdersAllFilters()
        {
            var dates = new DateTime(2020, 08, 29).ToString("dd/MM/yyyy");
            var dateFinal = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");
            var dicParams = new Dictionary<string, string>
            {
                { ServiceConstants.FechaFin, string.Format("{0}-{1}", dates, dateFinal) },
                { ServiceConstants.Status, "Asignado" },
                { ServiceConstants.Qfb, "abc" },
            };

            // act
            var result = await this.pedidosService.GetFabOrders(dicParams);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task CompletedBatches()
        {
            var orderId = 200;

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();

            localSapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetBatches()));

            var mockSapFile = new Mock<ISapFileService>();
            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var result = await pedidoServiceLocal.CompletedBatches(orderId);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task PrintOrders()
        {
            var orderId = new List<int> { 100 };

            var mockSapAdapter = new Mock<ISapAdapter>();
            mockSapAdapter
                .SetupSequence(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()))
                .Returns(Task.FromResult(this.GetRecipes()));

            var mockSapFile = new Mock<ISapFileService>();
            mockSapFile
                .Setup(m => m.PostSimple(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            var mockDiApi = new Mock<ISapDiApi>();

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel()));

            var pedidoServiceLocal = new PedidosService(mockSapAdapter.Object, this.pedidosDao, mockDiApi.Object, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var result = await pedidoServiceLocal.PrintOrders(orderId);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task PrintOrdersFabOrders()
        {
            var orderId = new List<int> { 100 };

            var mockSapAdapter = new Mock<ISapAdapter>();
            mockSapAdapter
                .SetupSequence(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetFabricacionOrderModel()))
                .Returns(Task.FromResult(this.GetRecipes()));

            var mockSapFile = new Mock<ISapFileService>();
            mockSapFile
                .Setup(m => m.PostSimple(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            var mockDiApi = new Mock<ISapDiApi>();

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUserModel()));

            // act
            var result = await SendToGeneratePdfUtils.CreateModelGeneratePdf(new List<int>(), orderId, mockSapAdapter.Object, this.pedidosDao, mockSapFile.Object, mockUsers.Object, true);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task UpdateSaleOrders()
        {
            var orderId = new UpdateOrderCommentsModel
            {
                OrderId = 100,
                Comments = "Comments",
            };

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();
            var mockSapFile = new Mock<ISapFileService>();
            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var result = await pedidoServiceLocal.UpdateSaleOrders(orderId);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task UpdateDesignerLabel()
        {
            var details = new List<UpdateDesignerLabelDetailModel>
            {
                new UpdateDesignerLabelDetailModel { OrderId = 100, Checked = true },
                new UpdateDesignerLabelDetailModel { OrderId = 200, Checked = true },
            };

            var orderId = new UpdateDesignerLabelModel
            {
                DesignerSignature = "aG9sYQ==",
                UserId = "abc",
                Details = details,
            };

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();
            var mockSapFile = new Mock<ISapFileService>();
            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var result = await pedidoServiceLocal.UpdateDesignerLabel(orderId);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task CreateSaleOrderPdf()
        {
            var details = new List<int> { 100 };

            var dictResponse = new Dictionary<string, string>
            {
                { "100", "Ok-C:\\algo" },
            };

            var response = new ResultModel
            {
                Code = 200,
                Success = true,
                Response = JsonConvert.SerializeObject(dictResponse),
            };

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();
            var mockSapFile = new Mock<ISapFileService>();
            mockSapFile
                .Setup(m => m.PostSimple(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(response));

            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var result = await pedidoServiceLocal.CreateSaleOrderPdf(details);

            // assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <returns>return nothing.</returns>
        [Test]
        public async Task SignOrdersByTecnic()
        {
            // arrange
            var update = new FinishOrderModel
            {
                FabricationOrderId = new List<int> { 100, 200 },
                TechnicalSignature = "QXhpdHkyMDIw",
                UserId = "abc",
            };

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();
            var mockSapFile = new Mock<ISapFileService>();

            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var response = await pedidoServiceLocal.SignOrdersByTecnic(update);

            // assert
            Assert.IsNotNull(response);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <param name="productionOrderId">Production Order Id.</param>
        /// <returns>return nothing.</returns>
        [Test]
        [TestCase("223740")]
        [TestCase("224212")]
        [TestCase("224211")]
        [TestCase("224159")]
        public async Task GetInvalidOrdersByMissingTecnicSign(string productionOrderId)
        {
            // arrange
            var productionOrderIds = new List<string>
            {
                productionOrderId,
            };

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            var mockUsers = new Mock<IUsersService>();
            var localSapAdapter = new Mock<ISapAdapter>();
            var mockSapFile = new Mock<ISapFileService>();

            var pedidoServiceLocal = new PedidosService(localSapAdapter.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, mockSapFile.Object, this.configuration.Object, this.reportingService.Object, this.redisService.Object, this.kafkaConnector.Object);

            // act
            var response = await pedidoServiceLocal.GetInvalidOrdersByMissingTecnicSign(productionOrderIds);

            // assert
            Assert.IsNotNull(response);
            Assert.IsNull(response.ExceptionMessage);
            Assert.IsNull(response.Comments);
            Assert.IsNull(response.UserError);
            Assert.IsTrue(response.Success);
            Assert.AreEqual(200, response.Code);

            if (productionOrderId.Equals("224212") || productionOrderId.Equals("224159"))
            {
                Assert.IsNotNull(response.Response);
            }
            else
            {
                Assert.AreEqual(new List<string>(), response.Response);
            }
        }
    }
}
