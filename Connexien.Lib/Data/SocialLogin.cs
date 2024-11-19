namespace Connexien.Lib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SocialLogin
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        [StringLength(50)]
        public string SocialName { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Deleted { get; set; }

        [StringLength(500)]
        public string Token { get; set; }

        public DateTime? Expires { get; set; }

        [StringLength(100)]
        public string SocialId { get; set; }

        public virtual User User { get; set; }
    }
}
