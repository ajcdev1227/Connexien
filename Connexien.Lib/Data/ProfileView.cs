namespace Connexien.Lib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProfileView
    {
        public long Id { get; set; }

        public long? UserId { get; set; }

        public long ProfileId { get; set; }

        public DateTime Created { get; set; }

        [Required]
        [StringLength(30)]
        public string Ip { get; set; }

        [StringLength(20)]
        public string Type { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual User User { get; set; }
    }
}
