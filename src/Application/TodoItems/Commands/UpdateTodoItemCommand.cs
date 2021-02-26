using DaprCleanArchitecture.Application.Common.Exceptions;
using DaprCleanArchitecture.Application.Common.Repositories;
using DaprCleanArchitecture.Application.Common.Results;
using DaprCleanArchitecture.Domain.Entities;
using DaprCleanArchitecture.Domain.ValueObjects;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DaprCleanArchitecture.Application.TodoItems.Commands
{
    public record UpdateTodoItemCommand : IRequest<Result>
    {
        public Guid Id { get; init; }

        public Detail Detail { get; init; }

        public bool Done { get; init; }
    }

    public class UpdateTodoItemCommandValidator : AbstractValidator<UpdateTodoItemCommand>
    {
        public UpdateTodoItemCommandValidator()
        {
            RuleFor(v => v.Id)
                .NotEqual(Guid.Empty).WithMessage("Id is required.");

            RuleFor(v => v.Detail.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");
        }
    }

    public class UpdateTodoItemCommandHandler : IRequestHandler<UpdateTodoItemCommand, Result>
    {
        private readonly IApplicationContext _context;

        public UpdateTodoItemCommandHandler(IApplicationContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.TodoItemQueries.GetAsync(request.Id, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(TodoItem), request.Id);
            }

            entity.Detail = request.Detail;
            entity.Done = request.Done;

            await _context.TodoItemCommands.UpdateAsync(request.Id, entity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
