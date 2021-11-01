namespace UpkModel.Database
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UpkDatabaseContext : DbContext
    {
        protected UpkDatabaseContext()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
            if( !Configs.Any() ) {
                Configs.Add(new Config() { Key = "TeacherFirstDate", Value = DateTime.Today.ToShortDateString() });
                Configs.Add(new Config() { Key = "TeacherLastDate", Value = DateTime.Today.AddDays(10).ToShortDateString() });
                //инициализируем базу данных
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=UPK.db");
            // optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SupkDatabase;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
            modelBuilder.Entity<Lesson>().HasKey(l => new { l.WorkDayId, l.LessonNum, l.Group });
            //т.к. часто идет поиск по недельным интервалам в БД, строим индексы по этим полям
            modelBuilder.Entity<WeekInterval>().HasIndex(wi => wi.Start);   
            modelBuilder.Entity<WeekInterval>().HasIndex(wi => wi.End);
            //индекс по ФИО преподавателя
            modelBuilder.Entity<Teacher>().HasIndex(t => t.FIO);
            //индекс по сокращенному названию группы, по которому будем производить поиск
            modelBuilder.Entity<Group>().HasIndex(t => t.ShortName );
            //modelBuilder.Entity<Teacher>().HasKey(t => new { t.FIO, t.id_KodKaf });
        }
        
        public virtual DbSet<Lesson> Lessons { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<Config> Configs { get; set; }
        public virtual DbSet<WeekInterval> WeekIntervals { get; set; }
        public virtual DbSet<WorkDay> WorkDays { get; set; }
        public virtual DbSet<Faculty> Faculties { get; set; }
        public virtual DbSet<Group> Groups { get; set; }

        public void UpdateWeekIntervals(IEnumerable<WeekInterval> weekIntervals)
        {
            lock (WeekIntervals) {
                Database.ExecuteSqlRaw("delete from WeekIntervals");
                WeekIntervals.AddRange(weekIntervals);
                SaveChanges();
            }
        }
        public void UpdateTeachers( IEnumerable<Teacher> teachers)
        {
            lock (Teachers) {
                Database.ExecuteSqlRaw("delete from Teachers");
                Database.ExecuteSqlRaw("delete from Departments");
                SaveChanges();
                Teachers.AddRange(teachers);
                SaveChanges();

            }
        }
        public void UpdateGroups( IEnumerable<Group> groups)
        {
            lock (Groups) {
                Database.ExecuteSqlRaw("delete from Groups");
                SaveChanges();
                Groups.AddRange(groups);
                SaveChanges();

            }
        }
        static UpkDatabaseContext _instance;
        public static UpkDatabaseContext Instance
        {
            get
            {
                return _instance ?? (_instance = new UpkDatabaseContext());
            }
        }
    }
}