﻿using Microsoft.EntityFrameworkCore;
using System.Linq;
using Telegram.Bot.Types;
using UpkModel.Database.Users;

namespace TelegramClientCore.BotDatabase
{
    public class BotDbContext : DbContext
    {
        protected BotDbContext()
        {
            Database.EnsureCreated();
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=UserLog2.db");            
        }        

        public DbSet<LogRecord> LogRecords { get; set; }
        public DbSet<UpkModel.Database.Users.User> Users { get; set; }
        public DbSet<UserConfig> UserConfigs { get; set; }

        private static BotDbContext _instance;
        public static object syncObject = new object();
        public static BotDbContext Instance
        {
            get
            {
                if (_instance == null) {
                    lock (syncObject) {
                        if (_instance == null) {
                            _instance = new BotDbContext();
                            _instance.Users.Load();
                            _instance.UserConfigs.Load();
                        }
                    }
                }
                return _instance;
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogRecord>().HasNoKey();

        }

        public void AddOrUpdateChatStateRecord(ChatId chatId, string stateName, string extraData = null)
        {
            lock (syncObject) {
                var user = Users.Find(chatId.Identifier);
                user.StateName = stateName;
                user.ExtraData = extraData;
                Users.Update(user);
                SaveChangesAsync();
            }
        }

        public bool TryToGetChatStateRecord(ChatId chatId, ref string stateName, ref string extraData)
        {
            lock (syncObject) {
                var userInfo = Users.AsNoTracking()
                    .Where(u => u.ChatId == chatId.Identifier)
                    .Select(u => new { u.StateName, u.ExtraData })
                    .FirstOrDefault();
         
            if(userInfo != null) {
                stateName = userInfo.StateName;
                extraData = userInfo.ExtraData;
                return true;
                }
            }
            return false;
        }
    }
}
