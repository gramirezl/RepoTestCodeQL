﻿// <summary>
// <copyright file="PedidosDxpService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Pedidos
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Omicron.Pedidos.DataAccess.DAO.Pedidos;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Constants;
    using Omicron.Pedidos.Services.Utils;

    /// <summary>
    /// the pedidos service.
    /// </summary>
    public class PedidosDxpService : IPedidosDxpService
    {
        private readonly IPedidosDao pedidosDao;

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidosDxpService"/> class.
        /// </summary>
        /// <param name="pedidosDao">pedidos dao.</param>
        public PedidosDxpService(IPedidosDao pedidosDao)
        {
            this.pedidosDao = pedidosDao.ThrowIfNull(nameof(pedidosDao));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersActive(List<int> ordersid)
        {
            var listIds = ordersid.Select(x => x.ToString()).ToList();
            var orders = (await this.pedidosDao.GetUserOrderBySaleOrder(listIds)).ToList();
            var listToReturn = orders.Select(x => new
            {
                x.Id,
                x.Salesorderid,
                x.Productionorderid,
                x.Status,
                x.StatusAlmacen,
                x.StatusInvoice,
                x.IsSalesOrder,
                x.InvoiceStoreDate,
                x.InvoiceId,
                x.PlanningDate,
                x.InvoiceType,
            }).ToList();
            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetDeliveredPayments(List<int> ordersId)
        {
            var listIds = ordersId.Select(x => x.ToString()).ToList();
            var salesOrdersGroups = (await this.pedidosDao.GetUserOrderBySaleOrder(listIds)).GroupBy(x => x.Salesorderid).ToList();
            var listSent = new List<UserOrderModel>();
            salesOrdersGroups.ForEach(x =>
            {
                if (x.Any(y => y.StatusInvoice == ServiceConstants.Entregado || y.StatusInvoice == ServiceConstants.Enviado))
                {
                    var orderSent = x.Where(y => y.StatusInvoice == ServiceConstants.Entregado || y.StatusInvoice == ServiceConstants.Enviado);
                    var order = orderSent.FirstOrDefault(x => x.IsProductionOrder);
                    order ??= new UserOrderModel();
                    listSent.Add(order);
                }
            });

            var listToReturn = listSent.Select(x => new
            {
                x.Salesorderid,
                x.StatusInvoice,
                x.InvoiceId,
                x.InvoiceType,
            });

            return ServiceUtils.CreateResult(true, 200, null, listToReturn, null, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> GetOrdersHeaderStatus(List<string> orders)
        {
            var userOrders = (await this.pedidosDao.GetUserOrderBySaleOrder(orders)).Select(o => new
            {
                o.Salesorderid,
                o.Productionorderid,
                o.Status,
                o.StatusAlmacen,
                o.StatusInvoice,
            }).ToList();
            userOrders = userOrders.Where(o => string.IsNullOrEmpty(o.Productionorderid)).ToList();
            return ServiceUtils.CreateResult(true, 200, null, userOrders, null, userOrders.Count.ToString());
        }
    }
}
