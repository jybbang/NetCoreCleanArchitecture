using Dapr;
using DaprCleanArchitecture.Application.Common.Results;
using DaprCleanArchitecture.Application.TodoItems.Commands;
using DaprCleanArchitecture.Application.TodoItems.Queries;
using DaprCleanArchitecture.Domain.Events;
using DaprCleanArchitecture.Endpoint.Extensions;
using DaprCleanArchitecture.Infrastructure.DomainEventSources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DaprCleanArchitecture.Endpoint.Controllers
{
    public class TodoItemController : ApiControllerBase
    {
        private readonly ILogger<TodoItemController> _logger;

        public TodoItemController(ILogger<TodoItemController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetTodoItem([FromQuery] GetTodoItemQuery request)
        {
            var result = await Mediator.Send(request);

            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodoItem([FromBody] CreateTodoItemCommand request)
        {
            var result = await Mediator.Send(request);

            if (result.IsFailed) return result.ToActionResult();

            return CreatedAtAction(nameof(GetTodoItem), result.Data);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTodoItem([FromBody] UpdateTodoItemCommand request)
        {
            var result = await Mediator.Send(request);

            return result.ToActionResult();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTodoItem([FromBody] DeleteTodoItemCommand request)
        {
            var result = await Mediator.Send(request);

            return result.ToActionResult();
        }

        [NSwag.Annotations.OpenApiIgnore]
        [Topic(nameof(DomainEventSource), "TodoItem/TodoItemCompletedEvent")]
        public Task<IActionResult> TodoItemCompletedEvent([FromBody] TodoItemCompletedEvent request)
        {
            _logger.LogInformation("Received Domain Event via PubSub: {@DomainEvent}", request);

            return Result.Success().ToActionResultAsync();
        }
    }
}
