using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.Models;

namespace TodoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
          if (_context.TodoItems == null)
          {
              return NotFound();
          }
            return await _context.TodoItems.Include(todoItem => todoItem.TodoItemDetail).ToListAsync();
        }

        // GET: api/TodoItems/5
        // 暫不使用
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
        {
          if (_context.TodoItems == null)
          {
              return NotFound();
          }
            var todoItem = await _context.TodoItems.Include(todoItem => todoItem.TodoItemDetail).FirstOrDefaultAsync(todoItem => todoItem.Id == id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/TodoItems/5
        // 暫不使用
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PUT: api/TodoItems/5/TodoItemDetails/2
        [HttpPut("{id}/TodoItemDetails/{itemDetailsId}")]
        public async Task<IActionResult> PutTodoItemDetail(int id, int itemDetailsId, TodoItemDetail todoItemDetail)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return BadRequest();
            }

            var itemDetail = await _context.TodoItemDetail.FindAsync(itemDetailsId);
            if (itemDetail == null)
            {
                return BadRequest();
            }

            itemDetail.isFinish = todoItemDetail.isFinish;

            _context.Entry(itemDetail).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
          if (_context.TodoItems == null)
          {
              return Problem("Entity set 'TodoContext.TodoItems'  is null.");
          }
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/TodoItems/empty
        [HttpDelete("empty")]
        public async Task<IActionResult> DeleteTodoItemAll()
        {
            if (_context.TodoItems == null)
            {
                return NotFound();
            }

            if (_context.TodoItemDetail == null) {
                return NotFound();
            }

            _context.TodoItems.RemoveRange(_context.TodoItems);
            _context.TodoItemDetail.RemoveRange(_context.TodoItemDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/TodoItems/5/TodoItemDetails/6
        [HttpDelete("{id}/TodoItemDetails/{itemDetailsId}")]
        public async Task<IActionResult> DeleteTodoItemDetail(int id, int itemDetailsId)
        {
            if (_context.TodoItems == null)
            {
                return NotFound();
            }
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            var todoItemDetail = await _context.TodoItemDetail.FindAsync(itemDetailsId);
            if (todoItemDetail == null)
            {
                return NotFound();
            }

            // _context.TodoItems.Remove(todoItem);
            _context.TodoItemDetail.Remove(todoItemDetail);
            await _context.SaveChangesAsync();

            var todoItemCount = await _context.TodoItems.Include(todoItem => todoItem.TodoItemDetail).FirstOrDefaultAsync(todoItem => todoItem.Id == id);
            if (todoItemCount.TodoItemDetail.Count == 0)
            {
                _context.TodoItems.Remove(todoItem);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        private bool TodoItemExists(int id)
        {
            return (_context.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
