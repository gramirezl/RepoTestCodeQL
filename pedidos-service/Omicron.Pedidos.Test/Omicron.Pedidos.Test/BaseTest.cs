// <summary>
// <copyright file="BaseTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Dtos.User;
    using Omicron.Pedidos.Entities.Context;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;

    /// <summary>
    /// Class Base Test.
    /// </summary>
    public abstract class BaseTest
    {
        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public UserDto GetUserDto()
        {
            return new UserDto
            {
                Id = 10,
                FirstName = "Jorge",
                LastName = "Morales",
                Email = "test@test.com",
                Birthdate = DateTime.Now,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public List<UserOrderModel> GetUserOrderModel()
        {
            var magistralQr = new MagistralQrModel
            {
                NeedsCooling = "Y",
                ProductionOrder = 100,
                Quantity = 1,
                SaleOrder = 100,
                ItemCode = "ITEM CODE 200",
            };

            var remisionQr = new RemisionQrModel
            {
                RemisionId = 100,
                NeedsCooling = true,
                PedidoId = 300,
                TotalPieces = 5,
            };

            var invoiceQr = new InvoiceQrModel
            {
                InvoiceId = 100,
                NeedsCooling = false,
            };

            var labelMuestra = new RemisionQrModel
            {
                PedidoId = 1,
                Ship = "Pedido Muestra",
            };

            return new List<UserOrderModel>
            {
                new UserOrderModel { Id = 1, Productionorderid = "100", Salesorderid = "100", Status = "Asignado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 1 },
                new UserOrderModel { Id = 2, Productionorderid = "101", Salesorderid = "100", Status = "Proceso", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 2 },
                new UserOrderModel { Id = 3, Productionorderid = "102", Salesorderid = "100", Status = "Terminado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 3, },
                new UserOrderModel { Id = 4, Productionorderid = "103", Salesorderid = "100", Status = "Reasignado", Userid = "abc", Comments = null, FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 4 },
                new UserOrderModel { Id = 5, Productionorderid = null, Salesorderid = "100", Status = "Terminado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 5 },
                new UserOrderModel { Id = 6, Productionorderid = null, Salesorderid = "100", Status = "Reasignado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 6 },
                new UserOrderModel { Id = 18, Productionorderid = "200", Salesorderid = "200", Status = "Reasignado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 7 },
                new UserOrderModel { Id = 19, Productionorderid = "301", Salesorderid = "300", Status = "Finalizado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 8 },
                new UserOrderModel { Id = 20, Productionorderid = null, Salesorderid = "300", Status = "Finalizado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 9 },

                // not complet sales log
                new UserOrderModel { Id = 1000, Productionorderid = "501", Salesorderid = "800", Status = "Finalizado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "21/03/2021", CreatorUserId = "abc", Quantity = 10 },
                new UserOrderModel { Id = 1002, Productionorderid = "502", Salesorderid = "800", Status = "Finalizado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "21/03/2021", CreatorUserId = "abc", Quantity = 11 },
                new UserOrderModel { Id = 1003, Productionorderid = null, Salesorderid = "800", Status = "Finalizado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "21/03/2021", CreatorUserId = "abc", Quantity = 12 },

                // Cancelled orders.
                new UserOrderModel { Id = 7, Productionorderid = null, Salesorderid = "100", Status = "Terminado", Userid = "abcd", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), Quantity = 13 },
                new UserOrderModel { Id = 8, Productionorderid = null, Salesorderid = "100", Status = "Reasignado", Userid = "abcd", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), Quantity = 14 },
                new UserOrderModel { Id = 9, Productionorderid = null, Salesorderid = "101", Status = "Asignado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), Quantity = 15 },
                new UserOrderModel { Id = 10, Productionorderid = "104", Salesorderid = "103", Status = "Proceso", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), Quantity = 16 },
                new UserOrderModel { Id = 11, Productionorderid = "105", Salesorderid = "103", Status = "Cancelado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), Quantity = 17 },
                new UserOrderModel { Id = 12, Productionorderid = null, Salesorderid = "103", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), Quantity = 18 },
                new UserOrderModel { Id = 13, Productionorderid = "106", Salesorderid = "103", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), Quantity = 19 },
                new UserOrderModel { Id = 14, Productionorderid = null, Salesorderid = "104", Status = "Terminado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), Quantity = 20 },
                new UserOrderModel { Id = 15, Productionorderid = "107", Salesorderid = "104", Status = "Terminado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), Quantity = 21 },
                new UserOrderModel { Id = 16, Productionorderid = "108", Salesorderid = "104", Status = "Cancelado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), Quantity = 22 },
                new UserOrderModel { Id = 17, Productionorderid = "109", Salesorderid = "104", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), Quantity = 23 },

                // orders for almacen
                new UserOrderModel { Id = 98, Salesorderid = "104", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), FinishedLabel = 1, FinalizedDate = DateTime.Now, MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 24 },
                new UserOrderModel { Id = 99, Productionorderid = "301", Salesorderid = "104", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 25 },

                // Orders for Qr.
                new UserOrderModel { Id = 100, Productionorderid = "300", Salesorderid = "104", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 26 },
                new UserOrderModel { Id = 101, Productionorderid = "301", Salesorderid = "104", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 27 },
                new UserOrderModel { Id = 102, Productionorderid = "302", Salesorderid = "105", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 28 },
                new UserOrderModel { Id = 103, Productionorderid = "303", Salesorderid = "105", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), DeliveryId = 105, Quantity = 29 },

                // orders for invoice
                new UserOrderModel { Id = 104, Productionorderid = null, Salesorderid = "106", Status = "Almacenado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 30 },
                new UserOrderModel { Id = 105, Productionorderid = "2", Salesorderid = "106", Status = "Almacenado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 31 },

                // order for invoice qr
                new UserOrderModel { Id = 106, Productionorderid = null, Salesorderid = "107", Status = "Almacenado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), InvoiceId = 100, InvoiceQr = JsonConvert.SerializeObject(invoiceQr), Quantity = 32 },

                // orders for packages
                new UserOrderModel { Id = 107, Productionorderid = null, Salesorderid = "107", Status = "Almacenado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), InvoiceId = 100, InvoiceQr = JsonConvert.SerializeObject(invoiceQr), StatusInvoice = "Empaquetado", InvoiceType = "local", Quantity = 33 },
                new UserOrderModel { Id = 108, Productionorderid = null, Salesorderid = "107", Status = "Almacenado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), InvoiceId = 101, InvoiceQr = JsonConvert.SerializeObject(invoiceQr), StatusInvoice = "Empaquetado", InvoiceType = "local", Quantity = 34 },

                // order pending for graph
                new UserOrderModel { Id = 109, Productionorderid = null, Salesorderid = "700", Status = "Liberado", FinalizedDate = DateTime.Now, Quantity = 35 },
                new UserOrderModel { Id = 110, Productionorderid = "300", Salesorderid = "700", Status = "Finalizado", FinalizedDate = DateTime.Now, FinishedLabel = 1, Quantity = 36 },
                new UserOrderModel { Id = 111, Productionorderid = "301", Salesorderid = "700", Status = "Pendiente", FinalizedDate = DateTime.Now, Quantity = 37 },
                new UserOrderModel { Id = 112, Productionorderid = "301", Salesorderid = "201", Status = "Pendiente", FinalizedDate = DateTime.Now, InvoiceId = 1, InvoiceType = "local", StatusInvoice = "Empaquetado", Quantity = 38 },
                new UserOrderModel { Id = 113, Productionorderid = "301", Salesorderid = "201", Status = "Pendiente", FinalizedDate = DateTime.Now, InvoiceId = 2, InvoiceType = "local", StatusInvoice = "Asignado", Quantity = 39 },
                new UserOrderModel { Id = 114, Productionorderid = "301", Salesorderid = "201", Status = "Pendiente", FinalizedDate = DateTime.Now, InvoiceId = 3, InvoiceType = "local", StatusInvoice = "En Camino", Quantity = 40 },
                new UserOrderModel { Id = 115, Productionorderid = "301", Salesorderid = "201", Status = "Pendiente", FinalizedDate = DateTime.Now, InvoiceId = 4, InvoiceType = "local", StatusInvoice = "Entregado", Quantity = 41 },
                new UserOrderModel { Id = 116, Productionorderid = "301", Salesorderid = "201", Status = "Pendiente", FinalizedDate = DateTime.Now, InvoiceId = 5, InvoiceType = "local", StatusInvoice = "No Entregado", Quantity = 42 },
                new UserOrderModel { Id = 117, Productionorderid = "301", Salesorderid = "201", Status = "Pendiente", FinalizedDate = DateTime.Now, InvoiceId = 6, InvoiceType = "foraneo", StatusInvoice = "Empaquetado", Quantity = 43 },
                new UserOrderModel { Id = 118, Productionorderid = "301", Salesorderid = "201", Status = "Pendiente", FinalizedDate = DateTime.Now, InvoiceId = 7, InvoiceType = "foraneo", StatusInvoice = "Enviado", Quantity = 44 },
                new UserOrderModel { Id = 119, Productionorderid = null, Salesorderid = "202", Status = "Recibir", StatusAlmacen = "Back Order", FinalizedDate = DateTime.Now, Quantity = 45 },
                new UserOrderModel { Id = 120, Productionorderid = null, Salesorderid = "203", Status = "Almacenado", FinalizedDate = DateTime.Now, Quantity = 46 },

                // orders DXP
                new UserOrderModel { Id = 121, Productionorderid = null, Salesorderid = "204", StatusInvoice = "Enviado", Quantity = 47 },
                new UserOrderModel { Id = 122, Productionorderid = null, Salesorderid = "205", StatusInvoice = "Entregado", Quantity = 48 },

                // orders for almacen By Id
                new UserOrderModel { Id = 123, Productionorderid = null, Salesorderid = "206", Status = "Liberado", Quantity = 47, FinishedLabel = 0 },
                new UserOrderModel { Id = 124, Productionorderid = "2", Salesorderid = "206", Status = "Finalizado", Quantity = 48, FinishedLabel = 1 },
                new UserOrderModel { Id = 125, Productionorderid = "3", Salesorderid = "206", Status = "Almacenado", Quantity = 47, FinishedLabel = 1 },
                new UserOrderModel { Id = 126, Productionorderid = "4", Salesorderid = "206", Status = "Pendiente", Quantity = 48, FinishedLabel = 0 },
                new UserOrderModel { Id = 127, Productionorderid = null, Salesorderid = "207", Status = "Finalizado", Quantity = 47, FinishedLabel = 1 },
                new UserOrderModel { Id = 128, Productionorderid = "2", Salesorderid = "207", Status = "Finalizado", Quantity = 48, FinishedLabel = 1 },

                // orders for sample lable
                new UserOrderModel { Id = 129, Productionorderid = "303", Salesorderid = "208", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(labelMuestra), DeliveryId = 105, Quantity = 29 },

                // order for manual assign Sale order
                new UserOrderModel { Id = 130, Productionorderid = "100", Salesorderid = "1502", Status = "Asignado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 1 },
                new UserOrderModel { Id = 131, Productionorderid = "101", Salesorderid = "1502", Status = "Planificado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 2 },
                new UserOrderModel { Id = 132, Productionorderid = null, Salesorderid = "1502", Status = "Planificado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 3, },

                // order for autmatic DXP
                new UserOrderModel { Id = 133, Productionorderid = "900", Salesorderid = "900", Status = "Planificado", Userid = null, Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 1 },
                new UserOrderModel { Id = 134, Productionorderid = null, Salesorderid = "900", Status = "Planificado", Userid = null, Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 2 },
                new UserOrderModel { Id = 135, Productionorderid = "901", Salesorderid = "901", Status = "Planificado", Userid = null, Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 1 },
                new UserOrderModel { Id = 136, Productionorderid = null, Salesorderid = "901", Status = "Planificado", Userid = null, Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 2 },

                // Tecnical id
                new UserOrderModel { Id = 137, Productionorderid = null, Salesorderid = "901", Status = "Planificado", Userid = "abc",  TecnicId = "tecnial", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 2 },
                new UserOrderModel { Id = 138, Productionorderid = null, Salesorderid = "902", Status = "Proceso", Userid = "abcquimico",  TecnicId = "tecnicoqfb", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 2, StatusForTecnic = "Asignado" },
                new UserOrderModel { Id = 139, Productionorderid = null, Salesorderid = "903", Status = "Asignado", Userid = "abcquimicocd",  TecnicId = "tecnicoqfb2", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 1, StatusForTecnic = "Asignado" },

                // Test For signed orders
                new UserOrderModel { Id = 140, Productionorderid = "223740", Salesorderid = "901", Status = "Planificado", Userid = "abc",  TecnicId = null, Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 2 },
                new UserOrderModel { Id = 141, Productionorderid = "224212", Salesorderid = "902", Status = "Proceso", Userid = "abcquimico",  TecnicId = "tecnicoqfb", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 2, StatusForTecnic = "Asignado" },
                new UserOrderModel { Id = 142, Productionorderid = "224211", Salesorderid = "903", Status = "Asignado", Userid = "abcquimicocd",  TecnicId = "tecnicoqfb2", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 1, StatusForTecnic = "Asignado" },
                new UserOrderModel { Id = 143, Productionorderid = "224159", Salesorderid = "903", Status = "Asignado", Userid = "abcquimicocd",  TecnicId = "tecnicoqfb2", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 1, StatusForTecnic = "Asignado" },
            };
        }

        /// <summary>
        /// retruns a list od completedetailorder.
        /// </summary>
        /// <returns>the data.</returns>
        public List<OrderWithDetailModel> GetOrderWithDetailModel()
        {
            var listDetalles = new List<CompleteDetailOrderModel>
            {
                new CompleteDetailOrderModel { CodigoProducto = "Aspirina", DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 100, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = null },
            };

            var listOrders = new List<OrderWithDetailModel>
            {
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel>(listDetalles),
                    Order = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 1, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 100, PedidoStatus = null },
                },
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel>(listDetalles),
                    Order = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 100, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 100, PedidoStatus = null },
                },
            };

            return listOrders;
        }

        /// <summary>
        /// Gets the signature.
        /// </summary>
        /// <returns>the data.</returns>
        public List<UserOrderSignatureModel> GetSignature()
        {
            return new List<UserOrderSignatureModel>
            {
                new UserOrderSignatureModel { Id = 1000, LogisticSignature = null, TechnicalSignature = null, UserOrderId = 1 },
                new UserOrderSignatureModel { Id = 1, LogisticSignature = null, TechnicalSignature = Convert.FromBase64String("aG9sYQ=="), UserOrderId = 142 },
                new UserOrderSignatureModel { Id = 2, LogisticSignature = null, TechnicalSignature = null, UserOrderId = 143 },
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultModelGetFabricacionModel()
        {
            var listOrders = new List<FabricacionOrderModel>
            {
                new FabricacionOrderModel { DataSource = "O", OrdenId = 100, PedidoId = 100, PostDate = DateTime.Now, ProductoId = "Aspirina", Quantity = 10, Status = "R" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listOrders),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultModelGetFabricacionModelNoSerealize()
        {
            var listOrders = new List<FabricacionOrderModel>
            {
                new FabricacionOrderModel { DataSource = "O", OrdenId = 100, PedidoId = 100, PostDate = DateTime.Now, ProductoId = "Aspirina", Quantity = 10, Status = "R" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = listOrders,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultModelCompleteDetailModel()
        {
            var listDetalles = new List<CompleteDetailOrderModel>
            {
                new CompleteDetailOrderModel
                {
                    CodigoProducto = "Aspirina",
                    DescripcionProducto = "dec",
                    FechaOf = "2020/01/01",
                    FechaOfFin = "2020/01/01",
                    IsChecked = false,
                    OrdenFabricacionId = 100,
                    Qfb = "qfb",
                    QtyPlanned = 1,
                    QtyPlannedDetalle = 1,
                    Status = "L",
                    CreatedDate = DateTime.Now,
                    Label = "Pesonalizada",
                    IsOmigenomics = false,
                    ProductFirmName = string.Empty,
                },
            };

            var listOrders = new List<OrderWithDetailModel>
            {
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel>(listDetalles),
                    Order = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 1, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 100, PedidoStatus = "L", OrderType = "MN" },
                },
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel>(listDetalles),
                    Order = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 100, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 100, PedidoStatus = "L", OrderType = "MG" },
                },
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel>(listDetalles),
                    Order = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 101, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 100, PedidoStatus = "L", OrderType = "MX" },
                },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = listOrders,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultModelCompleteDetailDZModel()
        {
            var listDetailDZ = Enumerable.Range(1, 1)
                .Select(detail => new CompleteDetailOrderModel
                {
                    CodigoProducto = $"DZ Test {detail}",
                    DescripcionProducto = "dec",
                    FechaOf = "2020/01/01",
                    FechaOfFin = "2020/01/01",
                    IsChecked = false,
                    OrdenFabricacionId = 100,
                    Qfb = "qfb",
                    QtyPlanned = 1,
                    QtyPlannedDetalle = 1,
                    Status = "L",
                    CreatedDate = DateTime.Now,
                    Label = "Pesonalizada",
                    IsOmigenomics = false,
                    ProductFirmName = string.Empty,
                });

            var listOrders = Enumerable.Range(1, 7)
                .Select(x =>
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel>(listDetailDZ),
                    Order = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = x, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = x, PedidoStatus = "L", OrderType = "MG" },
                }).ToList();

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = listOrders,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// gets the recipes.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultModel GetRecipes()
        {
            var recipes = new List<OrderRecipeModel>
            {
                new OrderRecipeModel { Order = 107, Recipe = "C:aglo" },
                new OrderRecipeModel { Order = 100, Recipe = "C:aglo" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = recipes,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets the fabrication orders model.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultModel GetFabricacionOrderModel()
        {
            var listData = new List<FabricacionOrderModel>
            {
                new FabricacionOrderModel { CreatedDate = DateTime.Now, OrdenId = 100, ProductoId = "Aspirina" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = listData,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultCreateOrder()
        {
            var listOrders = new Dictionary<string, string>
            {
                { "100-Aspirina-1-100", ServiceConstants.Ok },
                { "200-Aspirina", $"{ServiceConstants.ErrorCreateFabOrd}-404-Error en la cantidad" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listOrders),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <param name="isTecnic">Is tecnic.</param>
        /// <returns>the user.</returns>
        public ResultModel GetResultUserModel(bool isTecnic = false)
        {
            List<UserModel> listUsers;
            if (isTecnic)
            {
                listUsers = new List<UserModel>
                {
                    new UserModel { Activo = 1, FirstName = "Sutano", Id = "tecnic", LastName = "Lope", Password = "as", Role = 9, UserName = "sutan", Piezas = 1000, Asignable = 1 },
                };
            }
            else
            {
                listUsers = new List<UserModel>
                {
                    new UserModel { Activo = 1, FirstName = "Sutano", Id = "abc", LastName = "Lope", Password = "as", Role = 1, UserName = "sutan", Piezas = 1000, Asignable = 1, TechnicalRequire = true },
                };
            }

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listUsers),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultAssignBatch()
        {
            var listOrders = new Dictionary<string, string>
            {
                { "100-Aspirina-101", ServiceConstants.Ok },
                { "200-Aspirina-023", ServiceConstants.ErrorUpdateFabOrd },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listOrders),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetBatches()
        {
            var assigneBatches = new List<AssignedBatches>
            {
                new AssignedBatches { CantidadSeleccionada = 10, NumeroLote = "asd", SysNumber = 1 },
            };

            var listOrders = new List<BatchesComponentModel>
            {
                new BatchesComponentModel { CodigoProducto = "asd", LotesAsignados = assigneBatches },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listOrders),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetMissingBatches()
        {
            var listOrders = new List<BatchesComponentModel>
            {
                new BatchesComponentModel { CodigoProducto = "asd", LotesAsignados = new List<AssignedBatches>() },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listOrders),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultUpdateOrder()
        {
            var listOrders = new Dictionary<string, string>
            {
                { "100-100", ServiceConstants.Ok },
                { "200-200", ServiceConstants.ErrorUpdateFabOrd },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listOrders),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// the values for the formulas.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultModel GetFormulaDetalle()
        {
            var listFormula = new CompleteFormulaWithDetalle { BaseDocument = 100, Client = "C001", Code = "Aspirina", Container = "container", CompleteQuantity = 10, Details = new List<CompleteDetalleFormulaModel>(), DueDate = "01/01/2020", EndDate = "01/01/2020", FabDate = "01/01/2020", IsChecked = false, Number = 100, Origin = "PT", PlannedQuantity = 100, ProductDescription = "orden", ProductionOrderId = 100, ProductLabel = "label", RealEndDate = "01/01/2020", StartDate = "01/01/2020", Status = "L", Type = "type", Unit = "KG", User = "manager", Warehouse = "MN", DestinyAddress = "Nuevo Le?n, Mexico, CP. 54715", Comments = "Cooments" };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = listFormula,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// the values for the formulas.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultModel GetListFormulaDetalle()
        {
            var listFormula = new List<CompleteFormulaWithDetalle>
            {
                new CompleteFormulaWithDetalle { BaseDocument = 100, Client = "C001", Code = "Aspirina", Container = "container", CompleteQuantity = 10, Details = new List<CompleteDetalleFormulaModel>(), DueDate = "01/01/2020", EndDate = "01/01/2020", FabDate = "01/01/2020", IsChecked = false, Number = 100, Origin = "PT", PlannedQuantity = 100, ProductDescription = "orden", ProductionOrderId = 100, ProductLabel = "label", RealEndDate = "01/01/2020", StartDate = "01/01/2020", Status = "L", Type = "type", Unit = "KG", User = "manager", Warehouse = "MN", DestinyAddress = "Nuevo Le?n, Mexico, CP. 54715", Comments = "Cooments", HasMissingStock = false },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = listFormula,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// gets the result from detail orde rmodel.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultModel GetListCompleteDetailOrderModel()
        {
            var listDetails = new List<CompleteDetailOrderModel>
            {
                new CompleteDetailOrderModel { CodigoProducto = "CA", DescripcionProducto = "desc", FechaOf = "20/01/2020", FechaOfFin = "01/01/2020", IsChecked = false, OrdenFabricacionId = 100, Qfb = "qfb", QtyPlanned = 100, QtyPlannedDetalle = 100, Status = "Planificado" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listDetails),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// gets the users by role.
        /// </summary>
        /// <returns>the users.</returns>
        public ResultModel GetUsersByRole()
        {
            var users = new List<UserModel>
            {
                new UserModel { Id = "abc", Activo = 1, FirstName = "Gustavo", LastName = "Ramirez", Password = "pass", Role = 2, UserName = "gus1", Piezas = 1000, Asignable = 1, Classification = "MN" },
                new UserModel { Id = "abcd", Activo = 1, FirstName = "Hugo", LastName = "Ramirez", Password = "pass", Role = 2, UserName = "gus1", Piezas = 1000, Asignable = 1, Classification = "BE" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = users,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// gets the users by role.
        /// </summary>
        /// <param name="technicalRequire">Is Technical Require.</param>
        /// <param name="tecnicId">Tecnic id.</param>
        /// <returns>the users.</returns>
        public ResultModel GetUsersByRoleWithDZ(bool technicalRequire = false, string tecnicId = "8c426e2d-0615-4516-a94c-a79e5c11ae4d")
        {
            var users = new List<UserModel>
            {
                new UserModel { Id = "abc", Activo = 1, FirstName = "Gustavo", LastName = "Ramirez", Password = "pass", Role = 2, UserName = "gus1", Piezas = 1000, Asignable = 1, Classification = "MN", TechnicalRequire = true, TecnicId = "8c426e2d-0615-4516-a94c-a79e5c11ae4d" },
                new UserModel { Id = "abcd", Activo = 1, FirstName = "Hugo", LastName = "Ramirez", Password = "pass", Role = 2, UserName = "gus1", Piezas = 1000, Asignable = 1, Classification = "BE", TechnicalRequire = true, TecnicId = "8c426e2d-0615-4516-a94c-a79e5c11ae4d" },
                new UserModel { Id = "abcde", Activo = 1, FirstName = "Magistrales", LastName = "Magistrales", Password = "pass", Role = 2, UserName = "gus1", Piezas = 1000, Asignable = 1, Classification = "MG", TechnicalRequire = technicalRequire },
                new UserModel { Id = "abcdef", Activo = 1, FirstName = "Test DZ 1", LastName = "Test DZ 1", Password = "pass", Role = 2, UserName = "gus1", Piezas = 0, Asignable = 1, Classification = "DZ", TechnicalRequire = technicalRequire, TecnicId = tecnicId },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = users,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Generates a result model.
        /// </summary>
        /// <param name="dataToSend">the data.</param>
        /// <returns>the dta.</returns>
        public ResultModel GenerateResultModel(object dataToSend)
        {
            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(dataToSend),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// gets the users by role.
        /// </summary>
        /// <param name="isValidtecnic">Is valid tecnic.</param>
        /// <returns>the users.</returns>
        public ResultModel GetQfbInfoDto(bool isValidtecnic)
        {
            var qfbValidatedInfo = new List<QfbTecnicInfoDto>
            {
                new QfbTecnicInfoDto
                {
                    IsTecnicRequired = true,
                    IsValidTecnic = isValidtecnic,
                    QfbFirstName = "Juan",
                    QfbLastName = "P�rez",
                    QfbId = "abc",
                    TecnicId = "6bc7f8a8-8617-43ac-a804-79cf9667b801",
                    IsValidQfbConfiguration = true,
                },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = qfbValidatedInfo,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Get new db context for in memory database.
        /// </summary>
        /// <param name="dbname">Data base name.</param>
        /// <returns>New context options.</returns>
        internal static DbContextOptions<DatabaseContext> CreateNewContextOptions(string dbname)
        {
            // Create a fresh service provider, and therefore a fresh.
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<DatabaseContext>();
            builder.UseInMemoryDatabase(dbname)
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }
    }
}
