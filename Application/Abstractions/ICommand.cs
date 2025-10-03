using MediatR;

namespace Application.Abstractions;

public interface ICommand : IRequest, IBaseCommand
{
}

public interface ICommand<TResult> : IRequest<Result<TResult>>, IBaseCommand
{
}

public interface IBaseCommand
{
}