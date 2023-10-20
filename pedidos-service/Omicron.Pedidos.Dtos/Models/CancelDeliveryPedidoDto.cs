﻿// <summary>
// <copyright file="CancelDeliveryPedidoDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Dtos.Models
{
    /// <summary>
    /// clas for DTO.
    /// </summary>
    public class CancelDeliveryPedidoDto
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int SaleOrderId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int DeliveryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public bool NeedsCancel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Status { get; set; }
    }
}
