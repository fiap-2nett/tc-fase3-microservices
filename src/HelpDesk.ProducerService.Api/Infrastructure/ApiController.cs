using HelpDesk.Core.Domain.Primitives;
using HelpDesk.ProducerService.Api.Constants;
using HelpDesk.ProducerService.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.ProducerService.Api.Infrastructure
{
    [Authorize]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class ApiController : ControllerBase
    {
        #region Methods

        protected new IActionResult Ok(object value)
            => base.Ok(value);

        protected IActionResult BadRequest(Error error)
            => BadRequest(new ApiErrorResponse(error));

        protected new IActionResult NotFound()
            => NotFound(Errors.NotFoudError.Message);

        #endregion
    }
}
