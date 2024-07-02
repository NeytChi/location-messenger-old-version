﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace miniMessanger.Models
{
    public partial class Profile
    {
        [Key]
        public int ProfileId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public string UrlPhoto { get; set; }
        public sbyte? ProfileAge { get; set; }
        public bool ProfileGender { get; set; }
        public string ProfileCity { get; set; }
        public double profileLatitude { get; set; }
        public double profileLongitude { get; set; }
        public int weight { get; set; }
        public int height { get; set; }
        public string status { get; set; }
        public virtual User User { get; set; }
    }
}