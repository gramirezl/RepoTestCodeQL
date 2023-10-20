﻿// <summary>
// <copyright file="OrderIdDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Dtos.Models
{
    /// <summary>
    /// class for the order operation.
    /// </summary>
    public class OrderIdDto
    {
        /// <summary>
        /// Gets or sets user id.
        /// </summary>
        /// <value>The code.</value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets order indentifier.
        /// </summary>
        /// <value>The order id.</value>
        public int OrderId { get; set; }
    }
}
