using AutoMapper;
using TodoList.Api.DTOs;
using TodoList.Api.Entities;

namespace TodoList.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TodoItemDTO, TodoItem>().ReverseMap();
        }
    }
}
