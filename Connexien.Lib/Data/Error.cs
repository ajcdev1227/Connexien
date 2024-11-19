namespace Connexien.Lib
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class Error
    {
        public long Id { get; set; }

        public DateTime? Created { get; set; }

        public string Exception { get; set; }

        public string InnerException { get; set; }

        [StringLength(50)]
        public string Ip { get; set; }

        public string Note { get; set; }
    }
}
