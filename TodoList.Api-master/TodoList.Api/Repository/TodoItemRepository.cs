using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using TodoList.Api.Context;
using TodoList.Api.DTOs;
using TodoList.Api.Entities;

namespace TodoList.Api.Repository
{
    public class TodoItemRepository : IRepository<TodoItem, TodoItemDTO>
    {
        private readonly TodoContext _context;
        private readonly IMapper _mapper;

        public TodoItemRepository(TodoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TodoItemDTO>> GetAllAsync(Expression<Func<TodoItem, bool>> filter = null)
        {
            IQueryable<TodoItem> query = _context.TodoItems.AsNoTracking();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            var items = await query.ToListAsync();
            return _mapper.Map<IEnumerable<TodoItemDTO>>(items);
        }

        public async Task<TodoItemDTO> GetByIdAsync(Guid id)
        {
            var item = await _context.TodoItems.FindAsync(id);
            return _mapper.Map<TodoItemDTO>(item);
        }

        public async Task AddAsync(TodoItemDTO entity)
        {
            var item = _mapper.Map<TodoItem>(entity);
            await _context.TodoItems.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TodoItemDTO entity)
        {
            var item = _mapper.Map<TodoItem>(entity);
            _context.ChangeTracker.Clear();
            _context.TodoItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.TodoItems.FindAsync(id);
            if (entity != null)
            {
                _context.ChangeTracker.Clear();
                _context.TodoItems.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
