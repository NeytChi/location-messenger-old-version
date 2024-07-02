using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace miniMessanger.Models
{
    public partial class Participants
    {
        [Key]
        public long ParticipantId { get; set; }
        [ForeignKey("Chat")]
        public int ChatId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("Opposite")]
        public int OpposideId { get; set; }
        public virtual Chatroom Chat { get; set; }
        public virtual User Opposite { get; set; }
        public virtual User User { get; set; }
    }
}
