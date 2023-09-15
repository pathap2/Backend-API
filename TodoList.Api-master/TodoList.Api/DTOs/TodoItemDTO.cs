using System;

namespace TodoList.Api.DTOs
{
    public class TodoItemDTO
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public bool IsCompleted { get; set; }
    }
}
