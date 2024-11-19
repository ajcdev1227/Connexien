namespace Connexien.Lib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Profile
    {
        private string _vanity;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Profile()
        {
            ContentItems = new HashSet<ContentItem>();
            ProfileArticles = new HashSet<ProfileArticle>();
            ProfileDesignations = new HashSet<ProfileDesignation>();
            ProfileImages = new HashSet<ProfileImage>();
            ProfileLicenses = new HashSet<ProfileLicens>();
            ProfileLinks = new HashSet<ProfileLink>();
            ProfileProductTypes = new HashSet<ProfileProductType>();
            ProfileRatings = new HashSet<ProfileRating>();
            ProfileServiceTypes = new HashSet<ProfileServiceType>();
            ProfileViews = new HashSet<ProfileView>();
        }

        public long Id { get; set; }

        public long UserId { get; set; }

        public long CompanyId { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Deleted { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(30)]
        public string Phone { get; set; }

        [StringLength(30)]
        public string Fax { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(20)]
        public string YearsExp { get; set; }

        [StringLength(20)]
        public string Engagements { get; set; }

        [StringLength(50)]
        public string EdLevel { get; set; }

        [StringLength(100)]
        public string SchoolName { get; set; }

        [StringLength(100)]
        public string DegreeMajor { get; set; }

        public int? YearGrad { get; set; }

        public bool DisciplinaryHistory { get; set; }

        [StringLength(100)]
        public string ServiceFocus { get; set; }

        [StringLength(20)]
        public string Availability { get; set; }

        [StringLength(50)]
        public string CommunicationPref { get; set; }

        [StringLength(300)]
        public string ServiceStates { get; set; }

        [StringLength(30)]
        public string FeeType { get; set; }

        public bool MinFee { get; set; }

        public bool FeeNegot { get; set; }

        public int? HourlyRate { get; set; }

        public decimal? AvgRating { get; set; }

        [StringLength(20)]
        public string Type { get; set; }

        [StringLength(50)]
        public string Vanity
        {
            get
            {
                if (string.IsNullOrEmpty(_vanity))
                {
                    return System.Guid.NewGuid().ToString();
                }
                var vanityCleansed = _vanity?.Replace("https://app.connexien.com/p/", "");
                return vanityCleansed;
            }
            set { _vanity = value; }
        }

        [StringLength(50)]
        public string Guid { get; set; }

        [StringLength(100)]
        public string PaymentAccount { get; set; }

        [StringLength(100)]
        public string PaymentAccountRefreshToken { get; set; }

        public string Summary { get; set; }

        public virtual Company Company { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContentItem> ContentItems { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProfileArticle> ProfileArticles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProfileDesignation> ProfileDesignations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProfileImage> ProfileImages { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProfileLicens> ProfileLicenses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProfileLink> ProfileLinks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProfileProductType> ProfileProductTypes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProfileRating> ProfileRatings { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProfileServiceType> ProfileServiceTypes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProfileView> ProfileViews { get; set; }
    }
}
