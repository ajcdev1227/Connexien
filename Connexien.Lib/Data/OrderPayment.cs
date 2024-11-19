namespace Connexien.Lib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OrderPayment
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string OrderNumber { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string LineId { get; set; }

        [Required]
        [StringLength(50)]
        public string Sku { get; set; }

        [Column(TypeName = "numeric")]
        public decimal Quantity { get; set; }

        public decimal ItemPrice { get; set; }

        public long ContentPartnerId { get; set; }

        [Required]
        [StringLength(100)]
        public string ChargeId { get; set; }

        public decimal PaidOutPrice { get; set; }
    }
}
