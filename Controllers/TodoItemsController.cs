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
    // [Route("api/[controller]")]
    [Route("api/")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems
        // [HttpGet]
        [HttpGet("TodoItems")]
        public async Task<ActionResult<IEnumerable<TodoDateGroup>>> GetTodoItems()
        {
          if (_context.TodoDateGroup == null)
          {
              return NotFound();
          }
            return await ReturnTodoData();
        }

        // PUT: api/TodoItems/Sort
        // [HttpPut("Sort")]
        [HttpPut("TodoItems/Sort")]
        public async Task<ActionResult<IEnumerable<TodoDateGroup>>> PutTodoItemSort(List<TodoItems> todoItems)
        {
        
            for (int item = 0; item < todoItems.Count; item ++) {
                var itemID = todoItems[item].Id;
                var sqlTodoItem = await _context.TodoItems.FindAsync(itemID);
                if (sqlTodoItem != null) {
                    sqlTodoItem.SortId = item + 1;
                    sqlTodoItem.GroupId = todoItems[item].GroupId;
                    
                    _context.Entry(sqlTodoItem).State = EntityState.Modified;
                }
            }
            
            await _context.SaveChangesAsync();
            return await ReturnTodoData();
        }

        // PUT: api/TodoItems/5/item/2
        // [HttpPut("{id}/todoItems/{itemDetailsId}")]
        // PUT: api/TodoDateGroup/5/todoItem/2
        [HttpPut("TodoDateGroup/{groupID}/todoItem/{itemID}")]
        public async Task<ActionResult<IEnumerable<TodoDateGroup>>> PutTodoItemDetail(TodoItems todoItems)
        {
            var todoGroup = await _context.TodoDateGroup.FindAsync(todoItems.GroupId);
            if (todoGroup == null)
            {
                return BadRequest();
            }

            var todoItem = await _context.TodoItems.FindAsync(todoItems.Id);
            if (todoItem == null)
            {
                return BadRequest();
            }

            todoItem.IsFinish = todoItems.IsFinish;

            _context.Entry(todoItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // return NoContent();
            // return await ReturnTodoData();
            return await ReturnTodoData(false);
        }

        // POST: api/TodoItems
        // [HttpPost]
        [HttpPost("TodoItems")]
        public async Task<ActionResult<IEnumerable<TodoDateGroup>>> PostTodoItem(TodoDateGroup todoDateGroup) {
            var getItemDate = todoDateGroup.ItemDate;
            var getItemText = todoDateGroup.TodoItems[0].ItemText;
            var maxItemID = await GetMaxItemID();

            // step 1: 找出欲新增的日期是否已存在
            var todoGroup = await _context.TodoDateGroup.Where(item => item.ItemDate == getItemDate).FirstOrDefaultAsync();

            // step 2: 如果不存在，新增日期 & 待辦事項
            if (todoGroup == null) {
                var maxGroupID = await GetMaxGroupID();
                var newTodoDateGroup = new TodoDateGroup
                {
                    Id = maxGroupID, 
                    ItemDate = getItemDate
                };

                var newItemlist = new TodoItems
                {
                    Id = maxItemID, 
                    ItemText = getItemText, 
                    SortId = maxItemID, 
                    GroupId = maxGroupID
                };
                newTodoDateGroup.TodoItems.Add(newItemlist);

                _context.TodoDateGroup.Add(newTodoDateGroup);
            } else {
                // step 3: 如果存在，就只新增待辦事項
                var newTodoItem = new TodoItems
                {
                    Id = maxItemID, 
                    ItemText = getItemText, 
                    SortId = maxItemID, 
                    GroupId = todoGroup.Id
                };

                _context.TodoItems.Add(newTodoItem);
            }

            await _context.SaveChangesAsync();
            // return NoContent();
            return await ReturnTodoData();
        }

        // DELETE: api/TodoItems/empty
        [HttpDelete("TodoItems/empty")]
        public async Task<ActionResult<IEnumerable<TodoDateGroup>>> EmptyTodoItems()
        {
            if (_context.TodoDateGroup == null)
            {
                return NotFound();
            }

            if (_context.TodoItems == null) {
                return NotFound();
            }

            _context.TodoDateGroup.RemoveRange(_context.TodoDateGroup);
            _context.TodoItems.RemoveRange(_context.TodoItems);
            await _context.SaveChangesAsync();

            return await ReturnTodoData(false);
        }

        // DELETE: api/TodoItems/5/TodoItemDetails/6
        [HttpDelete("TodoDateGroup/{groupID}/todoItem/{itemID}")]
        public async Task<ActionResult<IEnumerable<TodoDateGroup>>> DeleteTodoItem(int groupID, int itemID)
        {
            if (_context.TodoDateGroup == null)
            {
                return NotFound();
            }
            
            var todoGroup = await _context.TodoDateGroup.FindAsync(groupID);
            if (todoGroup == null)
            {
                return NotFound();
            }

            var todoItem = await _context.TodoItems.FindAsync(itemID);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            var todoItemCount = await _context.TodoDateGroup.Include(todo => todo.TodoItems).FirstOrDefaultAsync(todoItem => todoItem.Id == groupID);
            if (todoItemCount?.TodoItems?.Count == 0)
            {
                _context.TodoDateGroup.Remove(todoGroup);
                await _context.SaveChangesAsync();
            }

            // return NoContent();
            return await ReturnTodoData();
        }

        private async Task<int> GetMaxGroupID() {
            var todoList = await _context.TodoDateGroup.OrderByDescending(item => item.Id).FirstOrDefaultAsync();
            if (todoList == null) {
                return 1;
            } else {
                return todoList.Id + 1;
            }
        }

        private async Task<int> GetMaxItemID() {
            var todoItem = await _context.TodoItems.OrderByDescending(item => item.Id).FirstOrDefaultAsync();
            if (todoItem == null) {
                return 1;
            } else {
                return todoItem.Id + 1;
            }
        }

        private async Task<ActionResult<IEnumerable<TodoDateGroup>>> ReturnTodoData(bool isOrderby = true) {
            if (isOrderby == true) {
                return await _context.TodoDateGroup.OrderByDescending(group => group.ItemDate).Include(todo => todo.TodoItems.OrderByDescending(item => item.SortId)).ToListAsync();
            } else {
                return await _context.TodoDateGroup.Include(todo => todo.TodoItems).ToListAsync();
            }
        }
    }
}
