using DaprCleanArchitecture.Application.Common.Exceptions;
using DaprCleanArchitecture.Application.Common.Repositories;
using DaprCleanArchitecture.Application.Common.Results;
using DaprCleanArchitecture.Domain.Entities;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DaprCleanArchitecture.Application.TodoItems.Commands
{
    public record DeleteTodoItemCommand : IRequest<Result>
    {
        public Guid Id { get; init; }
    }

    public class DeleteTodoItemCommandValidator : AbstractValidator<DeleteTodoItemCommand>
    {
        public DeleteTodoItemCommandValidator()
        {
            RuleFor(v => v.Id)
                .NotEqual(Guid.Empty).WithMessage("Id is required.");
        }
    }

    public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand, Result>
    {
        private readonly IApplicationContext _context;

        public DeleteTodoItemCommandHandler(IApplicationContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.TodoItemQueries.GetAsync(request.Id, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(TodoItem), request.Id);
            }

            await _context.TodoItemCommands.DeleteAsync(request.Id, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
