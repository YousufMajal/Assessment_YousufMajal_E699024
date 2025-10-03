using MediatR;

namespace Application.Abstractions;

internal interface IQuery<TResult> : IRequest<Result<TResult>>
{
}