using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TodoList.Api.Controllers;
using TodoList.Api.DTOs;
using TodoList.Api.Entities;
using TodoList.Api.Repository;

using Xunit;

namespace TodoList.Api.Tests
{
    public class TodoItemsControllerTests
    {
        private readonly TodoItemsController _controller;
        private readonly Mock<IRepository<TodoItem, TodoItemDTO>> _repositoryMock;
        private readonly Mock<ILogger<TodoItemsController>> _loggerMock;

        public TodoItemsControllerTests()
        {
            _repositoryMock = new Mock<IRepository<TodoItem, TodoItemDTO>>();
            _loggerMock = new Mock<ILogger<TodoItemsController>>();
            _controller = new TodoItemsController(_repositoryMock.Object, _loggerMock.Object);
        }

        /// <summary>
        ///  test case for whethere all TODO list return results
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetTodoItems_ReturnsOk()
        {
            _repositoryMock.Setup(repo => repo.GetAllAsync(null))
                           .ReturnsAsync(new List<TodoItemDTO>());
            var result = await _controller.GetAllTodoItems();
            Assert.IsType<OkObjectResult>(result);
        }
        /// <summary>
        /// This test case is to test whether perticular TODO Item returns from the list via API if it exists
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetTodoItem_ReturnsOk_WhenTodoExists()
        {
            var todoItemDTO = new TodoItemDTO { Id = Guid.NewGuid(), Description = "Test" };
            _repositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                           .ReturnsAsync(todoItemDTO);
            var result = await _controller.GetTodoItem(todoItemDTO.Id);
            Assert.IsType<OkObjectResult>(result);
        }
        /// <summary>
        /// This test case is to test whether the TODO item doesn't exits in list
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetTodoItem_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            _repositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                           .ReturnsAsync((TodoItemDTO)null);
            var result = await _controller.GetTodoItem(Guid.NewGuid());
            Assert.IsType<NotFoundResult>(result);
        }
        /// <summary>
        /// This is to test API will give a MISMATCH TODO Item error or bad request error since some one trying to Update list Via API which doesn't exists
        /// In such case it will give bad request error.
        /// </summary>
        /// <returns>MISMATCH ID and Bad request error</returns>
        [Fact]
        public async Task UpdateTodoItem_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            var result = await _controller.UpdateTodoItem(Guid.NewGuid(), new TodoItemDTO { Id = Guid.NewGuid() });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        /// <summary>
        /// This is to test API will give a MISMATCH TODO Item error or bad request error since some one trying to Update list Via API which doesn't exists
        /// In such case it will give bad request error.
        /// </summary>
        /// <returns>MISMATCH TO Item and Bad request error</returns>
        [Fact]
        public async Task UpdateTodoItem_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            var guid = Guid.NewGuid();
            _repositoryMock.Setup(repo => repo.GetByIdAsync(guid)).ReturnsAsync((TodoItemDTO)null);

            var result = await _controller.UpdateTodoItem(guid, new TodoItemDTO { Id = guid });

            Assert.IsType<NotFoundResult>(result);
        }
        /// <summary>
        /// This test case is to test the POST request when w are trying to add TODO Item in a list
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostTodoItem_ReturnsCreatedAtAction_WhenValidTodoIsSupplied()
        {
            var todoItemDTO = new TodoItemDTO { Id = Guid.NewGuid(), Description = "Test" };

            var result = await _controller.CreateTodoItem(todoItemDTO);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task CreateTodoItem_BadRequestObjectResult_WhenDescriptionIsInvalid()
        {
            // Assuming a null or empty "Description" is considered invalid
            var todoItemDTO = new TodoItemDTO { Id = Guid.NewGuid(), Description = "" };

            // Act
            var result = await _controller.CreateTodoItem(todoItemDTO);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        /// <summary>
        /// This Test case is to test when someone is trying to Add the TODO Item which is already exits in a list
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task PostTodoItem_BadRequestObjectResult_WhenDescriptionAlreadyExists()
        {
            // Assuming the "Name" should be unique
            var todoItemDTO = new TodoItemDTO { Id = Guid.NewGuid(), Description = "Test" };
            _repositoryMock.Setup(repo => repo.GetAllAsync(null))
                           .ReturnsAsync(new List<TodoItemDTO> { new TodoItemDTO { Description = "Test" } });

            var result = await _controller.CreateTodoItem(todoItemDTO);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
