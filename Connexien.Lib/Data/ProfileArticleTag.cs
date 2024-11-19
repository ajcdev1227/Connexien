namespace Connexien.Lib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProfileArticleTag
    {
        public long Id { get; set; }

        public long ArticleId { get; set; }

        public long? ServiceTypeId { get; set; }

        public string ProductTypeId { get; set; }

        public string SectorTypeId { get; set; }

        public virtual SectorType SectorType { get; set; }

        public virtual ProfileArticle ProfileArticle { get; set; }

        public virtual ServiceType ServiceType { get; set; }
    }
}
