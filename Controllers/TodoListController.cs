using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoList.Models;

namespace TodoList.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoListController : ControllerBase
    {
        [HttpGet("GetTodoItems")]
        public List<TodoItem> GetTodoItem() 
        {
            List<TodoItemDetail> itemDetail = new List<TodoItemDetail>()
            {
                new TodoItemDetail() {
                    ItemText = "測試項目 1",
                },
                new TodoItemDetail() {
                    ItemText = "測試項目 2",
                },
                new TodoItemDetail() {
                    ItemText = "測試項目 3 la",
                }
            };

            List<TodoItem> todoItems = new List<TodoItem>()
            {
                new TodoItem() {
                    ID = 1,
                    ItemDate = DateOnly.Parse("2023-12-12"),
                    TodoItems = itemDetail
                }
            };

            return todoItems;
        }
    }
}