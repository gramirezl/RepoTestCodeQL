﻿// <summary>
// <copyright file="GetFabOrderUtils.cs" company="Axity">
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
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;

    /// <summary>
    /// The class for the get orders.
    /// </summary>
    public static class GetFabOrderUtils
    {
        /// <summary>
        /// gets the data by the filters of status, qfb, end fate.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <param name="pedidosDao">the pedidos dao.</param>
        /// <returns>the data.</returns>
        public static async Task<List<UserOrderModel>> GetOrdersByFilter(Dictionary<string, string> parameters, IPedidosDao pedidosDao)
        {
            if (parameters.ContainsKey(ServiceConstants.DocNum))
            {
                var valueSplit = parameters[ServiceConstants.DocNum].Split("-");

                int.TryParse(valueSplit[0], out int docNumInit);
                int.TryParse(valueSplit[1], out int docNumEnd);
                docNumEnd += 1;
                var listIdString = new List<string>();
                Enumerable.Range(docNumInit, docNumEnd - docNumInit).ToList().ForEach(x => listIdString.Add(x.ToString()));

                return (await pedidosDao.GetUserOrderByProducionOrder(new List<string> { parameters[ServiceConstants.DocNum] })).ToList();
            }

            var listOrders = new List<UserOrderModel>();
            var filterQfb = parameters.ContainsKey(ServiceConstants.Qfb);
            var filterFechaFin = parameters.ContainsKey(ServiceConstants.FechaFin);
            var filterFechaIni = parameters.ContainsKey(ServiceConstants.FechaInicio);
            var filterStatus = parameters.ContainsKey(ServiceConstants.Status);

            if (filterQfb)
            {
                listOrders.AddRange((await pedidosDao.GetUserOrderByUserId(new List<string> { parameters[ServiceConstants.Qfb] })).ToList());
            }

            if (filterStatus)
            {
                var status = parameters[ServiceConstants.Status].ToLower() == ServiceConstants.ProcesoStatus.ToLower() ? ServiceConstants.Proceso : parameters[ServiceConstants.Status];
                listOrders = ServiceShared.CalculateTernary(filterQfb, listOrders.Where(x => x.Status.Equals(status)).ToList(), (await pedidosDao.GetUserOrderByStatus(new List<string> { status })).ToList());
            }

            if (filterFechaFin || filterFechaIni)
            {
                listOrders = await GetOrdersFilteredByDate(parameters, filterQfb || filterStatus, listOrders, pedidosDao);
            }

            return listOrders.DistinctBy(x => x.Id).ToList();
        }

        /// <summary>
        /// Creates the model to return.
        /// </summary>
        /// <param name="fabOrderModel">the order.</param>
        /// <param name="userOrders">the user order.</param>
        /// <param name="users">the user.</param>
        /// <returns>the data.</returns>
        public static List<CompleteOrderModel> CreateModels(List<FabricacionOrderModel> fabOrderModel, List<UserOrderModel> userOrders, List<UserModel> users)
        {
            var listToReturn = new List<CompleteOrderModel>();

            fabOrderModel.ForEach(x =>
            {
                var userOrder = userOrders.FirstOrDefault(y => y.Productionorderid.Equals(x.OrdenId.ToString()));
                userOrder ??= new UserOrderModel();
                var status = userOrder.Status ?? ServiceConstants.Abierto;

                var user = users.FirstOrDefault(y => y.Id.Equals(userOrder.Userid));

                var fabOrder = new CompleteOrderModel
                {
                    DocNum = !x.PedidoId.HasValue ? 0 : x.PedidoId.Value,
                    FabOrderId = x.OrdenId,
                    ItemCode = x.ProductoId,
                    Description = x.ProdName,
                    Quantity = x.Quantity,
                    CreateDate = x.CreatedDate.ToString("dd/MM/yyyy"),
                    FinishDate = userOrder.FinishDate.HasValue ? userOrder.FinishDate.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Status = ServiceShared.CalculateTernary(status.Equals(ServiceConstants.Proceso), ServiceConstants.ProcesoStatus, status),
                    Qfb = user == null ? string.Empty : $"{user.FirstName} {user.LastName}",
                    Unit = x.Unit,
                    HasMissingStock = x.HasMissingStock,
                    Batch = userOrder.BatchFinalized,
                };

                listToReturn.Add(fabOrder);
            });

            return listToReturn;
        }

        /// <summary>
        /// Get the data filtered by date.
        /// </summary>
        /// <param name="parameters">the original dict.</param>
        /// <param name="dataFiltered">if there are other filtesr.</param>
        /// <param name="listOrders">the data already filtered.</param>
        /// <param name="pedidosDao">the dao.</param>
        /// <returns>the data.</returns>
        private static async Task<List<UserOrderModel>> GetOrdersFilteredByDate(Dictionary<string, string> parameters, bool dataFiltered, List<UserOrderModel> listOrders, IPedidosDao pedidosDao)
        {
            var dateFilter = ServiceUtils.GetDateFilter(parameters);

            var endDate = dateFilter[ServiceConstants.FechaFin].AddHours(23).AddMinutes(59);

            if (dataFiltered)
            {
                return ServiceShared.CalculateTernary(
                    parameters.ContainsKey(ServiceConstants.FechaFin),
                    listOrders.Where(y => y.FinishDate != null && y.FinishDate >= dateFilter[ServiceConstants.FechaInicio] && y.FinishDate <= endDate).ToList(),
                    listOrders.Where(y => y.PlanningDate != null && y.PlanningDate >= dateFilter[ServiceConstants.FechaInicio] && y.PlanningDate <= endDate).ToList());
            }
            else
            {
                return ServiceShared.CalculateTernary(
                    parameters.ContainsKey(ServiceConstants.FechaFin),
                    (await pedidosDao.GetUserOrderByFechaFin(dateFilter[ServiceConstants.FechaInicio], endDate)).ToList(),
                    (await pedidosDao.GetUserOrderByPlanningDate(dateFilter[ServiceConstants.FechaInicio], endDate)).ToList());
            }
        }
    }
}
