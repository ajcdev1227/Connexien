namespace Connexien.Lib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProfileArticle
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProfileArticle()
        {
            ProfileArticleTags = new HashSet<ProfileArticleTag>();
        }

        public long Id { get; set; }

        public long ProfileId { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Published { get; set; }

        [Required]
        [StringLength(50)]
        public string Guid { get; set; }

        [StringLength(500)]
        public string Title { get; set; }

        public string Description { get; set; }

        [StringLength(250)]
        public string MimeType { get; set; }

        [StringLength(50)]
        public string Filename { get; set; }

        [StringLength(50)]
        public string PaymentStatus { get; set; }

        [StringLength(50)]
        public string PaymentTxn { get; set; }

        [StringLength(50)]
        public string Visibility { get; set; }

        public long Views { get; set; }

        [StringLength(500)]
        public string Authors { get; set; }

        public string WebHookRaw { get; set; }

        public virtual Profile Profile { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProfileArticleTag> ProfileArticleTags { get; set; }
    }
}
