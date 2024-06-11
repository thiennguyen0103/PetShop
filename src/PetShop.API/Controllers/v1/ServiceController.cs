using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PetShop.Application.Features.Services.Commands.CreateService;

namespace PetShop.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class ServiceController : BaseApiController
    {
        [HttpPost]
        public async Task<IActionResult> Post(CreateServiceCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
