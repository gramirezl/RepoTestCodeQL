﻿// <summary>
// <copyright file="PedidoFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Facade.Pedidos
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Omicron.Pedidos.Dtos.Models;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;
    using Omicron.Pedidos.Resources.Enums;
    using Omicron.Pedidos.Services.Pedidos;

    /// <summary>
    /// the pedidos facade.
    /// </summary>
    public class PedidoFacade : IPedidoFacade
    {
        /// <summary>
        /// Mapper Object.
        /// </summary>
        private readonly IMapper mapper;

        private readonly IPedidosService pedidoService;

        private readonly IAssignPedidosService assignPedidosService;

        private readonly ICancelPedidosService cancelPedidosService;

        private readonly IProductivityService productivityService;

        private readonly IFormulaPedidosService formulaPedidosService;

        private readonly IProcessOrdersService processOrdersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PedidoFacade"/> class.
        /// </summary>
        /// <param name="pedidoService">the pedido service.</param>
        /// <param name="mapper">the mapper.</param>
        /// <param name="assignPedidosService">The assign pedidos service.</param>
        /// <param name="productivityService">The productivity services.</param>
        /// <param name="cancelPedidosService">The cancel pedidos service.</param>
        /// <param name="formulaPedidosService">The formula pedidos service.</param>
        /// <param name="processOrdersService">Proces Order service.</param>
        public PedidoFacade(IPedidosService pedidoService, IMapper mapper, IAssignPedidosService assignPedidosService, ICancelPedidosService cancelPedidosService, IProductivityService productivityService, IFormulaPedidosService formulaPedidosService, IProcessOrdersService processOrdersService)
        {
            this.pedidoService = pedidoService ?? throw new ArgumentNullException(nameof(pedidoService));
            this.assignPedidosService = assignPedidosService ?? throw new ArgumentNullException(nameof(assignPedidosService));
            this.cancelPedidosService = cancelPedidosService ?? throw new ArgumentNullException(nameof(cancelPedidosService));
            this.productivityService = productivityService ?? throw new ArgumentNullException(nameof(productivityService));
            this.formulaPedidosService = formulaPedidosService ?? throw new ArgumentNullException(nameof(formulaPedidosService));
            this.processOrdersService = processOrdersService ?? throw new ArgumentNullException(nameof(processOrdersService));
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<ResultDto> ProcessOrders(ProcessOrderDto orderDto)
        {
            return this.mapper.Map<ResultDto>(await this.processOrdersService.ProcessOrders(this.mapper.Map<ProcessOrderModel>(orderDto)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetUserOrderBySalesOrder(List<int> listIds)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.GetUserOrderBySalesOrder(listIds));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetUserOrderByFabOrder(List<int> listIds)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.GetUserOrderByFabOrder(listIds));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetFabOrderByUserID(string userId)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.GetFabOrderByUserId(userId));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetUserOrdersByUserId(List<string> listIds)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.GetUserOrdersByUserId(listIds));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> AssignHeader(ManualAssignDto manualAssign)
        {
            return this.mapper.Map<ResultDto>(await this.assignPedidosService.AssignOrder(this.mapper.Map<ManualAssignModel>(manualAssign)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateComponents(UpdateFormulaDto updateFormula)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.UpdateComponents(this.mapper.Map<UpdateFormulaModel>(updateFormula)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateStatusOrder(List<UpdateStatusOrderDto> updateStatus)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.UpdateStatusOrder(this.mapper.Map<List<UpdateStatusOrderModel>>(updateStatus)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateFabOrderComments(List<UpdateOrderCommentsDto> updateComments)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.UpdateFabOrderComments(this.mapper.Map<List<UpdateOrderCommentsModel>>(updateComments)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> ConnectDiApi()
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.ConnectDiApi());
        }

        /// <inheritdoc/>
        public async Task<ResultDto> ProcessByOrder(ProcessByOrderDto processByOrder)
        {
            return this.mapper.Map<ResultDto>(await this.processOrdersService.ProcessByOrder(this.mapper.Map<ProcessByOrderModel>(processByOrder)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CancelOrder(List<OrderIdDto> cancelOrders)
        {
            return this.mapper.Map<ResultDto>(await this.cancelPedidosService.CancelSalesOrder(this.mapper.Map<List<OrderIdModel>>(cancelOrders)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CloseSalesOrders(List<OrderIdDto> finishOrders)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.CloseSalesOrders(this.mapper.Map<List<OrderIdModel>>(finishOrders)));
        }

        /// <summary>
        /// reject order (status to reject).
        /// </summary>
        /// <param name="rejectOrders">Orders to reject.</param>
        /// <returns>Order with updated info.</returns>
        public async Task<ResultDto> RejectSalesOrders(RejectOrdersDto rejectOrders)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.RejectSalesOrders(this.mapper.Map<RejectOrdersModel>(rejectOrders)));
        }

        /// <summary>
        /// reject order (status to reject).
        /// </summary>
        /// <param name="status">status.</param>}
        /// <param name="userId">userId.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetQfbOrdersByStatus(string status, string userId)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.GetQfbOrdersByStatus(status, userId));
        }

        /// <summary>
        /// Cancel fabrication orders.
        /// </summary>
        /// <param name="cancelOrders">Orders to cancel.</para
        /// <returns>Orders with updated info.</returns>urns>
        public async Task<ResultDto> CancelFabOrder(List<OrderIdDto> cancelOrders)
        {
            return this.mapper.Map<ResultDto>(await this.cancelPedidosService.CancelFabricationOrders(this.mapper.Map<List<OrderIdModel>>(cancelOrders)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CloseFabOrders(List<CloseProductionOrderDto> finishOrders)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.CloseFabOrders(this.mapper.Map<List<CloseProductionOrderModel>>(finishOrders)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> AutomaticAssign(AutomaticAssingDto automaticAssing)
        {
            return this.mapper.Map<ResultDto>(await this.assignPedidosService.AutomaticAssign(this.mapper.Map<AutomaticAssingModel>(automaticAssing)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateBatches(List<AssignBatchDto> assignBatch)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.UpdateBatches(this.mapper.Map<List<AssignBatchModel>>(assignBatch)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateOrderSignature(SignatureType signatureType, UpdateOrderSignatureDto signatureModel)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.UpdateOrderSignature(signatureType, this.mapper.Map<UpdateOrderSignatureModel>(signatureModel)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetOrderSignatures(int productionOrderId)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.GetOrderSignatures(productionOrderId));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> FinishOrder(FinishOrderDto updateOrderSignature)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.FinishOrder(this.mapper.Map<FinishOrderModel>(updateOrderSignature)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CreateIsolatedProductionOrder(CreateIsolatedFabOrderDto isolatedFabOrder)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.CreateIsolatedProductionOrder(this.mapper.Map<CreateIsolatedFabOrderModel>(isolatedFabOrder)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetFabOrders(Dictionary<string, string> parameters)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.GetFabOrders(parameters));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> ReassignOrder(ManualAssignDto manualAssign)
        {
            return this.mapper.Map<ResultDto>(await this.assignPedidosService.ReassignOrder(this.mapper.Map<ManualAssignModel>(manualAssign)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetProductivityData(Dictionary<string, string> parameters)
        {
            return this.mapper.Map<ResultDto>(await this.productivityService.GetProductivityData(parameters));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CreateCustomComponentList(string userId, CustomComponentListDto customList)
        {
            return this.mapper.Map<ResultDto>(await this.formulaPedidosService.CreateCustomComponentList(userId, this.mapper.Map<CustomComponentListModel>(customList)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetCustomComponentListByProductId(string productId)
        {
            return this.mapper.Map<ResultDto>(await this.formulaPedidosService.GetCustomComponentListByProductId(productId));
        }

        /// <summary>
        /// Delete custom component list.
        /// </summary>
        /// <param name="parameters">The user id.</param>
        /// <returns>New custom list.</returns>
        public async Task<ResultDto> DeleteCustomComponentList(Dictionary<string, string> parameters)
        {
            return this.mapper.Map<ResultDto>(await this.formulaPedidosService.DeleteCustomComponentList(parameters));
        }

        /// <summary>
        /// Gets the workload.
        /// </summary>
        /// <param name="parameters">the filters.</param>
        /// <returns>the data.</returns>
        public async Task<ResultDto> GetWorkLoad(Dictionary<string, string> parameters)
        {
            return this.mapper.Map<ResultDto>(await this.productivityService.GetWorkLoad(parameters));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CompletedBatches(int orderId)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.CompletedBatches(orderId));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> PrintOrders(List<int> ordersId)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.PrintOrders(ordersId));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateSaleOrders(UpdateOrderCommentsDto updateSaleOrder)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.UpdateSaleOrders(this.mapper.Map<UpdateOrderCommentsModel>(updateSaleOrder)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> UpdateDesignerLabel(UpdateDesignerLabelDto updateDesignerLabel)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.UpdateDesignerLabel(this.mapper.Map<UpdateDesignerLabelModel>(updateDesignerLabel)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> CreateSaleOrderPdf(List<int> orderIds)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.CreateSaleOrderPdf(orderIds));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> DeleteFiles()
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.DeleteFiles());
        }

        /// <inheritdoc/>
        public async Task<ResultDto> SignOrdersByTecnic(FinishOrderDto tecnicOrderSignature)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.SignOrdersByTecnic(this.mapper.Map<FinishOrderModel>(tecnicOrderSignature)));
        }

        /// <inheritdoc/>
        public async Task<ResultDto> GetInvalidOrdersByMissingTecnicSign(List<string> productionOrderIds)
        {
            return this.mapper.Map<ResultDto>(await this.pedidoService.GetInvalidOrdersByMissingTecnicSign(productionOrderIds));
        }
    }
}
