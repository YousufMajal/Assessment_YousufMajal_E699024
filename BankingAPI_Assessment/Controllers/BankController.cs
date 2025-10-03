using Application.Abstractions;
using Application.CQRS.Commands.Withdraw;
using Application.DTOs;
using BankingAPI.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BankingAPI.Controllers
{
    [Route("api/[controller]")]
    public class BankController : BaseApiController
    {
        public BankController(ISender sender) : base(sender)
        {
        }

        /// Withdraw funds from the specified account.
        [HttpPost("accounts/{accountId}/withdraw")]
        [ProducesResponseType(typeof(Result<WithdrawalResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Withdraw(string accountId, [FromBody] WithdrawRequest request, CancellationToken ct)
        {
            // Validate and convert accountId to GUID
            if (!Guid.TryParse(accountId, out var accountGuid))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid Account ID",
                    Detail = "The provided account ID is not a valid GUID format.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var command = new WithdrawFundsCommand(accountGuid, request.Amount);
            var result = await sender.Send(command, ct);
            return result.IsSuccess ? Ok(result) : HandleFailure(result);
        }
    }
}