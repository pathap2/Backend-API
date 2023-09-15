using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Api.Context;
using TodoList.Api.DTOs;
using TodoList.Api.Entities;
using TodoList.Api.Repository;

namespace TodoList.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoItemsController : ControllerBase
    {
        private readonly IRepository<TodoItem, TodoItemDTO> _repository;
        private readonly ILogger<TodoItemsController> _logger;

        public TodoItemsController(IRepository<TodoItem, TodoItemDTO> repository, ILogger<TodoItemsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTodoItems()
        {
            try
            {
                var items = await _repository.GetAllAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all todo items.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoItem(Guid id)
        {
            try
            {
                var item = await _repository.GetByIdAsync(id);
                if (item == null)
                {
                    return NotFound();
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching todo item with id {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodoItem([FromBody] TodoItemDTO todoItemDTO)
        {
            try
            {
                // Check if a TodoItem with the same description already exists
                var existingItems = await _repository.GetAllAsync();
                if (existingItems.Any(item => item.Description == todoItemDTO.Description))
                {
                    return BadRequest("Description already exists");
                }

                // Validate description (Name)
                if (string.IsNullOrEmpty(todoItemDTO.Description))
                {
                    return BadRequest("Description is invalid");
                }

                await _repository.AddAsync(todoItemDTO);
                return CreatedAtAction(nameof(GetTodoItem), new { id = todoItemDTO.Id }, todoItemDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new todo item.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(Guid id, [FromBody] TodoItemDTO todoItemDTO)
        {
            if (id != todoItemDTO.Id)
            {
                return BadRequest("ID mismatch");
            }

            // Check if the todo item exists before updating
            var existingItem = await _repository.GetByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound();  // Return NotFound if the item doesn't exist
            }
            try
            {
                await _repository.UpdateAsync(todoItemDTO);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating todo item with id {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(Guid id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting todo item with id {id}.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}