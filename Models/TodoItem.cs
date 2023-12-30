using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoList.Models
{
    public class TodoItem
    {
        /// <summary>
        /// 流水號
        /// </summary>
        /// <value></value>
        public int ID { get; set; }

        /// <summary>
        ///  待辦日期
        /// </summary>
        /// <value></value>
        public DateOnly ItemDate { get; set; }

        /// <summary>
        /// 是否編輯
        /// </summary>
        /// <value></value>
        public Boolean IsEdit { get; set; } = false;

        /// <summary>
        /// 待辦清單
        /// </summary>
        /// <value></value>
        public List<TodoItemDetail> TodoItems { get; set; } = new List<TodoItemDetail>();
    }
}