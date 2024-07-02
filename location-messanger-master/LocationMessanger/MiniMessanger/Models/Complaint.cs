using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace miniMessanger.Models
{
    public partial class Complaint
    {
        [Key]
        public int ComplaintId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("Blocked")]
        public int BlockId { get; set; }
        [ForeignKey("Message")]
        public long MessageId { get; set; }
        [Column("ComplaintText", TypeName = "varchar(2000) CHARACTER SET utf8 COLLATE utf8_general_ci")]
        public string ComplaintText { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual BlockedUser Blocked { get; set; }
        public virtual Message Message { get; set; }
        public virtual User User { get; set; }
    }
}
