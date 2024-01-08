using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using NuGet.Versioning;
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

        // PUT: api/TodoItems/Sort
        [HttpPut("Sort")]
        public async Task<IActionResult> PutTodoItemSort(List<TodoItemDetail> todoItemDetail)
        {
        
            for (int item = 0; item < todoItemDetail.Count; item ++) {
                var inputDetailID = todoItemDetail[item].Id;
                var sqlDetail = await _context.TodoItemDetail.FindAsync(inputDetailID);
                if (sqlDetail != null) {
                    sqlDetail.SortId = item + 1;
                    sqlDetail.TodoItemId = todoItemDetail[item].TodoItemId;
                    
                    _context.Entry(sqlDetail).State = EntityState.Modified;
                }
            }
            
            await _context.SaveChangesAsync();

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

            itemDetail.IsFinish = todoItemDetail.IsFinish;

            _context.Entry(itemDetail).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/TodoItems
        // 暫不使用
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPost]
        // public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        // {
        //   if (_context.TodoItems == null)
        //   {
        //       return Problem("Entity set 'TodoContext.TodoItems'  is null.");
        //   }
        //     _context.TodoItems.Add(todoItem);
        //     await _context.SaveChangesAsync();

        //     return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
        // }

        // POST: api/TodoItems
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem) {
            // 撰寫中
            var getItemDate = todoItem.ItemDate;
            var getItemText = todoItem.TodoItemDetail[0].ItemText;
            var maxTodoID = await GetMaxTodoID();
            var maxItemID = await GetMaxItemID();

            // step 1: 找出欲新增的日期是否已存在
            var todoItems = await _context.TodoItems.Where(item => item.ItemDate == getItemDate).FirstOrDefaultAsync();

            // step 2: 如果不存在，新增日期 & 待辦事項
            if (todoItems == null) {
                var newTodoList = new TodoItem
                {
                    Id = maxTodoID, 
                    ItemDate = getItemDate
                };

                var newItemlist = new TodoItemDetail
                {
                    Id = maxItemID, 
                    ItemText = getItemText, 
                    SortId = maxItemID, 
                    TodoItemId = maxTodoID
                };
                newTodoList.TodoItemDetail.Add(newItemlist);

                _context.TodoItems.Add(newTodoList);
            } else {
                // step 3: 如果存在，就只新增待辦事項
                var newItemlist = new TodoItemDetail
                {
                    Id = maxItemID, 
                    ItemText = getItemText, 
                    SortId = maxItemID, 
                    TodoItemId = todoItems.Id
                };

                _context.TodoItemDetail.Add(newItemlist);
            }

            await _context.SaveChangesAsync();
            return NoContent();
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
            if (todoItemCount?.TodoItemDetail?.Count == 0)
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

        private async Task<int> GetMaxTodoID() {
            var todoList = await _context.TodoItems.OrderByDescending(item => item.Id).FirstOrDefaultAsync();
            if (todoList == null) {
                return 1;
            } else {
                return todoList.Id + 1;
            }
        }

        private async Task<int> GetMaxItemID() {
            var todoItemDetail = await _context.TodoItemDetail.OrderByDescending(item => item.Id).FirstOrDefaultAsync();
            if (todoItemDetail == null) {
                return 1;
            } else {
                return todoItemDetail.Id + 1;
            }
        }
    }
}
