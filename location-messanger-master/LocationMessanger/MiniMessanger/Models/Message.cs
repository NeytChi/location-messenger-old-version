using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace miniMessanger.Models
{
    /// <summary>
    /// This class need to contains chat's messages.
    /// Message type can be: "text" and "photo". 
    /// </summary>
    public partial class Message
    {
        [Key]
        public long MessageId { get; set; }
        [ForeignKey("Chat")]
        public int ChatId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public string MessageType { get; set; }
        [Column("MessageText", TypeName = "varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci")]
        public string MessageText { get; set; }
        public string UrlFile { get; set; }
        public bool MessageViewed { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public Chatroom Chat { get; set; }
        public User User { get; set; }

    }
}
