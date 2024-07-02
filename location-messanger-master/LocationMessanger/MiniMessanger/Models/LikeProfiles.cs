using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace miniMessanger.Models
{
    public partial class LikeProfiles
    {
        public LikeProfiles()
        {
            
        }
        [Key]
        public long LikeId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("ToUser")] 
        public int ToUserId { get; set; }
        public bool Like { get; set; }
        public bool Dislike { get; set; }
        public virtual User User { get; set; }
        public virtual User ToUser { get; set; }
    }
}
