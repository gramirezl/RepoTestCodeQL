﻿// <summary>
// <copyright file="AssignOrderServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using NUnit.Framework;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Context;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Resources.Exceptions;
    using Omicron.Pedidos.Services.Broker;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Pedidos;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapDiApi;
    using Omicron.Pedidos.Services.User;

    /// <summary>
    /// class for the test.
    /// </summary>
    [TestFixture]
    public class AssignOrderServiceTest : BaseTest
    {
        private IAssignPedidosService pedidosService;

        private IPedidosDao pedidosDao;

        private Mock<ISapAdapter> sapAdapter;

        private Mock<IUsersService> usersService;

        private Mock<IKafkaConnector> kafkaConnector;

        private DatabaseContext context;

        /// <summary>
        /// The set up.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Temporal2")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.UserOrderModel.AddRange(this.GetUserOrderModel());
            this.context.UserOrderSignatureModel.AddRange(this.GetSignature());
            this.context.SaveChanges();

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

            this.pedidosDao = new PedidosDao(this.context);
            this.pedidosService = new AssignPedidosService(this.sapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, this.usersService.Object, this.kafkaConnector.Object);
        }

        /// <summary>
        /// the processs.
        /// </summary>
        /// <param name="isValidtecnic">Is valid tecnic.</param>
        /// <returns>return nothing.</returns>
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task AssignOrderPedido(bool isValidtecnic)
        {
            // arrange
            var assign = new ManualAssignModel
            {
                DocEntry = new List<int> { 1502 },
                OrderType = "Pedido",
                UserId = "abc",
                UserLogistic = "abd",
            };

            var mockSaDiApi = new Mock<ISapDiApi>();
            var mockUsers = new Mock<IUsersService>();

            mockSaDiApi
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUpdateOrder()));

            this.sapAdapter
                .Setup(m => m.GetSapAdapter(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetListCompleteDetailOrderModel()));

            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetQfbInfoDto(isValidtecnic)));

            var pedidosServiceLocal = new AssignPedidosService(this.sapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, mockUsers.Object, this.kafkaConnector.Object);

            // act
            var response = await pedidosServiceLocal.AssignOrder(assign);

            // assert
            Assert.IsNotNull(response);
            Assert.IsNull(response.ExceptionMessage);
            Assert.IsNull(response.Comments);

            if (isValidtecnic)
            {
                Assert.IsNotNull(response.UserError);
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
        public async Task AssignOrder()
        {
            // arrange
            var assign = new ManualAssignModel
            {
                DocEntry = new List<int> { 100 },
                OrderType = "Orden",
                UserId = "abc",
                UserLogistic = "abd",
            };

            var mockSaDiApi = new Mock<ISapDiApi>();
            mockSaDiApi
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultUpdateOrder()));

            var mockSapAdapter = new Mock<ISapAdapter>();
            mockSapAdapter
                .Setup(x => x.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()));

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetQfbInfoDto(true)));

            var pedidosServiceLocal = new AssignPedidosService(mockSapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, mockUsers.Object, this.kafkaConnector.Object);

            // act
            var response = await pedidosServiceLocal.AssignOrder(assign);

            // assert
            // ssert.IsNotNull(response);
            // assert
            Assert.IsNotNull(response);
            Assert.IsNull(response.ExceptionMessage);
            Assert.IsNull(response.Comments);
            Assert.IsNotNull(response.UserError);
            Assert.IsTrue(response.Success);
            Assert.AreEqual(200, response.Code);
            Assert.IsNotNull(response.Response);
        }

        /// <summary>
        /// the automatic assign test.
        /// </summary>
        [Test]
        public void AutomaticAssign()
        {
            var assign = new AutomaticAssingModel
            {
                DocEntry = new List<int> { 100, 101 },
                UserLogistic = "abc",
            };

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.SimpleGetUsers(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersByRole()));

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            mockSaDiApiLocal
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            var sapAdapterLocal = new Mock<ISapAdapter>();
            sapAdapterLocal
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()));

            var pedidoServiceLocal = new AssignPedidosService(sapAdapterLocal.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, this.kafkaConnector.Object);

            // act
            Assert.ThrowsAsync<CustomServiceException>(async () => await pedidoServiceLocal.AutomaticAssign(assign));
        }

        /// <summary>
        /// the automatic assign test.
        /// </summary>
        /// <returns>the reutn.</returns>
        [Test]
        public async Task AutomaticAssignDZ()
        {
            var assign = new AutomaticAssingModel
            {
                DocEntry = new List<int> { 900, 902 },
                UserLogistic = "abcde",
            };

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.SimpleGetUsers(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersByRoleWithDZ(false)));

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            mockSaDiApiLocal
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            var detalle900 = new CompleteDetailOrderModel { CodigoProducto = "DZ Test 1", IsOmigenomics = false, DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 900, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = "P", CreatedDate = DateTime.Now, Label = "Pesonalizada", ProductFirmName = string.Empty };
            var detalle901 = new CompleteDetailOrderModel { CodigoProducto = "DZ Test 2", IsOmigenomics = true, DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 901, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = "P", CreatedDate = DateTime.Now, Label = "Pesonalizada", ProductFirmName = string.Empty };
            var detalle902 = new CompleteDetailOrderModel { CodigoProducto = "567 120 ML", IsOmigenomics = false, DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 902, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = "P", CreatedDate = DateTime.Now, Label = "Pesonalizada", ProductFirmName = string.Empty };

            var order900 = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 900, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 900, PedidoStatus = "P", OrderType = "MG", DocNumDxp = "A1" };
            var order902 = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 902, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 902, PedidoStatus = "L", OrderType = "MG", DocNumDxp = "A1" };

            var listOrders = new List<OrderWithDetailModel>
            {
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel> { detalle900, detalle902 },
                    Order = order900,
                },
            };

            var realtioOrderType = new List<RelationOrderAndTypeModel>
            {
                new RelationOrderAndTypeModel { DocNum = 900, OrderType = "MN" },
                new RelationOrderAndTypeModel { DocNum = 902, OrderType = "MN" },
            };

            var relationShip = new List<RelationDxpDocEntryModel>
            {
                new RelationDxpDocEntryModel { DxpDocNum = "A1", DocNum = realtioOrderType },
            };

            var resultSap = this.GenerateResultModel(listOrders);
            resultSap.Response = listOrders;
            resultSap.Comments = relationShip;

            var sapAdapterLocal = new Mock<ISapAdapter>();
            sapAdapterLocal
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(resultSap));

            var pedidoServiceLocal = new AssignPedidosService(sapAdapterLocal.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, this.kafkaConnector.Object);
            var result = await pedidoServiceLocal.AutomaticAssign(assign);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(200, result.Code);
            Assert.IsNull(result.ExceptionMessage);
        }

        /// <summary>
        /// the automatic assign test.
        /// </summary>
        [Test]
        public void AutomaticAssignTecnicError()
        {
            var assign = new AutomaticAssingModel
            {
                DocEntry = new List<int> { 900, 902 },
                UserLogistic = "abcde",
            };

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.SimpleGetUsers(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersByRoleWithDZ(true, null)));

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            mockSaDiApiLocal
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            var detalle900 = new CompleteDetailOrderModel { CodigoProducto = "DZ Test 1", IsOmigenomics = false, DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 900, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = "P", CreatedDate = DateTime.Now, Label = "Pesonalizada", ProductFirmName = string.Empty };
            var detalle901 = new CompleteDetailOrderModel { CodigoProducto = "DZ Test 2", IsOmigenomics = true, DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 901, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = "P", CreatedDate = DateTime.Now, Label = "Pesonalizada", ProductFirmName = string.Empty };
            var detalle902 = new CompleteDetailOrderModel { CodigoProducto = "567 120 ML", IsOmigenomics = false, DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 902, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = "P", CreatedDate = DateTime.Now, Label = "Pesonalizada", ProductFirmName = string.Empty };

            var order900 = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 900, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 900, PedidoStatus = "P", OrderType = "MG", DocNumDxp = "A1" };
            var order902 = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 902, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 902, PedidoStatus = "L", OrderType = "MG", DocNumDxp = "A1" };

            var listOrders = new List<OrderWithDetailModel>
            {
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel> { detalle900, detalle902 },
                    Order = order900,
                },
            };

            var realtioOrderType = new List<RelationOrderAndTypeModel>
            {
                new RelationOrderAndTypeModel { DocNum = 900, OrderType = "MN" },
                new RelationOrderAndTypeModel { DocNum = 902, OrderType = "MN" },
            };

            var relationShip = new List<RelationDxpDocEntryModel>
            {
                new RelationDxpDocEntryModel { DxpDocNum = "A1", DocNum = realtioOrderType },
            };

            var resultSap = this.GenerateResultModel(listOrders);
            resultSap.Response = listOrders;
            resultSap.Comments = relationShip;

            var sapAdapterLocal = new Mock<ISapAdapter>();
            sapAdapterLocal
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(resultSap));

            var pedidoServiceLocal = new AssignPedidosService(sapAdapterLocal.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, this.kafkaConnector.Object);
            Assert.ThrowsAsync<CustomServiceException>(async () => await pedidoServiceLocal.AutomaticAssign(assign));
        }

        /// <summary>
        /// the automatic assign test.
        /// </summary>
        /// <returns>the reutn.</returns>
        [Test]
        public async Task AutomaticAssignOnlyDZ()
        {
            var assign = new AutomaticAssingModel
            {
                DocEntry = new List<int> { 900, 901 },
                UserLogistic = "abcde",
            };

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.SimpleGetUsers(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersByRoleWithDZ()));

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            mockSaDiApiLocal
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            var detalle900 = new CompleteDetailOrderModel { CodigoProducto = "DZ Test 1", IsOmigenomics = false, DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 900, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = "P", CreatedDate = DateTime.Now, Label = "Pesonalizada", ProductFirmName = string.Empty };

            var order900 = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 900, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 900, PedidoStatus = "P", OrderType = "MG", DocNumDxp = "A1" };

            var listOrders = new List<OrderWithDetailModel>
            {
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel> { detalle900 },
                    Order = order900,
                },
            };

            var realtioOrderType = new List<RelationOrderAndTypeModel>
            {
                new RelationOrderAndTypeModel { DocNum = 900, OrderType = "MN" },
                new RelationOrderAndTypeModel { DocNum = 901, OrderType = "MN" },
            };

            var relationShip = new List<RelationDxpDocEntryModel>
            {
                new RelationDxpDocEntryModel { DxpDocNum = "A1", DocNum = realtioOrderType },
            };

            var resultSap = this.GenerateResultModel(listOrders);
            resultSap.Response = listOrders;
            resultSap.Comments = relationShip;

            var sapAdapterLocal = new Mock<ISapAdapter>();
            sapAdapterLocal
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(resultSap));

            var pedidoServiceLocal = new AssignPedidosService(sapAdapterLocal.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, this.kafkaConnector.Object);
            var result = await pedidoServiceLocal.AutomaticAssign(assign);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(200, result.Code);
            Assert.IsNull(result.ExceptionMessage);
        }

        /// <summary>
        /// the automatic assign test.
        /// </summary>
        [Test]
        public void AutomaticAssignDZError()
        {
            var assign = new AutomaticAssingModel
            {
                DocEntry = new List<int> { 100, 101 },
                UserLogistic = "abcde",
            };

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.SimpleGetUsers(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersByRole()));

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            mockSaDiApiLocal
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            var sapAdapterLocal = new Mock<ISapAdapter>();
            sapAdapterLocal
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailDZModel()));

            var pedidoServiceLocal = new AssignPedidosService(sapAdapterLocal.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, this.kafkaConnector.Object);

            // act
            Assert.ThrowsAsync<CustomServiceException>(async () => await pedidoServiceLocal.AutomaticAssign(assign), ServiceConstants.ErrorUsersDZAutomatico);
        }

        /// <summary>
        ///  The rest.
        /// </summary>
        /// <returns>the reutn.</returns>
        [Test]
        public async Task AutomaticAssignDXP()
        {
            var assign = new AutomaticAssingModel
            {
                DocEntry = new List<int> { 900, 901 },
                UserLogistic = "abc",
            };

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.SimpleGetUsers(It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetUsersByRole()));

            var mockSaDiApiLocal = new Mock<ISapDiApi>();
            mockSaDiApiLocal
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            var detalle900 = new CompleteDetailOrderModel { CodigoProducto = "Aspirina", DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 900, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = "L", CreatedDate = DateTime.Now, Label = "Pesonalizada", ProductFirmName = string.Empty };
            var detalle901 = new CompleteDetailOrderModel { CodigoProducto = "Aspirina", DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 901, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = "L", CreatedDate = DateTime.Now, Label = "Pesonalizada", ProductFirmName = string.Empty };

            var order900 = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 900, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 900, PedidoStatus = "L", OrderType = "MN", DocNumDxp = "A1" };
            var order901 = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 901, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 901, PedidoStatus = "L", OrderType = "MN", DocNumDxp = "A1" };

            var listOrders = new List<OrderWithDetailModel>
            {
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel> { detalle900 },
                    Order = order900,
                },
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel> { detalle901 },
                    Order = order901,
                },
            };

            var realtioOrderType = new List<RelationOrderAndTypeModel>
            {
                new RelationOrderAndTypeModel { DocNum = 900, OrderType = "MN" },
                new RelationOrderAndTypeModel { DocNum = 901, OrderType = "MN" },
            };

            var relationShip = new List<RelationDxpDocEntryModel>
            {
                new RelationDxpDocEntryModel { DxpDocNum = "A1", DocNum = realtioOrderType },
            };

            var resultSap = this.GenerateResultModel(listOrders);
            resultSap.Response = listOrders;
            resultSap.Comments = relationShip;

            var sapAdapterLocal = new Mock<ISapAdapter>();
            sapAdapterLocal
                .Setup(m => m.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(resultSap));

            var pedidoServiceLocal = new AssignPedidosService(sapAdapterLocal.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, this.kafkaConnector.Object);

            // act
            var result = await pedidoServiceLocal.AutomaticAssign(assign);

            // assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(200, result.Code);
            Assert.IsNull(result.ExceptionMessage);
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <param name="isValidtecnic">Is valid tecnic.</param>
        /// <returns>the data.</returns>
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task ReassignOrder(bool isValidtecnic)
        {
            var reassign = new ManualAssignModel
            {
                DocEntry = new List<int> { 1, 2, 3 },
                OrderType = "Pedido",
                UserId = "abc",
                UserLogistic = "abc",
            };

            var sapAdapterLocal = new Mock<ISapAdapter>();
            var mockUsers = new Mock<IUsersService>();
            var mockSaDiApiLocal = new Mock<ISapDiApi>();

            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetQfbInfoDto(isValidtecnic)));

            // act
            var assignPedidosService = new AssignPedidosService(sapAdapterLocal.Object, this.pedidosDao, mockSaDiApiLocal.Object, mockUsers.Object, this.kafkaConnector.Object);
            var result = await assignPedidosService.ReassignOrder(reassign);

            // assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.ExceptionMessage);
            Assert.IsNull(result.Response);
            Assert.IsNull(result.Comments);

            if (isValidtecnic)
            {
                Assert.IsNull(result.UserError);
                Assert.IsTrue(result.Success);
                Assert.AreEqual(200, result.Code);
            }
            else
            {
                Assert.IsNotNull(result.UserError);
                Assert.IsFalse(result.Success);
                Assert.AreEqual(400, result.Code);
            }
        }

        /// <summary>
        /// Get last isolated production order id.
        /// </summary>
        /// <returns>the data.</returns>
        [Test]
        public async Task ReassignOrderOrden()
        {
            var reassign = new ManualAssignModel
            {
                DocEntry = new List<int> { 1502 },
                OrderType = "Orden",
                UserId = "abc",
                UserLogistic = "abc",
            };

            var mockSapAdapter = new Mock<ISapAdapter>();
            mockSapAdapter
                .Setup(x => x.PostSapAdapter(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultModelCompleteDetailModel()));

            var mockSaDiApi = new Mock<ISapDiApi>();
            mockSaDiApi
                .Setup(x => x.PostToSapDiApi(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetResultCreateOrder()));

            mockSaDiApi
                .Setup(x => x.GetSapDiApi(It.IsAny<string>()))
                .Returns(Task.FromResult(new ResultModel()));

            var mockUsers = new Mock<IUsersService>();
            mockUsers
                .Setup(m => m.PostSimpleUsers(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(this.GetQfbInfoDto(true)));

            var pedidosServiceLocal = new AssignPedidosService(mockSapAdapter.Object, this.pedidosDao, mockSaDiApi.Object, mockUsers.Object, this.kafkaConnector.Object);

            // act
            var result = await pedidosServiceLocal.ReassignOrder(reassign);

            // assert
            Assert.IsNotNull(result);
        }
    }
}
