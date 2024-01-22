using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TodoList.Models
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<TodoDateGroup> TodoDateGroup { get; set; } = null!;
        public DbSet<TodoItems> TodoItems { get; set; } = null!;
    }
}