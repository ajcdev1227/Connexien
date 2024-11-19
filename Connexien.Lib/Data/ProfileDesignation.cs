namespace Connexien.Lib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProfileDesignation
    {
        public long Id { get; set; }

        public long ProfileId { get; set; }

        public long DesignationId { get; set; }

        public DateTime Created { get; set; }

        public virtual Designation Designation { get; set; }

        public virtual Profile Profile { get; set; }
    }
}
