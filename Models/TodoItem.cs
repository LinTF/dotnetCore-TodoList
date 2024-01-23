using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TodoList.Models
{
    /// <summary>
    /// 某日期中的待辦事項
    /// </summary>
    public class TodoItem
    {
        public int Id { get; set; }
        /// <summary>
        /// 待辦事項文字
        /// </summary>
        /// <value></value>
        public string ItemText { get; set; } = "";
        /// <summary>
        /// 是否已完成
        /// </summary>
        /// <value></value>
        public Boolean IsFinish { get; set; } = false;
        /// <summary>
        /// 排序
        /// </summary>
        /// <value></value>
        public int SortId { get; set; }


        public int? TodoGroupId { get; set; }
        [JsonIgnore]
        public TodoGroup? TodoGroup { get; set; }
    }
}