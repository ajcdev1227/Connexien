namespace Connexien.Lib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProfileRating
    {
        public long Id { get; set; }

        public long? UserId { get; set; }

        public long ProfileId { get; set; }

        public DateTime Created { get; set; }

        public decimal Rating { get; set; }

        [StringLength(500)]
        public string Comments { get; set; }

        [StringLength(50)]
        public string IP { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual User User { get; set; }
    }
}
