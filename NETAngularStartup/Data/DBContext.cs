using AngleSharp.Css;
using AngleSharp.Dom;
using DevCodeArchitect.Entity;
using DevCodeArchitect.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection.Emit;


namespace DevCodeArchitect.DBContext
{
    public class ApplicationDBContext :
            IdentityDbContext<ApplicationUser,
            ApplicationRole,
            string,
            IdentityUserClaim<string>,
            ApplicationUserRole,
            IdentityUserLogin<string>,
            IdentityRoleClaim<string>,
            IdentityUserToken<string>>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        // Field Settings e.g
        public string Table_Prefix { get; set; } = "Dev_";
        public int UserId_Length { get; set; } = 80;
        public int Description_Length { get; set; } = 300;
        public int Url_Length { get; set; } = 200;
        public int Title_Length { get; set; } = 200;
        public int Tags_Length { get; set; } = 500;
        public int Slug_Length { get; set; } = 50;

        /*private string GetCurrentTimestampSql()
        {
            return Database.ProviderName switch
            {
                "Microsoft.EntityFrameworkCore.SqlServer" => "GETUTCDATE()",
                "Npgsql.EntityFrameworkCore.PostgreSQL" => "NOW() AT TIME ZONE 'UTC'",
                "Microsoft.EntityFrameworkCore.Sqlite" => "CURRENT_TIMESTAMP",
                "Pomelo.EntityFrameworkCore.MySql" => "UTC_TIMESTAMP()",
                _ => "CURRENT_TIMESTAMP"
            };
        }*/

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Then use in configuration:
            //var currentTimestampSql = GetCurrentTimestampSql();

            // Customize DBContext => Table Field Attributes, Default Values, Mappings, Max Values etc

            // Application Role Entity
            builder.Entity<ApplicationRole>(b =>
            {
                b.Property(u => u.Id).HasMaxLength(this.UserId_Length);
                b.Property(u => u.IPAddress).HasMaxLength(20);
                b.Property(u => u.Description).HasMaxLength(this.Description_Length);

                // default values
                b.Property(u => u.Type).HasDefaultValue(RoleEnum.Types.Admin);
            });

            // ApplicationUserRole Entity
            builder.Entity<ApplicationUserRole>(b =>
            {
                b.HasKey(u => new { u.UserId, u.RoleId });
            });

            // ApplicationUser Entity
            builder.Entity<ApplicationUser>(b =>
            {
                b.Property(u => u.Id).HasMaxLength(this.UserId_Length);
                b.Property(u => u.Slug).HasMaxLength(30);
                b.Property(u => u.FirstName).HasMaxLength(50);
                b.Property(u => u.LastName).HasMaxLength(50);
                b.Property(u => u.Avatar).HasMaxLength(this.Url_Length);
                b.Property(u => u.UserRole).HasMaxLength(80);

                // default value
                b.Property(u => u.IsFeatured).HasDefaultValue(Types.FeaturedTypes.Basic);
                b.Property(u => u.IsEnabled).HasDefaultValue(Types.ActionTypes.Enabled);
                b.Property(u => u.Type).HasDefaultValue(UserEnum.UserTypes.NormalUser);

            });

            // Blogs -> Table Settings
            builder.Entity<Blogs>(b =>
            {
                // Primary Key
                b.HasKey(u => u.Id);
                // Map with prefix table
                b.ToTable(this.Table_Prefix + "Blogs");

                b.Property(u => u.UserId).HasMaxLength(this.UserId_Length);
                b.Property(u => u.Term).HasMaxLength(this.Title_Length);
                b.Property(u => u.Tags).HasMaxLength(this.Tags_Length);
                b.Property(u => u.Cover).HasMaxLength(this.Url_Length);

                // default values
                b.Property(u => u.IsEnabled).HasDefaultValue(Types.ActionTypes.Enabled);
                b.Property(u => u.IsApproved).HasDefaultValue(Types.ActionTypes.Enabled);
                b.Property(u => u.IsDraft).HasDefaultValue(Types.DraftTypes.Normal);
                b.Property(u => u.IsArchive).HasDefaultValue(Types.ActionTypes.Disabled);
                b.Property(u => u.IsFeatured).HasDefaultValue(Types.FeaturedTypes.Basic);

                b.Property(u => u.Views).HasDefaultValue(0);
                b.Property(u => u.Comments).HasDefaultValue(0);

            });

            builder.Entity<BlogData>(b =>
            {
                // Primary Key
                b.HasKey(u => u.Id);
                // Map with prefix table
                b.ToTable(this.Table_Prefix + "BlogData");

                b.Property(u => u.Title).HasMaxLength(this.Title_Length);
                b.Property(u => u.Culture).HasMaxLength(10);
            });

            // conventional mapping ApplicaitonUser => Blogs table
            builder.Entity<ApplicationUser>()
                .HasMany(e => e.Blogs)
                .WithOne(e => e.Author)
                .HasForeignKey(e => e.UserId)
                .IsRequired();

            // Categories -> Table Settings
            builder.Entity<Categories>(b =>
            {
                // Primary Key
                b.HasKey(u => u.Id);
                // Map with prefix table
                b.ToTable(this.Table_Prefix + "Categories");

                //b.Property(u => u.title).HasMaxLength(this.Title_Length);
                //b.Property(u => u.sub_title).HasMaxLength(this.Title_Length);
                b.Property(u => u.Term).HasMaxLength(this.Title_Length);
                b.Property(u => u.SubTerm).HasMaxLength(this.Title_Length);
                b.Property(u => u.Avatar).HasMaxLength(this.Url_Length);

                // default values
                b.Property(u => u.ParentId).HasDefaultValue(0);
                b.Property(u => u.Type).HasDefaultValue(CategoryEnum.Types.Blogs);
                b.Property(u => u.Priority).HasDefaultValue(0);
                b.Property(u => u.IsEnabled).HasDefaultValue(Types.ActionTypes.Enabled);
                b.Property(u => u.IsFeatured).HasDefaultValue(Types.FeaturedTypes.Basic);

                /* b.Property(u => u.CreatedAt)
                     .HasDefaultValueSql(currentTimestampSql)
                     .ValueGeneratedOnAdd();

                 b.Property(u => u.UpdatedAt)
                     .HasDefaultValueSql(currentTimestampSql)
                     .ValueGeneratedOnAddOrUpdate();*/

                b.Property(u => u.Records).HasDefaultValue(0);

            });

            builder.Entity<CategoryData>(b =>
            {
                // Primary Key
                b.HasKey(u => u.Id);
                // Map with prefix table
                b.ToTable(this.Table_Prefix + "CategoryData");

                b.Property(u => u.Title).HasMaxLength(this.Title_Length);
                b.Property(u => u.SubTitle).HasMaxLength(this.Title_Length);
                b.Property(u => u.Culture).HasMaxLength(10);
            });

            // CategoryContents -> Table Settings
            builder.Entity<CategoryContents>(b =>
            {
                // Primary Key
                b.HasKey(u => u.Id);
                // Map with prefix table
                b.ToTable(this.Table_Prefix + "CategoryContents");

                // default values
                b.Property(u => u.CategoryId).HasDefaultValue(0);
                b.Property(u => u.ContentId).HasDefaultValue(0);
                b.Property(u => u.Type).HasDefaultValue(CategoryEnum.Types.Blogs);

            });

            // ErrorLogs -> Table Settings
            builder.Entity<ErrorLogs>(b =>
            {
                // Primary Key
                b.HasKey(u => u.Id);
                // Map with prefix table
                b.ToTable(this.Table_Prefix + "ErrorLogs");

                b.Property(u => u.Description).HasMaxLength(this.Description_Length);
                b.Property(u => u.Url).HasMaxLength(this.Url_Length);


            });

            // Tags -> Table Settings
            builder.Entity<Tags>(b =>
            {
                // Primary Key
                b.HasKey(u => u.Id);
                // Map with prefix table
                b.ToTable(this.Table_Prefix + "Tags");

                b.Property(u => u.Title).HasMaxLength(this.Title_Length);
                b.Property(u => u.Term).HasMaxLength(this.Title_Length);
                // default values
                b.Property(u => u.IsEnabled).HasDefaultValue(Types.ActionTypes.Enabled);
                b.Property(u => u.TagLevel).HasDefaultValue(TagEnum.TagLevel.Medium);
                b.Property(u => u.TagType).HasDefaultValue(TagEnum.TagType.Normal);
                b.Property(u => u.Type).HasDefaultValue(TagEnum.Types.Blog);
                b.Property(u => u.Records).HasDefaultValue(0);
            });

        
        }

        public virtual DbSet<ApplicationUser> AspNetUsers { get; set; }
        public virtual DbSet<ApplicationRole> AspNetRoles { get; set; }
        public virtual DbSet<ApplicationUserRole> AspNetUserRoles { get; set; }

        public virtual DbSet<Blogs> Blogs { get; set; }
        public virtual DbSet<BlogData> BlogData { get; set; }

        public virtual DbSet<Categories> Categories { get; set; }

        public virtual DbSet<CategoryData> CategoryData { get; set; }
        public virtual DbSet<CategoryContents> CategoryContents { get; set; }

        public virtual DbSet<ErrorLogs> ErrorLogs { get; set; }

        public virtual DbSet<Tags> Tags { get; set; }


    }
}
