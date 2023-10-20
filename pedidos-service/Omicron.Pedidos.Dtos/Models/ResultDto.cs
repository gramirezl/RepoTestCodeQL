﻿// <summary>
// <copyright file="ResultDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Dtos.Models
{
    /// <summary>
    /// The class for the ResultDto.
    /// </summary>
    public class ResultDto
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        /// <value>The code.</value>
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets User Error.
        /// </summary>
        /// <value>The user error.</value>
        public string UserError { get; set; }

        /// <summary>
        /// Gets or sets ErrorMessage.
        /// </summary>
        /// <value>The exception message.</value>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Success.
        /// </summary>
        /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets Response.
        /// </summary>
        /// <value>The response.</value>
        public object Response { get; set; }

        /// <summary>
        /// Gets or sets Response.
        /// </summary>
        /// <value>The response.</value>
        public object Comments { get; set; }
    }
}
