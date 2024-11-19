namespace Connexien.Lib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProfileProductType
    {
        public long Id { get; set; }

        public long ProfileId { get; set; }

        public long ProductTypeId { get; set; }

        public virtual ProductType ProductType { get; set; }

        public virtual Profile Profile { get; set; }
    }
}
