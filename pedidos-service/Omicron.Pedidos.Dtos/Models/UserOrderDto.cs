﻿// <summary>
// <copyright file="UserOrderDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Dtos.Models
{
    using System;

    /// <summary>
    /// Class for the user dto.
    /// </summary>
    public class UserOrderDto
    {
        /// <summary>
        /// Gets or sets Id.
        /// </summary>
        /// <value>
        /// Int Id.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public string Userid { get; set; }

        /// <summary>
        /// Gets or sets salesorderid.
        /// </summary>
        /// <value>
        /// String salesorderid.
        /// </value>
        public string Salesorderid { get; set; }

        /// <summary>
        /// Gets or sets productionorderid.
        /// </summary>
        /// <value>
        /// String productionorderid.
        public string Productionorderid { get; set; }

        /// <summary>
        /// Gets or sets status.
        /// </summary>
        /// <value>
        /// String status.
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets comments.
        /// </summary>
        /// <value>
        /// String comments.
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets close user id.
        /// </summary>
        /// <value>
        /// String user id.
        public string StatusAlmacen { get; set; }

        /// <summary>
        /// Gets or sets close user id.
        /// </summary>
        /// <value>
        /// String user id.
        public string UserCheckIn { get; set; }

        /// <summary>
        /// Gets or sets close user id.
        /// </summary>
        /// <value>
        /// String user id.
        public DateTime? DateTimeCheckIn { get; set; }

        /// <summary>
        /// Gets or sets close user id.
        /// </summary>
        /// <value>
        /// String user id.
        public string RemisionQr { get; set; }

        /// <summary>
        /// Gets or sets close user id.
        /// </summary>
        /// <value>
        /// String user id.
        public int DeliveryId { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public string UserInvoiceStored { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public DateTime? InvoiceStoreDate { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public string InvoiceQr { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public int InvoiceId { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public string StatusInvoice { get; set; }

        /// <summary>
        /// Gets or sets userid.
        /// </summary>
        /// <value>
        /// Datetime userid.
        /// </value>
        public string InvoiceType { get; set; }
    }
}
