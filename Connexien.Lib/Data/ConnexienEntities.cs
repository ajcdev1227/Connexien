namespace Connexien.Lib
{
	using System.Data.Entity;

	public partial class ConnexienEntities : DbContext
	{
		public ConnexienEntities()
			: base("name=ConnexienEntities")
		{
		}

		public virtual DbSet<Company> Companies { get; set; }

		public virtual DbSet<CompanyLink> CompanyLinks { get; set; }

		public virtual DbSet<ContentItem> ContentItems { get; set; }

		public virtual DbSet<Designation> Designations { get; set; }

		public virtual DbSet<Error> Errors { get; set; }

		public virtual DbSet<License> Licenses { get; set; }

		public virtual DbSet<OrderPayment> OrderPayments { get; set; }

		public virtual DbSet<ProductType> ProductTypes { get; set; }

		public virtual DbSet<ProfileArticle> ProfileArticles { get; set; }

		public virtual DbSet<ProfileArticleTag> ProfileArticleTags { get; set; }

		public virtual DbSet<ProfileDesignation> ProfileDesignations { get; set; }

		public virtual DbSet<ProfileImage> ProfileImages { get; set; }

		public virtual DbSet<ProfileLicens> ProfileLicenses { get; set; }

		public virtual DbSet<ProfileLink> ProfileLinks { get; set; }

		public virtual DbSet<ProfileProductType> ProfileProductTypes { get; set; }

		public virtual DbSet<ProfileRating> ProfileRatings { get; set; }

		public virtual DbSet<Profile> Profiles { get; set; }

		public virtual DbSet<ProfileServiceType> ProfileServiceTypes { get; set; }

		public virtual DbSet<ProfileView> ProfileViews { get; set; }

		public virtual DbSet<Role> Roles { get; set; }

		public virtual DbSet<SectorType> SectorTypes { get; set; }

		public virtual DbSet<ServiceType> ServiceTypes { get; set; }

		public virtual DbSet<SocialLogin> SocialLogins { get; set; }

		public virtual DbSet<UserRole> UserRoles { get; set; }

		public virtual DbSet<User> Users { get; set; }

		public virtual DbSet<UserValidation> UserValidations { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Company>()
				.Property(e => e.Name)
				.IsUnicode(false);

			modelBuilder.Entity<Company>()
				.Property(e => e.Size)
				.IsUnicode(false);

			modelBuilder.Entity<Company>()
				.Property(e => e.Address1)
				.IsUnicode(false);

			modelBuilder.Entity<Company>()
				.Property(e => e.Address2)
				.IsUnicode(false);

			modelBuilder.Entity<Company>()
				.Property(e => e.City)
				.IsUnicode(false);

			modelBuilder.Entity<Company>()
				.Property(e => e.State)
				.IsUnicode(false);

			modelBuilder.Entity<Company>()
				.Property(e => e.Zip)
				.IsUnicode(false);

			modelBuilder.Entity<Company>()
				.Property(e => e.Country)
				.IsUnicode(false);

			modelBuilder.Entity<Company>()
				.Property(e => e.Phone)
				.IsUnicode(false);

			modelBuilder.Entity<Company>()
				.Property(e => e.Fax)
				.IsUnicode(false);

			modelBuilder.Entity<Company>()
				.HasMany(e => e.Profiles)
				.WithRequired(e => e.Company)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<CompanyLink>()
				.Property(e => e.Type)
				.IsUnicode(false);

			modelBuilder.Entity<CompanyLink>()
				.Property(e => e.Url)
				.IsUnicode(false);

			modelBuilder.Entity<ContentItem>()
				.Property(e => e.Guid)
				.IsUnicode(false);

			modelBuilder.Entity<ContentItem>()
				.Property(e => e.Title)
				.IsUnicode(false);

			modelBuilder.Entity<ContentItem>()
				.Property(e => e.Description)
				.IsUnicode(false);

			modelBuilder.Entity<ContentItem>()
				.Property(e => e.Sector)
				.IsFixedLength()
				.IsUnicode(false);

			modelBuilder.Entity<ContentItem>()
				.Property(e => e.MimeType)
				.IsUnicode(false);

			modelBuilder.Entity<ContentItem>()
				.Property(e => e.Filename)
				.IsUnicode(false);

			modelBuilder.Entity<ContentItem>()
				.Property(e => e.Authors)
				.IsUnicode(false);

			modelBuilder.Entity<Designation>()
				.Property(e => e.Name)
				.IsUnicode(false);

			modelBuilder.Entity<Designation>()
				.Property(e => e.Type)
				.IsUnicode(false);

			modelBuilder.Entity<Designation>()
				.Property(e => e.Abbrev)
				.IsUnicode(false);

			modelBuilder.Entity<Designation>()
				.HasMany(e => e.ProfileDesignations)
				.WithRequired(e => e.Designation)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Error>()
				.Property(e => e.Exception)
				.IsUnicode(false);

			modelBuilder.Entity<Error>()
				.Property(e => e.InnerException)
				.IsUnicode(false);

			modelBuilder.Entity<Error>()
				.Property(e => e.Ip)
				.IsUnicode(false);

			modelBuilder.Entity<Error>()
				.Property(e => e.Note)
				.IsUnicode(false);

			modelBuilder.Entity<License>()
				.Property(e => e.Name)
				.IsUnicode(false);

			modelBuilder.Entity<License>()
				.Property(e => e.Type)
				.IsUnicode(false);

			modelBuilder.Entity<License>()
				.Property(e => e.Abbrev)
				.IsUnicode(false);

			modelBuilder.Entity<License>()
				.HasMany(e => e.ProfileLicenses)
				.WithRequired(e => e.License)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<OrderPayment>()
				.Property(e => e.Id)
				.IsUnicode(false);

			modelBuilder.Entity<OrderPayment>()
				.Property(e => e.OrderNumber)
				.IsUnicode(false);

			modelBuilder.Entity<OrderPayment>()
				.Property(e => e.LineId)
				.IsUnicode(false);

			modelBuilder.Entity<OrderPayment>()
				.Property(e => e.Sku)
				.IsUnicode(false);

			modelBuilder.Entity<OrderPayment>()
				.Property(e => e.Quantity)
				.HasPrecision(18, 0);

			modelBuilder.Entity<OrderPayment>()
				.Property(e => e.ChargeId)
				.IsUnicode(false);

			modelBuilder.Entity<ProductType>()
				.Property(e => e.Name)
				.IsUnicode(false);

			modelBuilder.Entity<ProductType>()
				.Property(e => e.SubCategory)
				.IsUnicode(false);

			modelBuilder.Entity<ProductType>()
				.HasMany(e => e.ProfileProductTypes)
				.WithRequired(e => e.ProductType)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<ProfileArticle>()
				.Property(e => e.Guid)
				.IsUnicode(false);

			modelBuilder.Entity<ProfileArticle>()
				.Property(e => e.Title)
				.IsUnicode(false);

			modelBuilder.Entity<ProfileArticle>()
				.Property(e => e.Description)
				.IsUnicode(false);

			modelBuilder.Entity<ProfileArticle>()
				.Property(e => e.MimeType)
				.IsUnicode(false);

			modelBuilder.Entity<ProfileArticle>()
				.Property(e => e.Filename)
				.IsUnicode(false);

			modelBuilder.Entity<ProfileArticle>()
				.Property(e => e.PaymentStatus)
				.IsUnicode(false);

			modelBuilder.Entity<ProfileArticle>()
				.Property(e => e.PaymentTxn)
				.IsUnicode(false);

			modelBuilder.Entity<ProfileArticle>()
				.Property(e => e.Visibility)
				.IsUnicode(false);

			modelBuilder.Entity<ProfileArticle>()
				.Property(e => e.Authors)
				.IsUnicode(false);

			modelBuilder.Entity<ProfileArticle>()
				.Property(e => e.WebHookRaw)
				.IsUnicode(false);

			modelBuilder.Entity<ProfileArticle>()
				.HasMany(e => e.ProfileArticleTags)
				.WithRequired(e => e.ProfileArticle)
				.HasForeignKey(e => e.ArticleId);

			modelBuilder.Entity<ProfileImage>()
				.Property(e => e.Guid)
				.IsUnicode(false);

			modelBuilder.Entity<ProfileImage>()
				.Property(e => e.Type)
				.IsUnicode(false);

			modelBuilder.Entity<ProfileImage>()
				.Property(e => e.MimeType)
				.IsUnicode(false);

			modelBuilder.Entity<ProfileImage>()
				.Property(e => e.Filename)
				.IsUnicode(false);

			modelBuilder.Entity<ProfileLicens>()
				.Property(e => e.LicenseNumber)
				.IsUnicode(false);

			modelBuilder.Entity<ProfileLink>()
				.Property(e => e.Type)
				.IsUnicode(false);

			modelBuilder.Entity<ProfileLink>()
				.Property(e => e.Url)
				.IsUnicode(false);

			modelBuilder.Entity<ProfileRating>()
				.Property(e => e.Rating)
				.HasPrecision(5, 2);

			modelBuilder.Entity<ProfileRating>()
				.Property(e => e.Comments)
				.IsUnicode(false);

			modelBuilder.Entity<ProfileRating>()
				.Property(e => e.IP)
				.IsUnicode(false);

			modelBuilder.Entity<Profile>()
				.Property(e => e.Title)
				.IsUnicode(false);

			modelBuilder.Entity<Profile>()
				.Property(e => e.Phone)
				.IsUnicode(false);

			modelBuilder.Entity<Profile>()
				.Property(e => e.Fax)
				.IsUnicode(false);

			modelBuilder.Entity<Profile>()
				.Property(e => e.Email)
				.IsUnicode(false);

			modelBuilder.Entity<Profile>()
				.Property(e => e.YearsExp)
				.IsUnicode(false);

			modelBuilder.Entity<Profile>()
				.Property(e => e.Engagements)
				.IsUnicode(false);

			modelBuilder.Entity<Profile>()
				.Property(e => e.EdLevel)
				.IsUnicode(false);

			modelBuilder.Entity<Profile>()
				.Property(e => e.SchoolName)
				.IsUnicode(false);

			modelBuilder.Entity<Profile>()
				.Property(e => e.DegreeMajor)
				.IsUnicode(false);

			modelBuilder.Entity<Profile>()
				.Property(e => e.ServiceFocus)
				.IsUnicode(false);

			modelBuilder.Entity<Profile>()
				.Property(e => e.Availability)
				.IsUnicode(false);

			modelBuilder.Entity<Profile>()
				.Property(e => e.CommunicationPref)
				.IsUnicode(false);

			modelBuilder.Entity<Profile>()
				.Property(e => e.ServiceStates)
				.IsUnicode(false);

			modelBuilder.Entity<Profile>()
				.Property(e => e.FeeType)
				.IsUnicode(false);

			modelBuilder.Entity<Profile>()
				.Property(e => e.AvgRating)
				.HasPrecision(6, 3);

			modelBuilder.Entity<Profile>()
				.Property(e => e.Type)
				.IsUnicode(false);

			modelBuilder.Entity<Profile>()
				.Property(e => e.Vanity)
				.IsUnicode(false);

			modelBuilder.Entity<Profile>()
				.Property(e => e.Guid)
				.IsUnicode(false);

			modelBuilder.Entity<Profile>()
				.Property(e => e.PaymentAccount)
				.IsUnicode(false);

			modelBuilder.Entity<Profile>()
				.Property(e => e.PaymentAccountRefreshToken)
				.IsUnicode(false);

			modelBuilder.Entity<Profile>()
				.HasMany(e => e.ProfileRatings)
				.WithRequired(e => e.Profile)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<ProfileView>()
				.Property(e => e.Ip)
				.IsUnicode(false);

			modelBuilder.Entity<ProfileView>()
				.Property(e => e.Type)
				.IsUnicode(false);

			modelBuilder.Entity<Role>()
				.Property(e => e.Name)
				.IsUnicode(false);

			modelBuilder.Entity<Role>()
				.HasMany(e => e.UserRoles)
				.WithRequired(e => e.Role)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<SectorType>()
				.Property(e => e.Id)
				.IsFixedLength()
				.IsUnicode(false);

			modelBuilder.Entity<SectorType>()
				.Property(e => e.Name)
				.IsUnicode(false);

			modelBuilder.Entity<SectorType>()
				.HasMany(e => e.ContentItems)
				.WithRequired(e => e.SectorType)
				.HasForeignKey(e => e.Sector)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<ServiceType>()
				.Property(e => e.Name)
				.IsUnicode(false);

			modelBuilder.Entity<ServiceType>()
				.Property(e => e.SubCategory)
				.IsUnicode(false);

			modelBuilder.Entity<ServiceType>()
				.HasMany(e => e.ProfileServiceTypes)
				.WithRequired(e => e.ServiceType)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<SocialLogin>()
				.Property(e => e.SocialName)
				.IsUnicode(false);

			modelBuilder.Entity<SocialLogin>()
				.Property(e => e.Token)
				.IsUnicode(false);

			modelBuilder.Entity<SocialLogin>()
				.Property(e => e.SocialId)
				.IsUnicode(false);

			modelBuilder.Entity<User>()
				.Property(e => e.FirstName)
				.IsUnicode(false);

			modelBuilder.Entity<User>()
				.Property(e => e.LastName)
				.IsUnicode(false);

			modelBuilder.Entity<User>()
				.Property(e => e.Email)
				.IsUnicode(false);

			modelBuilder.Entity<User>()
				.Property(e => e.Password)
				.IsUnicode(false);

			modelBuilder.Entity<User>()
				.Property(e => e.Timezone)
				.IsUnicode(false);

			modelBuilder.Entity<User>()
				.Property(e => e.Ip)
				.IsUnicode(false);

			modelBuilder.Entity<User>()
				.Property(e => e.Key)
				.IsUnicode(false);

			modelBuilder.Entity<User>()
				.HasMany(e => e.ProfileRatings)
				.WithOptional(e => e.User)
				.WillCascadeOnDelete();

			modelBuilder.Entity<User>()
				.HasMany(e => e.UserValidations)
				.WithRequired(e => e.User)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<UserValidation>()
				.Property(e => e.Token)
				.IsUnicode(false);

			modelBuilder.Entity<UserValidation>()
				.Property(e => e.Process)
				.IsUnicode(false);

			modelBuilder.Entity<ProfileArticleTag>().Property(e => e.ProductTypeId).HasColumnName("ProductTypeId");
		}
	}
}
