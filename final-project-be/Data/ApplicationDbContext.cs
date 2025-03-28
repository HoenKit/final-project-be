using final_project_be.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace final_project_be.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Category> categories {  get; set; }
        public DbSet<SubCategory> subcategories { get; set; }
        public DbSet<Comment> comments { get; set; }
        public DbSet<PollOption> pollOptions { get; set; }
        public DbSet<PollOptionVote> pollVotes { get; set; }
        public DbSet<Notification> notification { get; set; }
        public DbSet<Post> posts { get; set; }
        public DbSet<User> users { get; set;}
        public DbSet<UserRole> userRoles { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<PostFile>postFiles { get; set; }
        public DbSet<Report> report { get; set; }
        public DbSet<ReportComment> reportComments { get; set; }
        public DbSet<ReportPost> reportPost { get; set; }
        public DbSet<ReportUser> reportUser { get; set; }
        public DbSet<UserMetadata> UserMetadata { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var config = new ConfigurationBuilder()
        //         .SetBasePath(Directory.GetCurrentDirectory())
        //         .AddJsonFile("appsettings.json").Build();

        //    optionsBuilder.UseSqlServer(config.GetConnectionString("MyConnection"));
        //}
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                   .HasOne(u => u.UserMetaData)
                   .WithOne(um => um.User)
                   .HasForeignKey<UserMetadata>(um => um.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SubCategory>()
                   .HasOne(sc => sc.Category)
                   .WithMany(c => c.SubCategories)
                   .HasForeignKey(sc => sc.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure Post and SubCategory relationship
            builder.Entity<Post>()
                .HasOne(p => p.SubCategory)
                .WithMany(sc => sc.Posts)
                .HasForeignKey(p => p.SubCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Post and User relationship
            builder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Comment relationships
            builder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Comments)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure PollOption and Post relationship
            builder.Entity<PollOption>()
                .HasOne(po => po.Post)
                .WithMany(p => p.PollOptions)
                .HasForeignKey(po => po.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure PollOptionVote relationships
            builder.Entity<PollOptionVote>()
                .HasOne(pov => pov.PollOption)
                .WithMany(po => po.PollOptionVotes)
                .HasForeignKey(pov => pov.PollOptionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PollOptionVote>()
                .HasOne(pov => pov.User)
                .WithMany(u => u.PollOptionVotes)
                .HasForeignKey(pov => pov.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Notification and User relationship
            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Report relationships
            builder.Entity<ReportComment>()
                .HasKey(rc => new { rc.ReportId, rc.CommentId });

            builder.Entity<ReportComment>()
                .HasOne(rc => rc.Report)
                .WithMany(r => r.ReportComments)
                .HasForeignKey(rc => rc.ReportId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ReportComment>()
                .HasOne(rc => rc.Comment)
                .WithMany(c => c.ReportComments)
                .HasForeignKey(rc => rc.CommentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ReportPost>()
                .HasKey(rp => new { rp.ReportId, rp.PostId });

            builder.Entity<ReportPost>()
                .HasOne(rp => rp.Report)
                .WithMany(r => r.ReportPosts)
                .HasForeignKey(rp => rp.ReportId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ReportPost>()
                .HasOne(rp => rp.Post)
                .WithMany(p => p.ReportPosts)
                .HasForeignKey(rp => rp.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ReportUser>()
                .HasKey(ru => new { ru.ReportId, ru.UserId });

            builder.Entity<ReportUser>()
                .HasOne(ru => ru.Report)
                .WithMany(r => r.ReportUsers)
                .HasForeignKey(ru => ru.ReportId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ReportUser>()
                .HasOne(ru => ru.User)
                .WithMany(u => u.ReportUsers)
                .HasForeignKey(ru => ru.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure UserRole relationships
            builder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            builder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
