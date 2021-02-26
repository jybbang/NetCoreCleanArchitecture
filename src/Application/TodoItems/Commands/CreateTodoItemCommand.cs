using DaprCleanArchitecture.Application.Common.Interfaces;
using DaprCleanArchitecture.Application.Common.Repositories;
using DaprCleanArchitecture.Application.Common.Results;
using DaprCleanArchitecture.Application.Dtos;
using DaprCleanArchitecture.Domain.Entities;
using DaprCleanArchitecture.Domain.ValueObjects;
using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DaprCleanArchitecture.Application.TodoItems.Commands
{
    public record CreateTodoItemCommand : IRequest<Result<TodoItemDto>>
    {
        public Detail Detail { get; init; }
    }

    public class CreateTodoItemCommandValidator : AbstractValidator<CreateTodoItemCommand>
    {
        public CreateTodoItemCommandValidator()
        {
            RuleFor(v => v.Detail.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");
        }
    }

    public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, Result<TodoItemDto>>
    {
        private readonly IApplicationContext _context;
        private readonly IMapper _mapper;

        public CreateTodoItemCommandHandler(IApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<TodoItemDto>> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
        {
            var entity = new TodoItem
            {
                Detail = request.Detail
            };

            await _context.TodoItemCommands.AddAsync(entity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            var result = _mapper.Map<TodoItemDto>(entity);

            return Result<TodoItemDto>.Success(result);
        }
    }
}
