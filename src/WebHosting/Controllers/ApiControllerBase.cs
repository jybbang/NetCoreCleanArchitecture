using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Common.Exceptions;
using NetCoreCleanArchitecture.Application.Common.Results;
using System;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.WebHosting.Controllers
{
    [ApiController]
    [EnableCors]
    public abstract class ApiControllerBase : ControllerBase
    {
        private ISender _mediator;

        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    }
}
