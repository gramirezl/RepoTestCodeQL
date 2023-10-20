﻿// <summary>
// <copyright file="RedisService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Redis
{
    using System;
    using System.Threading.Tasks;
    using StackExchange.Redis;

    /// <summary>
    /// Get the redis data.
    /// </summary>
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer redis;

        private readonly IDatabase database;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisService"/> class.
        /// </summary>
        /// <param name="redis">Redis Cache.</param>
        public RedisService(IConnectionMultiplexer redis)
        {
            this.redis = redis ?? throw new ArgumentNullException(nameof(redis));
            this.database = redis.GetDatabase();
        }

        /// <inheritdoc/>
        public async Task<string> GetRedisKey(string key)
        {
            var result = await this.database.StringGetAsync(key);
            return result.HasValue ? result.ToString() : string.Empty;
        }

        /// <inheritdoc/>
        public async Task<bool> WriteToRedis(string key, string value)
        {
            await this.database.KeyDeleteAsync(key);
            await this.database.StringSetAsync(key, value);
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> WriteToRedis(string key, string value, TimeSpan timeToLive)
        {
            await this.database.KeyDeleteAsync(key);
            await this.database.StringSetAsync(key, value, timeToLive);
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteKey(string key)
        {
            await this.database.KeyDeleteAsync(key);
            return true;
        }
    }
}
