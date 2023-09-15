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
        /// <summary>
        /// This is a get request where we are trying to Fetch Item from the list via existing GUID in a list
        /// </summary>
        /// <param name="id"></param>
        /// <returns>It will return with the specific item from the list</returns>
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
        /// <summary>
        /// This will allow to ADD the Item in the list via HTTP post request
        /// <Validation>
        /// It will handle the bad request if the Item already exists.
        /// It will handle if the item we are trying to post with empty list
        /// </Validation>
        /// </summary>
        /// <param name="todoItemDTO"></param>
        /// <returns></returns>
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
        /// <summary>
        /// This will allow to update the TODO Item list via GUID provided.
        /// </summary>
        /// <Validation>
        /// It provide with user friendly message if someone tring to update the TODO item visa wrong ID whcich is not exists in a list
        /// It will handle if the item we are trying to post with empty list
        /// </Validation>
        /// <param name="id"></param>
        /// <param name="todoItemDTO"></param>
        /// <returns></returns>
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
       
        /// <summary>
        /// This will allow to delete the TODO Item list via GUID provided.
        /// </summary>
        /// <Validation>
        /// It provide with user friendly message if someone tring to elete the TODO item visa wrong ID whcich is not exists in a list
        /// </Validation>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(Guid id)
        {
            try
            {
                var existingItem = await _repository.GetByIdAsync(id);
                if (existingItem == null)
                {
                    return BadRequest("ID not found");  // Returns ID not found use
                }

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