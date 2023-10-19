// <summary>
// <copyright file="BaseTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Test
{
    using System;
    using System.Collections.Generic;
    using Omicron.Catalogos.Dtos.User;
    using Omicron.Catalogos.Entities.Model;

    /// <summary>
    /// Class Base Test.
    /// </summary>
    public abstract class BaseTest
    {
        /// <summary>
        /// List of Users.
        /// </summary>
        /// <returns>IEnumerable Users.</returns>
        public IEnumerable<UserModel> GetAllUsers()
        {
            return new List<UserModel>()
            {
                new UserModel { Id = 1, FirstName = "Alejandro", LastName = "Ojeda", Email = "alejandro.ojeda@axity.com", Birthdate = DateTime.Now },
                new UserModel { Id = 2, FirstName = "Jorge", LastName = "Morales", Email = "jorge.morales@axity.com", Birthdate = DateTime.Now },
                new UserModel { Id = 3, FirstName = "Arturo", LastName = "Miranda", Email = "arturo.miranda@axity.com", Birthdate = DateTime.Now },
                new UserModel { Id = 4, FirstName = "Benjamin", LastName = "Galindo", Email = "benjamin.galindo@axity.com", Birthdate = DateTime.Now },
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public UserDto GetUserDto()
        {
            return new UserDto
            {
                Id = 10,
                FirstName = "Jorge",
                LastName = "Morales",
                Email = "test@test.com",
                Birthdate = DateTime.Now,
            };
        }

        /// <summary>
        /// Gets all the roles.
        /// </summary>
        /// <returns>the roles.</returns>
        public IEnumerable<RoleModel> GetListRoles()
        {
            return new List<RoleModel>
            {
                new RoleModel { Id = 1, Description = "Administrador" },
                new RoleModel { Id = 2, Description = "QFB" },
            };
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <returns>the parameters.</returns>
        public IEnumerable<ParametersModel> GetParameters()
        {
            return new List<ParametersModel>
            {
                new ParametersModel { Id = 1, Field = "Email", Type = "string", Value = "email" },
            };
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <returns>the parameters.</returns>
        public IEnumerable<ClassificationQfbModel> GetActiveClassificationQfbModel()
        {
            return new List<ClassificationQfbModel>
            {
                new ClassificationQfbModel { Id = 1, Value = "MN", Description = "Bioelite (MN)", Active = true },
                new ClassificationQfbModel { Id = 2, Value = "BE", Description = "Bioequal (BE)", Active = true },
                new ClassificationQfbModel { Id = 3, Value = "MG", Description = "Magistrales (MG)", Active = true },
                new ClassificationQfbModel { Id = 4, Value = "DZ", Description = "Dermazone (DZ)", Active = true },
                new ClassificationQfbModel { Id = 5, Value = "O", Description = "OTRO (O)", Active = false },
            };
        }
    }
}
