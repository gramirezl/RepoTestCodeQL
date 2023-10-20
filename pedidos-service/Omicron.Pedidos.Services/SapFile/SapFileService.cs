﻿// <summary>
// <copyright file="SapFileService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.SapFile
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Services.Utils;
    using Serilog;

    /// <summary>
    /// Send to file service.
    /// </summary>
    public class SapFileService : ISapFileService
    {
        /// <summary>
        /// Client Http.
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapFileService"/> class.
        /// </summary>
        /// <param name="httpClient">Object to mapper.</param>
        /// <param name="logger">the logger.</param>
        public SapFileService(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        /// <summary>
        /// Sends to the sap file service.
        /// </summary>
        /// <param name="data">the data.</param>
        /// <param name="route">the route.</param>
        /// <returns>the response.</returns>
        public async Task<ResultModel> PostSimple(object data, string route)
        {
            ResultModel result;
            var stringContent = new StringContent(JsonConvert.SerializeObject(data), UnicodeEncoding.UTF8, "application/json");
            var url = this.httpClient.BaseAddress + route;

            using (var response = await this.httpClient.PostAsync(url, stringContent))
            {
                result = await ServiceShared.GetResponse(response, this.logger, "Error peticion sapfile service");
            }

            return result;
        }
    }
}
