// <summary>
// <copyright file="IDatabaseContext.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Entities.Context
{
    using Microsoft.EntityFrameworkCore;
    using Omicron.Catalogos.Entities.Model;

    /// <summary>
    /// Interface IDataBaseContext.
    /// </summary>
    public interface IDatabaseContext
    {
        /// <summary>
        /// Gets or sets CatUser.
        /// </summary>
        /// <value>
        /// Object UserModel CatUser.
        /// </value>
        DbSet<UserModel> CatUser { get; set; }

        /// <summary>
        /// Gets or sets CatUser.
        /// </summary>
        /// <value>
        /// Object UserModel CatUser.
        /// </value>
        DbSet<RoleModel> RoleModel { get; set; }

        /// <summary>
        /// Gets or sets parameters model.
        /// </summary>
        /// <value>
        /// Object parameters model.
        /// </value>
        DbSet<ParametersModel> ParametersModel { get; set; }

        /// <summary>
        /// Gets or sets Classification Qfb Model.
        /// </summary>
        /// <value>
        /// Object Classification Qfb Model.
        /// </value>
        DbSet<ClassificationQfbModel> ClassificationQfbModel { get; set; }
    }
}
