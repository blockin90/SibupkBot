using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

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

        protected DbSet<ChatStateRecord> ChatStateRecords { get; set; }
        public DbSet<LogRecord> LogRecords { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserConfig> UserConfigs { get; set; }

        private static BotDbContext _instance;
        public static BotDbContext Instance
        {
            get
            {
                return _instance ?? (_instance = new BotDbContext());
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public void AddOrUpdateChatStateRecord(ChatId chatId, string stateName, string extraData = null)
        {
            var chatState = ChatStateRecords.Find(chatId.Identifier);
            if (chatState == null) {
                chatState = new ChatStateRecord() { ChatId = chatId.Identifier, StateName = stateName, ExtraData = extraData };
                ChatStateRecords.Add(chatState);
            } else {
                chatState.StateName = stateName;
                chatState.ExtraData = extraData;
                ChatStateRecords.Update(chatState);
            }
            SaveChangesAsync();
        }

        public bool TryToGetChatStateRecord(ChatId chatId, out ChatStateRecord chatState)
        {
            chatState = ChatStateRecords.Find(chatId.Identifier);
            if (chatState != null) {
                return true;
            }
            return false;
        }
    }
}
