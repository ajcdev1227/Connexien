namespace Connexien.Lib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProfileServiceType
    {
        public long Id { get; set; }

        public long ProfileId { get; set; }

        public long ServiceTypeId { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual ServiceType ServiceType { get; set; }
    }
}
