using MediatR;

namespace Application.Abstractions;

internal interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, Result<TResult>>
       where TQuery : IQuery<TResult>
{
}