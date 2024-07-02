using System;
using System.ComponentModel.DataAnnotations;

namespace miniMessanger.Models
{
    public partial class Chatroom
    {
        [Key]
        public int ChatId { get; set; }
        public string ChatToken { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
