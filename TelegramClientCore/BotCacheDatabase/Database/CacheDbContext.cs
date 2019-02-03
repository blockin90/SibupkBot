using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpkModel.Database;

namespace TelegramClientCore.BotCache.Database
{
    /// <summary>
    /// Класс для хранения в памяти результатов последних запросов расписания.
    /// Используется для оптимизации рассылок расписания "на сегодня" и "на завтра" 
    /// для избежания лишних запросов к сайту 
    /// </summary>
    public class CacheDbContext : DbContext
    {
        SqliteConnection _connection = new SqliteConnection("DataSource=:memory:");
        public CacheDbContext()
        {
            //база sqlite с режимом InMemory живет только при открытом подключении. Принудительно открываем: 
            _connection.Open();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //производим много поисковых операций с датой, создаем индекс (а надо ли?)
            modelBuilder.Entity<CachedGroupWorkDay>().HasIndex(wd => wd.Date);
        }

        public override void Dispose()
        {
            _connection.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// набор кэшированных групп (по факту хранятся только id групп из основной БД)
        /// </summary>
        public DbSet<CachedGroup> CachedGroups { get; set; }
        /// <summary>
        /// кэшированные дни расписания, привязанные к группам
        /// </summary>
        public DbSet<CachedGroupWorkDay> CachedGroupWorkDays { get; set; }
        public DbSet<WorkDay> WorkDays { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
    }
}
