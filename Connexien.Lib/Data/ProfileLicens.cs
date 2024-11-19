namespace Connexien.Lib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProfileLicenses")]
    public partial class ProfileLicens
    {
        public long Id { get; set; }

        public long ProfileId { get; set; }

        public long LicenseId { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Expiration { get; set; }

        [StringLength(100)]
        public string LicenseNumber { get; set; }

        public virtual License License { get; set; }

        public virtual Profile Profile { get; set; }
    }
}
