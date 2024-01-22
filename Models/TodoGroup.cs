using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TodoList.Models
{
    public class TodoDateGroup
    {
        /// <summary>
        /// 流水號
        /// </summary>
        /// <value></value>
        public int Id { get; set; }

        /// <summary>
        ///  待辦日期
        /// </summary>
        /// <value></value>
        public DateOnly ItemDate { get; set; }

        /// <summary>
        /// 待辦清單
        /// </summary>
        /// <value></value>
        public List<TodoItems> TodoItems { get; set; } = new List<TodoItems>();
    }
}