﻿// <summary>
// <copyright file="IRedisService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Redis
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Class for redis.
    /// </summary>
    public interface IRedisService
    {
        /// <summary>
        /// Gets the redis key.
        /// </summary>
        /// <param name="key">the key.</param>
        /// <returns>the data.</returns>
        Task<string> GetRedisKey(string key);

        /// <summary>
        /// Writes to Reddis.
        /// </summary>
        /// <param name="key">the key.</param>
        /// <param name="value">the value.</param>
        /// <returns>the data.</returns>
        Task<bool> WriteToRedis(string key, string value);

        /// <summary>
        /// Writes to Reddis.
        /// </summary>
        /// <param name="key">the key.</param>
        /// <param name="value">the value.</param>
        /// <param name="timeToLive">Time to live.</param>
        /// <returns>the data.</returns>
        Task<bool> WriteToRedis(string key, string value, TimeSpan timeToLive);

        /// <summary>
        /// Deletes a key from redis.
        /// </summary>
        /// <param name="key">the key.</param>
        /// <returns>the data.</returns>
        Task<bool> DeleteKey(string key);
    }
}
