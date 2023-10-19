// <summary>
// <copyright file="AutoMapperProfile.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Services.Mapping
{
    using AutoMapper;
    using Omicron.Catalogos.Dtos.Models;
    using Omicron.Catalogos.Dtos.User;
    using Omicron.Catalogos.Entities.Model;

    /// <summary>
    /// Class Automapper.
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperProfile"/> class.
        /// </summary>
        public AutoMapperProfile()
        {
            this.CreateMap<UserModel, UserDto>();
            this.CreateMap<UserDto, UserModel>();
            this.CreateMap<ResultModel, ResultDto>();
            this.CreateMap<ResultDto, ResultModel>();
        }
    }
}