using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class Courses
    {
        [Key]
        public int CourseId { get; set; }
        [ForeignKey("Mentor")]
        public int MentorId { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set;}
        public string CourseName { get; set;}
        public string CourseContent { get; set;}
        public string Cost {  get; set;}
        public string SkillLearn {  get; set;}
        public int StudentCount { get; set;}
        public DateTime? CreateAt { get; set;}
        public DateTime? UpdateAt { get; set;}
        public ICollection<CourseCoupon>? CourseCoupons { get; set;}
        public ICollection<PaymentCourse>? PaymentCourses { get; set;}
        public ICollection<Review>? Reviews { get; set;}
        public ICollection<UserCourse> UserCourses { get; set;}
        public ICollection<Module> Modules { get; set;}
        public Certificate? Certificate { get; set; }
        public Mentor? Mentor { get; set;}
        public Category?  Category { get; set;}

    }
}
