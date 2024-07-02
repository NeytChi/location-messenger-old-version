using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace miniMessanger.Models
{
    public partial class BlockedUser
    {
        [Key]
        public int BlockedId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("Blocked")]
        public int BlockedUserId { get; set; }
        [Column("BlockedReason", TypeName = "varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci")]
        public string BlockedReason { get; set; }
        public int BlockedDeleted { get; set; }
        public virtual User User { get; set; }
        public virtual User Blocked { get; set; }
    }
}
