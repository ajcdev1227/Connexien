namespace Connexien.Lib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ContentItem
    {
        public long Id { get; set; }

        public long ProfileId { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        [Required]
        [StringLength(50)]
        public string Guid { get; set; }

        [Required]
        [StringLength(500)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [StringLength(4)]
        public string Sector { get; set; }

        public decimal Price { get; set; }

        [Required]
        [StringLength(250)]
        public string MimeType { get; set; }

        [Required]
        [StringLength(50)]
        public string Filename { get; set; }

        [Required]
        [StringLength(500)]
        public string Authors { get; set; }

        public int Version { get; set; }

        public int Pages { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual SectorType SectorType { get; set; }
    }
}
