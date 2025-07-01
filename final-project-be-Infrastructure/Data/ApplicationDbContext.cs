using final_project_be_Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace final_project_be_Infrastructure.Data
{
	public class ApplicationDbContext : DbContext
	{
		public DbSet<UserWorkshop> UserWorkshop { get; set; }
		public DbSet<Review> Reviews { get; set; }
		public DbSet<Answer> Answers { get; set; }
		public DbSet<Assignment> Assignment { get; set; }
		public DbSet<Coupon> Coupon { get; set; }
		public DbSet<CourseCoupon> courseCoupons { get; set; }
		public DbSet<Courses> Courses { get; set; }
		public DbSet<Event> Events { get; set; }
		public DbSet<Lesson> Lessons { get; set; }
		public DbSet<MembershipPlan> MembershipPlans { get; set; }
		public DbSet<Mentor> Mentors { get; set; }
		public DbSet<MentorCertificate> MentorCertificates { get; set; }
		public DbSet<Messages> Messages { get; set; }
		public DbSet<Module> Modules { get; set; }
		public DbSet<Payment> Payment { get; set; }
		public DbSet<PaymentCourse> PaymentCourse { get; set; }
		public DbSet<PaymentPlan> PaymentPlans { get; set; }
		public DbSet<Question> Question { get; set; }
		public DbSet<ReportEvent> ReportEvents { get; set; }
		public DbSet<Schedule> Schedules { get; set; }
		public DbSet<Transaction> Transaction { get; set; }
		public DbSet<UserAnswer> UserAnswers { get; set; }
		public DbSet<UserAssignment> UserAssignments { get; set; }
		public DbSet<UserCourse> UserCourses { get; set; }
		public DbSet<UserLesson> UserLessons { get; set; }
		public DbSet<UserModule> UserModules { get; set; }
		public DbSet<UserSchedule> UserSchedules { get; set; }
		public DbSet<WorkShop> WorkShop { get; set; }
		public DbSet<Category> categories { get; set; }
		public DbSet<Comment> comments { get; set; }
		public DbSet<PollOption> pollOptions { get; set; }
		public DbSet<PollOptionVote> pollVotes { get; set; }
		public DbSet<Notification> notification { get; set; }
		public DbSet<Post> posts { get; set; }
		public DbSet<User> users { get; set; }
		public DbSet<UserRole> userRoles { get; set; }
		public DbSet<Role> roles { get; set; }
		public DbSet<PostFile> postFiles { get; set; }
		public DbSet<Report> report { get; set; }
		public DbSet<ReportComment> reportComments { get; set; }
		public DbSet<ReportPost> reportPost { get; set; }
		public DbSet<ReportUser> reportUser { get; set; }
		public DbSet<UserMetadata> UserMetadata { get; set; }
        public DbSet<CourseEmbedding> CourseEmbeddings { get; set; }
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

			builder.Entity<User>()
				   .HasOne(u => u.Mentor)
				   .WithOne(um => um.User)
				   .HasForeignKey<Mentor>(um => um.UserId)
				   .OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Category>()
				.HasOne(c => c.ParentCategory)
				.WithMany(c => c.Categories)
				.HasForeignKey(c => c.ParentCategoryId)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure Post and Category relationship
			builder.Entity<Post>()
				.HasOne(p => p.Category)
				.WithMany(sc => sc.Posts)
				.HasForeignKey(p => p.CategoryId)
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


			// Configure MembershipPlan
			builder.Entity<MembershipPlan>()
				.Property(r => r.Price)
				.HasColumnType("decimal(18,4)");

			// Configure Payment
			builder.Entity<Payment>()
				.Property(r => r.Amount)
				.HasColumnType("decimal(18,4)");

			// Configure Courses
			builder.Entity<Courses>()
				.Property(r => r.Cost)
				.HasColumnType("decimal(18,4)");

			// Configure Transaction
			builder.Entity<Transaction>()
				.Property(r => r.Points)
				.HasColumnType("decimal(18,4)");

			builder.Entity<Transaction>()
				.Property(r => r.Amount)
				.HasColumnType("decimal(18,4)");

			// Configure User Point
			builder.Entity<User>()
				.Property(r => r.Point)
				.HasColumnType("decimal(18,4)");

			// Configure Review and User relationship
			builder.Entity<Review>()
				.Property(r => r.Rate)
				.HasColumnType("decimal(18,4)");

			builder.Entity<Review>()
				.HasOne(n => n.Courses)
				.WithMany(u => u.Reviews)
				.HasForeignKey(n => n.CourseId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Review>()
				   .HasOne(r => r.User)
				   .WithMany(u => u.Reviews)
				   .HasForeignKey(r => r.UserId)
				   .OnDelete(DeleteBehavior.Restrict);

			// Configure Lesson and Assignment relationship
			builder.Entity<Assignment>()
				.HasOne(n => n.Lesson)
				.WithMany(u => u.Assignments)
				.HasForeignKey(n => n.LessonId)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure MentorCertificate and Mentor relationship
			builder.Entity<MentorCertificate>()
				.HasOne(n => n.Mentor)
				.WithMany(u => u.MentorCertificates)
				.HasForeignKey(n => n.MentorId)
				.OnDelete(DeleteBehavior.Restrict);


			// Configure Messages and User relationship
			builder.Entity<Messages>()
				.HasOne(n => n.User)
				.WithMany(u => u.Messages)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure Messages and Mentors relationship
			builder.Entity<Messages>()
				.HasOne(n => n.Mentors)
				.WithMany(u => u.Messages)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure Transaction and User relationship
			builder.Entity<Transaction>()
				.HasOne(n => n.Users)
				.WithMany(u => u.Transactions)
				.HasForeignKey(n => n.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure Module and Lession relationship
			builder.Entity<Lesson>()
				.HasOne(n => n.Module)
				.WithMany(u => u.Lessons)
				.HasForeignKey(n => n.ModuleId)
				.OnDelete(DeleteBehavior.Restrict);



			// Configure Course and Module relationship
			builder.Entity<Module>()
				.HasOne(n => n.Courses)
				.WithMany(u => u.Modules)
				.HasForeignKey(n => n.CourseId)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure Schedule and Mentor relationship
			builder.Entity<Schedule>()
				.HasOne(n => n.Mentor)
				.WithMany(u => u.Schedules)
				.HasForeignKey(n => n.MentorId)
				.OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Schedule>()
				.HasOne(n => n.Courses)
				.WithMany(u => u.schedules)
				.HasForeignKey(n => n.CourseId)
				.OnDelete(DeleteBehavior.Restrict);


            // Configure Payment relationship
            builder.Entity<Payment>()
				.HasOne(n => n.User)
				.WithMany(u => u.Payments)
				.HasForeignKey(n => n.UserId)
				.OnDelete(DeleteBehavior.Restrict);


			// Configure WorkShop and Mentor relationship
			builder.Entity<WorkShop>()
				.HasOne(n => n.Mentor)
				.WithMany(u => u.WorkShops)
				.HasForeignKey(n => n.MentorId)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure PaymentPlan relationship
			builder.Entity<PaymentPlan>()
				.HasKey(pc => new { pc.PaymentId, pc.PlanId });
			builder.Entity<PaymentPlan>()
				.HasOne(n => n.Payment)
				.WithMany(u => u.PaymentPlans)
				.HasForeignKey(n => n.PaymentId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<PaymentPlan>()
				.HasOne(n => n.MembershipPlan)
				.WithMany(u => u.PaymentPlans)
				.HasForeignKey(n => n.PlanId)
				.OnDelete(DeleteBehavior.Restrict);


			// Configure PaymentCourse relationships
			builder.Entity<PaymentCourse>()
				.HasKey(pc => new { pc.PaymentId, pc.CourseId });

			builder.Entity<PaymentCourse>()
				.HasOne(pc => pc.Payment)
				.WithMany(p => p.PaymentCourses)
				.HasForeignKey(pc => pc.PaymentId);

			builder.Entity<PaymentCourse>()
				.HasOne(pc => pc.Courses)
				.WithMany(c => c.PaymentCourses)
				.HasForeignKey(pc => pc.CourseId);

			builder.Entity<PaymentCourse>()
				.HasOne(pc => pc.Coupon)
				.WithMany(c => c.PaymentCourses)
				.HasForeignKey(pc => pc.CouponId);

			// Configure CourseCoupon relationships
			builder.Entity<CourseCoupon>()
				.HasKey(cp => new { cp.CouponId, cp.CourseId });

			builder.Entity<CourseCoupon>()
				.HasOne(rc => rc.Courses)
				.WithMany(r => r.CourseCoupons)
				.HasForeignKey(rc => rc.CourseId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<CourseCoupon>()
				.HasOne(rc => rc.Coupons)
				.WithMany(c => c.CourseCoupon)
				.HasForeignKey(rc => rc.CouponId)
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

			builder.Entity<ReportWorkShop>()
				.HasKey(rc => new { rc.ReportId, rc.WorkshopId });

			builder.Entity<ReportWorkShop>()
				.HasOne(rc => rc.Report)
				.WithMany(r => r.ReportWorkShops)
				.HasForeignKey(rc => rc.ReportId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<ReportWorkShop>()
				.HasOne(rc => rc.WorkShop)
				.WithMany(c => c.ReportWorkShops)
				.HasForeignKey(rc => rc.WorkshopId)
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


			builder.Entity<ReportEvent>()
				.HasKey(rp => new { rp.ReportId, rp.EventId });

			builder.Entity<ReportEvent>()
				.HasOne(rp => rp.Report)
				.WithMany(r => r.ReportEvents)
				.HasForeignKey(rp => rp.ReportId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<ReportEvent>()
				.HasOne(rp => rp.Event)
				.WithMany(p => p.ReportEvents)
				.HasForeignKey(rp => rp.EventId)
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


			// Configure UserCourse relationships
			builder.Entity<UserAssignment>()
				.HasKey(ru => new { ru.AssignmentId, ru.UserId });

			builder.Entity<UserAssignment>()
				.HasOne(ru => ru.User)
				.WithMany(r => r.UserAssignments)
				.HasForeignKey(ru => ru.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<UserAssignment>()
				.HasOne(uc => uc.Assignment)
				.WithMany(c => c.UserAssignments)
				.HasForeignKey(ru => ru.AssignmentId)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure UserCourse relationships
			builder.Entity<UserCourse>()
				.HasKey(ru => new { ru.CourseId, ru.UserId });

			builder.Entity<UserCourse>()
				.HasOne(ru => ru.User)
				.WithMany(r => r.UserCourses)
				.HasForeignKey(ru => ru.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<UserCourse>()
				.HasOne(uc => uc.Courses)
				.WithMany(c => c.UserCourses)
				.HasForeignKey(ru => ru.CourseId)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure UserModule relationships
			builder.Entity<UserModule>()
				.HasKey(ru => new { ru.ModuleId, ru.UserId });

			builder.Entity<UserModule>()
				.HasOne(ru => ru.User)
				.WithMany(r => r.UserModules)
				.HasForeignKey(ru => ru.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<UserModule>()
				.HasOne(uc => uc.Module)
				.WithMany(c => c.UserModules)
				.HasForeignKey(ru => ru.ModuleId)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure UserLesson relationships

			builder.Entity<UserLesson>()
				.HasOne(ru => ru.User)
				.WithMany(r => r.UserLessons)
				.HasForeignKey(ru => ru.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<UserLesson>()
				.HasOne(uc => uc.Lesson)
				.WithMany(c => c.UserLesson)
				.HasForeignKey(ru => ru.LessonId)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure UserAnswer relationships
			builder.Entity<UserAnswer>()
				.HasKey(ru => new { ru.UserLessonId, ru.AnswerId });

			builder.Entity<UserAnswer>()
				.HasOne(ru => ru.Answer)
				.WithMany(r => r.UserAnswers)
				.HasForeignKey(ru => ru.AnswerId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<UserAnswer>()
				.HasOne(uc => uc.UserLesson)
				.WithMany(c => c.UserAnswer)
				.HasForeignKey(ru => ru.UserLessonId)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure UserSchedule relationships
			builder.Entity<UserSchedule>()
				.HasKey(ru => new { ru.ScheduleId, ru.UserId });

			builder.Entity<UserSchedule>()
				.HasOne(ru => ru.Schedule)
				.WithMany(r => r.UserSchedules)
				.HasForeignKey(ru => ru.ScheduleId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<UserSchedule>()
				.HasOne(uc => uc.User)
				.WithMany(c => c.UserSchedules)
				.HasForeignKey(ru => ru.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure UserWorkShop relationships
			builder.Entity<UserWorkshop>()
				.HasKey(ru => new { ru.WorkShopId, ru.UserId });

			builder.Entity<UserWorkshop>()
				.HasOne(ru => ru.WorkShop)
				.WithMany(r => r.UserWorkshops)
				.HasForeignKey(ru => ru.WorkShopId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<UserWorkshop>()
				.HasOne(uc => uc.User)
				.WithMany(c => c.UserWorkshops)
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
