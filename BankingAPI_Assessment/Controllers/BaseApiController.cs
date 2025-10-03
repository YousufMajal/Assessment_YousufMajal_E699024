using Application.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BankingAPI.Controllers
{
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected readonly ISender sender;

        protected BaseApiController(ISender sender) => this.sender = sender;

        // Handles failure results and returns appropriate HTTP responses.
        protected IActionResult HandleFailure(Result result) =>
            result switch
            {
                { IsSuccess: true } => throw new InvalidOperationException(),
                IValidationResult validationResult =>
                    BadRequest(
                        CreateProblemDetails(
                            "Validation Error", StatusCodes.Status400BadRequest,
                            result.Error,
                            validationResult.Errors)),
                _ =>
                    BadRequest(
                        CreateProblemDetails("Bad Request", StatusCodes.Status400BadRequest, result.Error)
                        )
            };

        private static ProblemDetails CreateProblemDetails(
            string title,
            int status,
            Error error,
            Error[]? errors = null) =>
            new()
            {
                Title = title,
                Status = status,
                Detail = error.Description,
                Extensions = { { nameof(errors), errors } }
            };
    }
}