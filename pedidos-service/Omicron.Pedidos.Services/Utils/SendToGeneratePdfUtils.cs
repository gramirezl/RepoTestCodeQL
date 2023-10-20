﻿// <summary>
// <copyright file="SendToGeneratePdfUtils.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.SapAdapter;
    using Omicron.Pedidos.Services.SapFile;
    using Omicron.Pedidos.Services.User;

    /// <summary>
    /// Class to generate pdfs.
    /// </summary>
    public static class SendToGeneratePdfUtils
    {
        /// <summary>
        /// Creates the models to send.
        /// </summary>
        /// <param name="ordersId">the orders id.</param>
        /// <param name="fabOrdersId">the fab orders id.</param>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <param name="pedidosDao">the pedidos dao.</param>
        /// <param name="sapFileService">the sap file service.</param>
        /// <param name="usersService">the user service.</param>
        /// <param name="onlyFinalized">if only applies to finalized.</param>
        /// <returns>the data.</returns>
        public static async Task<Task<ResultModel>> CreateModelGeneratePdf(
            List<int> ordersId,
            List<int> fabOrdersId,
            ISapAdapter sapAdapter,
            IPedidosDao pedidosDao,
            ISapFileService sapFileService,
            IUsersService usersService,
            bool onlyFinalized)
        {
            var listOrdersWithDetail = new List<OrderWithDetailModel>();
            var listFabOrders = new List<FabricacionOrderModel>();
            var recipes = new List<OrderRecipeModel>();
            var listToSend = new List<FinalizaGeneratePdfModel>();
            var listUserOrders = new List<UserOrderModel>();

            if (ordersId.Any())
            {
                listOrdersWithDetail = await GetDetails(ordersId, sapAdapter, ServiceConstants.GetOrderWithDetail);
                var listIdString = ordersId.Select(x => x.ToString()).ToList();
                var userSaleOrders = (await pedidosDao.GetUserOrderBySaleOrder(listIdString)).Where(x => x.Status != ServiceConstants.Cancelled).ToList();
                userSaleOrders = ServiceShared.CalculateTernary(onlyFinalized, userSaleOrders.Where(x => x.Status == ServiceConstants.Finalizado).ToList(), userSaleOrders);
                listUserOrders.AddRange(userSaleOrders);
                recipes = await GetRecipes(ordersId, sapAdapter, ServiceConstants.GetRecipes);
            }

            if (fabOrdersId.Any())
            {
                listFabOrders = await GetFabOrders(fabOrdersId, sapAdapter);
                var listFabOrderIdString = fabOrdersId.Select(x => x.ToString()).ToList();
                var userOrders = (await pedidosDao.GetUserOrderByProducionOrder(listFabOrderIdString)).Where(x => x.Status != ServiceConstants.Cancelled).ToList();
                listUserOrders.AddRange(userOrders);
            }

            var userIds = listUserOrders.Where(x => !string.IsNullOrEmpty(x.Userid)).Select(x => x.Userid).DistinctBy(x => x).ToList();
            var userOrdersId = listUserOrders.Select(x => x.Id).ToList();
            var orderSignature = (await pedidosDao.GetSignaturesByUserOrderId(userOrdersId)).ToList();

            var designerIds = orderSignature.Where(x => !string.IsNullOrEmpty(x.DesignerId)).Select(x => x.DesignerId).ToList();
            userIds.AddRange(designerIds);

            var tecnicIds = listUserOrders.Where(x => !string.IsNullOrEmpty(x.TecnicId)).Select(x => x.TecnicId).DistinctBy(x => x).ToList();
            userIds.AddRange(tecnicIds);

            var users = await GetUsers(userIds, usersService);

            listToSend.AddRange(GetModelsBySaleOrders(listOrdersWithDetail, recipes, users, orderSignature, listUserOrders));
            listToSend.AddRange(GetModelByOrder(listFabOrders, users, orderSignature, listUserOrders));

            return sapFileService.PostSimple(listToSend, ServiceConstants.CreatePdf);
        }

        /// <summary>
        /// Creates the models by sale Order.
        /// </summary>
        /// <param name="ordersWithDetail">the orders with detail.</param>
        /// <param name="recipes">the recipes.</param>
        /// <param name="users">the users.</param>
        /// <param name="signatures">the signatures.</param>
        /// <param name="userOrders">the userOrders.</param>
        /// <returns>the data.</returns>
        private static List<FinalizaGeneratePdfModel> GetModelsBySaleOrders(
            List<OrderWithDetailModel> ordersWithDetail,
            List<OrderRecipeModel> recipes,
            List<UserModel> users,
            List<UserOrderSignatureModel> signatures,
            List<UserOrderModel> userOrders)
        {
            var listToReturn = new List<FinalizaGeneratePdfModel>();
            foreach (var order in ordersWithDetail)
            {
                var recipe = recipes.Where(r => r.Order == order.Order.PedidoId).Select(y => y.Recipe).ToList();

                if (!order.Detalle.Any(d => d.OrdenFabricacionId != 0))
                {
                    var modelOrder = new FinalizaGeneratePdfModel
                    {
                        OrderId = order.Order.PedidoId,
                        SaleOrderCreateDate = order.Order.FechaInicio.ToString("dd/MM/yyyy"),
                        MedicName = NormalizeMedicName(order.Order.Medico),
                        RecipeRoute = recipe == null ? new List<string>() : recipe,
                    };

                    listToReturn.Add(modelOrder);
                    continue;
                }

                foreach (var detail in order.Detalle.Where(x => x.OrdenFabricacionId != 0).ToList())
                {
                    var userOrder = userOrders.Where(y => !string.IsNullOrEmpty(y.Productionorderid)).FirstOrDefault(x => x.Productionorderid.Equals(detail.OrdenFabricacionId.ToString()));
                    userOrder = userOrder ?? new UserOrderModel { Id = -1, Userid = "NoUser" };

                    if (userOrder.Id == -1)
                    {
                        continue;
                    }

                    var signaturesByOrder = signatures.FirstOrDefault(x => x.UserOrderId == userOrder.Id);
                    var user = users.FirstOrDefault(x => x.Id.Equals(userOrder.Userid));

                    var designerId = signaturesByOrder == null || signaturesByOrder.DesignerId == null ? string.Empty : signaturesByOrder.DesignerId;
                    var designer = users.FirstOrDefault(x => x.Id.Equals(designerId));

                    var tecnical = users.FirstOrDefault(x => x.Id.Equals(userOrder.TecnicId));

                    var qfbName = user == null ? string.Empty : $"{user.FirstName} {user.LastName}";
                    var tecnicalName = tecnical == null ? qfbName : $"{tecnical.FirstName} {tecnical.LastName}";

                    var qfbSignature = signaturesByOrder?.QfbSignature == null ? new byte[0] : signaturesByOrder.QfbSignature;
                    var technicalSignature = signaturesByOrder?.TechnicalSignature == null ? qfbSignature : signaturesByOrder.TechnicalSignature;

                    var model = new FinalizaGeneratePdfModel
                    {
                        CreateDate = detail.CreatedDate.HasValue ? detail.CreatedDate.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy"),
                        FabOrderId = detail.OrdenFabricacionId,
                        ItemCode = detail.CodigoProducto,
                        MedicName = NormalizeMedicName(order.Order.Medico),
                        OrderId = order.Order.PedidoId,
                        QfbName = qfbName,
                        QfbSignature = qfbSignature,
                        RecipeRoute = recipe ?? new List<string>(),
                        SaleOrderCreateDate = order.Order.FechaInicio.ToString("dd/MM/yyyy"),
                        TechnicalSignature = technicalSignature,
                        UserOrderId = userOrder.Id,
                        DesignerName = designer == null ? string.Empty : $"{designer.FirstName} {designer.LastName}",
                        DesignerSignature = signaturesByOrder == null ? new byte[0] : signaturesByOrder.DesignerSignature,
                        TechnicalName = tecnicalName,
                    };

                    listToReturn.Add(model);
                }
            }

            return listToReturn;
        }

        /// <summary>
        /// Creates the model for fab orders.
        /// </summary>
        /// <param name="orders">the fab orders.</param>
        /// <param name="users">the user.</param>
        /// <param name="signatures">the signatures.</param>
        /// <param name="userOrders">the userordees.</param>
        /// <returns>the data.</returns>
        private static List<FinalizaGeneratePdfModel> GetModelByOrder(List<FabricacionOrderModel> orders, List<UserModel> users, List<UserOrderSignatureModel> signatures, List<UserOrderModel> userOrders)
        {
            var listToReturn = new List<FinalizaGeneratePdfModel>();

            foreach (var order in orders)
            {
                var userOrder = userOrders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).FirstOrDefault(x => x.Productionorderid.Equals(order.OrdenId.ToString()));
                userOrder = userOrder ?? new UserOrderModel { Id = -1, Userid = "NoUser" };

                if (userOrder.Id == -1)
                {
                    continue;
                }

                var signaturesByOrder = signatures.FirstOrDefault(x => x.UserOrderId == userOrder.Id);
                var user = users.FirstOrDefault(x => x.Id.Equals(userOrder.Userid));
                var tecnical = users.FirstOrDefault(x => x.Id.Equals(userOrder.TecnicId));

                var qfbName = user == null ? string.Empty : $"{user.FirstName} {user.LastName}";
                var tecnicalName = tecnical == null ? qfbName : $"{tecnical.FirstName} {tecnical.LastName}";

                var qfbSignature = signaturesByOrder?.QfbSignature == null ? new byte[0] : signaturesByOrder.QfbSignature;
                var technicalSignature = signaturesByOrder?.TechnicalSignature == null ? qfbSignature : signaturesByOrder.TechnicalSignature;

                var model = new FinalizaGeneratePdfModel
                {
                    CreateDate = order.CreatedDate.ToString("dd/MM/yyyy"),
                    FabOrderId = order.OrdenId,
                    ItemCode = order.ProductoId,
                    OrderId = 0,
                    QfbName = qfbName,
                    QfbSignature = qfbSignature,
                    RecipeRoute = new List<string>(),
                    SaleOrderCreateDate = string.Empty,
                    TechnicalSignature = technicalSignature,
                    UserOrderId = userOrder.Id,
                    DesignerSignature = new byte[0],
                    DesignerName = string.Empty,
                    TechnicalName = tecnicalName,
                };

                listToReturn.Add(model);
            }

            return listToReturn;
        }

        /// <summary>
        /// Normalize the medic name.
        /// </summary>
        /// <param name="medicName">the medic.</param>
        /// <returns>the data.</returns>
        private static string NormalizeMedicName(string medicName)
        {
            medicName = medicName.Replace("*", string.Empty);
            medicName = medicName.Replace(":", string.Empty);
            medicName = medicName.Replace("/", string.Empty);
            medicName = medicName.Replace(@"\", string.Empty);
            return medicName;
        }

        /// <summary>
        /// Gets The recipes.
        /// </summary>
        /// <param name="orderIds">the orders id.</param>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <returns>the data.</returns>
        private static async Task<List<OrderRecipeModel>> GetRecipes(List<int> orderIds, ISapAdapter sapAdapter, string route)
        {
            var sapResponse = await sapAdapter.PostSapAdapter(orderIds, route);
            return JsonConvert.DeserializeObject<List<OrderRecipeModel>>(JsonConvert.SerializeObject(sapResponse.Response));
        }

        /// <summary>
        /// Gets The recipes.
        /// </summary>
        /// <param name="orderIds">the orders id.</param>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <returns>the data.</returns>
        private static async Task<List<OrderWithDetailModel>> GetDetails(List<int> orderIds, ISapAdapter sapAdapter, string route)
        {
            var sapResponse = await sapAdapter.PostSapAdapter(orderIds, route);
            return JsonConvert.DeserializeObject<List<OrderWithDetailModel>>(JsonConvert.SerializeObject(sapResponse.Response));
        }

        /// <summary>
        /// Gets the order fabs.
        /// </summary>
        /// <param name="fabOrdersId">the orders fab.</param>
        /// <param name="sapAdapter">the sap adapter.</param>
        /// <returns>the data.</returns>
        private static async Task<List<FabricacionOrderModel>> GetFabOrders(List<int> fabOrdersId, ISapAdapter sapAdapter)
        {
            var sapResponse = await sapAdapter.PostSapAdapter(fabOrdersId, ServiceConstants.GetUsersByOrdersById);
            return JsonConvert.DeserializeObject<List<FabricacionOrderModel>>(JsonConvert.SerializeObject(sapResponse.Response));
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <param name="userIds">the user id.</param>
        /// <param name="usersService">the service.</param>
        /// <returns>the data.</returns>
        private static async Task<List<UserModel>> GetUsers(List<string> userIds, IUsersService usersService)
        {
            var userResponse = await usersService.PostSimpleUsers(userIds, ServiceConstants.GetUsersById);
            return JsonConvert.DeserializeObject<List<UserModel>>(userResponse.Response.ToString());
        }
    }
}
