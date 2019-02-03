using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TelegramClientCore.BotDatabase
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ChatId { get; set; }

        [MaxLength(64)]
        public string Username{ get; set; }
        [MaxLength(64)]
        public string LastName{ get; set; }
        [MaxLength(64)]
        public string FirstName{ get; set; }        
    }
}
