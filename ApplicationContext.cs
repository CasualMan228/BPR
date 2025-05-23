using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity; //работа с БД

namespace BPR
{
    public class ApplicationContext : DbContext //подключение к бд
    {
        public DbSet<User> Users { get; set; } //DbSet -> тип, который представляет таблицу бд
        public DbSet<Plane> Planes { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public ApplicationContext() : base("DefaultConnection") { } //указание, где находится бд
    }
}
