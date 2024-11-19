namespace Connexien.Lib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProfileLink
    {
        public long Id { get; set; }

        public long ProfileId { get; set; }

        public DateTime Created { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        [StringLength(200)]
        public string Url { get; set; }

        public virtual Profile Profile { get; set; }
    }
}
