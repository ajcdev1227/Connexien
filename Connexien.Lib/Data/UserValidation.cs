namespace Connexien.Lib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserValidation
    {
        [Key]
        [StringLength(150)]
        public string Token { get; set; }

        public long UserId { get; set; }

        public DateTime Created { get; set; }

        [Required]
        [StringLength(50)]
        public string Process { get; set; }

        public int HoursValid { get; set; }

        public virtual User User { get; set; }
    }
}
