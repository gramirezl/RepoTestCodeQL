﻿// <summary>
// <copyright file="QfbOrderModel.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// the QfbOrderModel.
    /// </summary>
    public class QfbOrderModel
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public List<QfbOrderDetail> Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether require technical.
        /// </summary>
        /// <value>Require Technical.</value>
        public bool RequireTechnical { get; set; }
    }
}
