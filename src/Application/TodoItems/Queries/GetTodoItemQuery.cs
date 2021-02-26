using DaprCleanArchitecture.Application.Common.Interfaces;
using DaprCleanArchitecture.Application.Common.Repositories;
using DaprCleanArchitecture.Application.Common.Results;
using DaprCleanArchitecture.Application.Dtos;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DaprCleanArchitecture.Application.TodoItems.Queries
{
    public record GetTodoItemQuery : IRequest<Result<TodoItemDto>>
    {
        public Guid Id { get; init; }
    }

    public class GetTodoItemQueryValidator : AbstractValidator<GetTodoItemQuery>
    {
        public GetTodoItemQueryValidator()
        {
            RuleFor(v => v.Id)
                .NotEqual(Guid.Empty).WithMessage("Id is required.");
        }
    }

    public class GetTodoItemQueryHandler : IRequestHandler<GetTodoItemQuery, Result<TodoItemDto>>
    {
        private readonly IApplicationContext _context;
        private readonly IMapper _mapper;

        public GetTodoItemQueryHandler(IApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<TodoItemDto>> Handle(GetTodoItemQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.TodoItemQueries.GetAsync(request.Id, cancellationToken);

            var result = _mapper.Map<TodoItemDto>(entity);

            return Result<TodoItemDto>.Success(result);
        }
    }
}
