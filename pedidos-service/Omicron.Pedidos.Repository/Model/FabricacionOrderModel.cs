﻿// <summary>
// <copyright file="FabricacionOrderModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    using System;

    /// <summary>
    /// the fabricacion order.
    /// </summary>
    public class FabricacionOrderModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int OrdenId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int? PedidoId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public DateTime PostDate { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string ProductoId { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string DataSource { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public string ProdName { get; set; }

        /// <summary>
        /// Gets or sets measurement of unit.
        /// </summary>
        /// <value>The measurement of unit.</value>
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets.
        /// </summary>
        /// <value>
        /// Bool is sales order.
        public bool HasMissingStock { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsOmigenomics.
        /// </summary>
        /// <value>
        /// Bool IsOmigenomics.
        public bool IsOmigenomics { get; set; }
    }
}
