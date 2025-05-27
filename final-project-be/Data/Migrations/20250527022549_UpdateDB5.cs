using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace final_project_be.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answer_Question_QuestionId",
                table: "Answer");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignment_Lesson_LessonId",
                table: "Assignment");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseCoupon_Coupon_CouponId",
                table: "CourseCoupon");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseCoupon_Courses_CourseId",
                table: "CourseCoupon");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Mentor_MentorId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Lesson_Module_ModuleId",
                table: "Lesson");

            migrationBuilder.DropForeignKey(
                name: "FK_Mentor_users_UserId",
                table: "Mentor");

            migrationBuilder.DropForeignKey(
                name: "FK_MentorCertificate_Mentor_MentorId",
                table: "MentorCertificate");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Mentor_MentorsMentorId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Module_Courses_CourseId",
                table: "Module");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentPlan_MembershipPlan_PlanId",
                table: "PaymentPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentPlan_Payment_PaymentId",
                table: "PaymentPlan");

            migrationBuilder.DropForeignKey(
                name: "FK_Question_Lesson_LessonId",
                table: "Question");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportEvent_Event_EventId",
                table: "ReportEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportEvent_report_ReportId",
                table: "ReportEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Courses_CourseId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_users_UserId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Mentor_MentorId",
                table: "Schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswer_Answer_AnswerId",
                table: "UserAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswer_UserLesson_UserLessonId",
                table: "UserAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignment_Assignment_AssignmentId",
                table: "UserAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignment_users_UserId",
                table: "UserAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCourse_Courses_CourseId",
                table: "UserCourse");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCourse_users_UserId",
                table: "UserCourse");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLesson_Lesson_LessonId",
                table: "UserLesson");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLesson_users_UserId",
                table: "UserLesson");

            migrationBuilder.DropForeignKey(
                name: "FK_UserModule_Module_ModuleId",
                table: "UserModule");

            migrationBuilder.DropForeignKey(
                name: "FK_UserModule_users_UserId",
                table: "UserModule");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSchedule_Schedule_ScheduleId",
                table: "UserSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSchedule_users_UserId",
                table: "UserSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkShop_Mentor_MentorId",
                table: "WorkShop");

            migrationBuilder.DropTable(
                name: "Certificate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSchedule",
                table: "UserSchedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserModule",
                table: "UserModule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLesson",
                table: "UserLesson");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCourse",
                table: "UserCourse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAssignment",
                table: "UserAssignment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAnswer",
                table: "UserAnswer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Schedule",
                table: "Schedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Review",
                table: "Review");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportEvent",
                table: "ReportEvent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentPlan",
                table: "PaymentPlan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Module",
                table: "Module");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MentorCertificate",
                table: "MentorCertificate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mentor",
                table: "Mentor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MembershipPlan",
                table: "MembershipPlan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lesson",
                table: "Lesson");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Event",
                table: "Event");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseCoupon",
                table: "CourseCoupon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Answer",
                table: "Answer");

            migrationBuilder.RenameTable(
                name: "UserSchedule",
                newName: "UserSchedules");

            migrationBuilder.RenameTable(
                name: "UserModule",
                newName: "UserModules");

            migrationBuilder.RenameTable(
                name: "UserLesson",
                newName: "UserLessons");

            migrationBuilder.RenameTable(
                name: "UserCourse",
                newName: "UserCourses");

            migrationBuilder.RenameTable(
                name: "UserAssignment",
                newName: "UserAssignments");

            migrationBuilder.RenameTable(
                name: "UserAnswer",
                newName: "UserAnswers");

            migrationBuilder.RenameTable(
                name: "Schedule",
                newName: "Schedules");

            migrationBuilder.RenameTable(
                name: "Review",
                newName: "Reviews");

            migrationBuilder.RenameTable(
                name: "ReportEvent",
                newName: "ReportEvents");

            migrationBuilder.RenameTable(
                name: "PaymentPlan",
                newName: "PaymentPlans");

            migrationBuilder.RenameTable(
                name: "Module",
                newName: "Modules");

            migrationBuilder.RenameTable(
                name: "MentorCertificate",
                newName: "MentorCertificates");

            migrationBuilder.RenameTable(
                name: "Mentor",
                newName: "Mentors");

            migrationBuilder.RenameTable(
                name: "MembershipPlan",
                newName: "MembershipPlans");

            migrationBuilder.RenameTable(
                name: "Lesson",
                newName: "Lessons");

            migrationBuilder.RenameTable(
                name: "Event",
                newName: "Events");

            migrationBuilder.RenameTable(
                name: "CourseCoupon",
                newName: "courseCoupons");

            migrationBuilder.RenameTable(
                name: "Answer",
                newName: "Answers");

            migrationBuilder.RenameIndex(
                name: "IX_UserSchedule_UserId",
                table: "UserSchedules",
                newName: "IX_UserSchedules_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserModule_UserId",
                table: "UserModules",
                newName: "IX_UserModules_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLesson_UserId",
                table: "UserLessons",
                newName: "IX_UserLessons_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLesson_LessonId",
                table: "UserLessons",
                newName: "IX_UserLessons_LessonId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCourse_UserId",
                table: "UserCourses",
                newName: "IX_UserCourses_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignment_UserId",
                table: "UserAssignments",
                newName: "IX_UserAssignments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAnswer_AnswerId",
                table: "UserAnswers",
                newName: "IX_UserAnswers_AnswerId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedule_MentorId",
                table: "Schedules",
                newName: "IX_Schedules_MentorId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_UserId",
                table: "Reviews",
                newName: "IX_Reviews_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_CourseId",
                table: "Reviews",
                newName: "IX_Reviews_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportEvent_EventId",
                table: "ReportEvents",
                newName: "IX_ReportEvents_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentPlan_PlanId",
                table: "PaymentPlans",
                newName: "IX_PaymentPlans_PlanId");

            migrationBuilder.RenameIndex(
                name: "IX_Module_CourseId",
                table: "Modules",
                newName: "IX_Modules_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_MentorCertificate_MentorId",
                table: "MentorCertificates",
                newName: "IX_MentorCertificates_MentorId");

            migrationBuilder.RenameIndex(
                name: "IX_Mentor_UserId",
                table: "Mentors",
                newName: "IX_Mentors_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Lesson_ModuleId",
                table: "Lessons",
                newName: "IX_Lessons_ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseCoupon_CourseId",
                table: "courseCoupons",
                newName: "IX_courseCoupons_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Answer_QuestionId",
                table: "Answers",
                newName: "IX_Answers_QuestionId");

            migrationBuilder.AddColumn<string>(
                name: "CourseLength",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoursesImage",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificateLink",
                table: "UserCourses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSchedules",
                table: "UserSchedules",
                columns: new[] { "ScheduleId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserModules",
                table: "UserModules",
                columns: new[] { "ModuleId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLessons",
                table: "UserLessons",
                column: "UserLessonId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCourses",
                table: "UserCourses",
                columns: new[] { "CourseId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAssignments",
                table: "UserAssignments",
                columns: new[] { "AssignmentId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAnswers",
                table: "UserAnswers",
                columns: new[] { "UserLessonId", "AnswerId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Schedules",
                table: "Schedules",
                column: "ScheduleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews",
                column: "ReviewId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportEvents",
                table: "ReportEvents",
                columns: new[] { "ReportId", "EventId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentPlans",
                table: "PaymentPlans",
                columns: new[] { "PaymentId", "PlanId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Modules",
                table: "Modules",
                column: "ModuleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MentorCertificates",
                table: "MentorCertificates",
                column: "MentorCertificateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mentors",
                table: "Mentors",
                column: "MentorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MembershipPlans",
                table: "MembershipPlans",
                column: "PlanId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lessons",
                table: "Lessons",
                column: "LessonId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_courseCoupons",
                table: "courseCoupons",
                columns: new[] { "CouponId", "CourseId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Answers",
                table: "Answers",
                column: "AnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Question_QuestionId",
                table: "Answers",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_Lessons_LessonId",
                table: "Assignment",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "LessonId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_courseCoupons_Coupon_CouponId",
                table: "courseCoupons",
                column: "CouponId",
                principalTable: "Coupon",
                principalColumn: "CouponId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_courseCoupons_Courses_CourseId",
                table: "courseCoupons",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Mentors_MentorId",
                table: "Courses",
                column: "MentorId",
                principalTable: "Mentors",
                principalColumn: "MentorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Modules_ModuleId",
                table: "Lessons",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "ModuleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MentorCertificates_Mentors_MentorId",
                table: "MentorCertificates",
                column: "MentorId",
                principalTable: "Mentors",
                principalColumn: "MentorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mentors_users_UserId",
                table: "Mentors",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Mentors_MentorsMentorId",
                table: "Messages",
                column: "MentorsMentorId",
                principalTable: "Mentors",
                principalColumn: "MentorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_Courses_CourseId",
                table: "Modules",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentPlans_MembershipPlans_PlanId",
                table: "PaymentPlans",
                column: "PlanId",
                principalTable: "MembershipPlans",
                principalColumn: "PlanId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentPlans_Payment_PaymentId",
                table: "PaymentPlans",
                column: "PaymentId",
                principalTable: "Payment",
                principalColumn: "PaymentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Question_Lessons_LessonId",
                table: "Question",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "LessonId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportEvents_Events_EventId",
                table: "ReportEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportEvents_report_ReportId",
                table: "ReportEvents",
                column: "ReportId",
                principalTable: "report",
                principalColumn: "ReportId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Courses_CourseId",
                table: "Reviews",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_users_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Mentors_MentorId",
                table: "Schedules",
                column: "MentorId",
                principalTable: "Mentors",
                principalColumn: "MentorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_Answers_AnswerId",
                table: "UserAnswers",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "AnswerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_UserLessons_UserLessonId",
                table: "UserAnswers",
                column: "UserLessonId",
                principalTable: "UserLessons",
                principalColumn: "UserLessonId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignments_Assignment_AssignmentId",
                table: "UserAssignments",
                column: "AssignmentId",
                principalTable: "Assignment",
                principalColumn: "AssignmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignments_users_UserId",
                table: "UserAssignments",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourses_Courses_CourseId",
                table: "UserCourses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourses_users_UserId",
                table: "UserCourses",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLessons_Lessons_LessonId",
                table: "UserLessons",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "LessonId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLessons_users_UserId",
                table: "UserLessons",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserModules_Modules_ModuleId",
                table: "UserModules",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "ModuleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserModules_users_UserId",
                table: "UserModules",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSchedules_Schedules_ScheduleId",
                table: "UserSchedules",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "ScheduleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSchedules_users_UserId",
                table: "UserSchedules",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkShop_Mentors_MentorId",
                table: "WorkShop",
                column: "MentorId",
                principalTable: "Mentors",
                principalColumn: "MentorId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Question_QuestionId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignment_Lessons_LessonId",
                table: "Assignment");

            migrationBuilder.DropForeignKey(
                name: "FK_courseCoupons_Coupon_CouponId",
                table: "courseCoupons");

            migrationBuilder.DropForeignKey(
                name: "FK_courseCoupons_Courses_CourseId",
                table: "courseCoupons");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Mentors_MentorId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Modules_ModuleId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_MentorCertificates_Mentors_MentorId",
                table: "MentorCertificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Mentors_users_UserId",
                table: "Mentors");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Mentors_MentorsMentorId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Modules_Courses_CourseId",
                table: "Modules");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentPlans_MembershipPlans_PlanId",
                table: "PaymentPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentPlans_Payment_PaymentId",
                table: "PaymentPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_Question_Lessons_LessonId",
                table: "Question");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportEvents_Events_EventId",
                table: "ReportEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportEvents_report_ReportId",
                table: "ReportEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Courses_CourseId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_users_UserId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Mentors_MentorId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_Answers_AnswerId",
                table: "UserAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_UserLessons_UserLessonId",
                table: "UserAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignments_Assignment_AssignmentId",
                table: "UserAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignments_users_UserId",
                table: "UserAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCourses_Courses_CourseId",
                table: "UserCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCourses_users_UserId",
                table: "UserCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLessons_Lessons_LessonId",
                table: "UserLessons");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLessons_users_UserId",
                table: "UserLessons");

            migrationBuilder.DropForeignKey(
                name: "FK_UserModules_Modules_ModuleId",
                table: "UserModules");

            migrationBuilder.DropForeignKey(
                name: "FK_UserModules_users_UserId",
                table: "UserModules");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSchedules_Schedules_ScheduleId",
                table: "UserSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSchedules_users_UserId",
                table: "UserSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkShop_Mentors_MentorId",
                table: "WorkShop");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSchedules",
                table: "UserSchedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserModules",
                table: "UserModules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLessons",
                table: "UserLessons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCourses",
                table: "UserCourses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAssignments",
                table: "UserAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAnswers",
                table: "UserAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Schedules",
                table: "Schedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportEvents",
                table: "ReportEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentPlans",
                table: "PaymentPlans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Modules",
                table: "Modules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mentors",
                table: "Mentors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MentorCertificates",
                table: "MentorCertificates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MembershipPlans",
                table: "MembershipPlans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lessons",
                table: "Lessons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_courseCoupons",
                table: "courseCoupons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Answers",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "CourseLength",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CoursesImage",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CertificateLink",
                table: "UserCourses");

            migrationBuilder.RenameTable(
                name: "UserSchedules",
                newName: "UserSchedule");

            migrationBuilder.RenameTable(
                name: "UserModules",
                newName: "UserModule");

            migrationBuilder.RenameTable(
                name: "UserLessons",
                newName: "UserLesson");

            migrationBuilder.RenameTable(
                name: "UserCourses",
                newName: "UserCourse");

            migrationBuilder.RenameTable(
                name: "UserAssignments",
                newName: "UserAssignment");

            migrationBuilder.RenameTable(
                name: "UserAnswers",
                newName: "UserAnswer");

            migrationBuilder.RenameTable(
                name: "Schedules",
                newName: "Schedule");

            migrationBuilder.RenameTable(
                name: "Reviews",
                newName: "Review");

            migrationBuilder.RenameTable(
                name: "ReportEvents",
                newName: "ReportEvent");

            migrationBuilder.RenameTable(
                name: "PaymentPlans",
                newName: "PaymentPlan");

            migrationBuilder.RenameTable(
                name: "Modules",
                newName: "Module");

            migrationBuilder.RenameTable(
                name: "Mentors",
                newName: "Mentor");

            migrationBuilder.RenameTable(
                name: "MentorCertificates",
                newName: "MentorCertificate");

            migrationBuilder.RenameTable(
                name: "MembershipPlans",
                newName: "MembershipPlan");

            migrationBuilder.RenameTable(
                name: "Lessons",
                newName: "Lesson");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "Event");

            migrationBuilder.RenameTable(
                name: "courseCoupons",
                newName: "CourseCoupon");

            migrationBuilder.RenameTable(
                name: "Answers",
                newName: "Answer");

            migrationBuilder.RenameIndex(
                name: "IX_UserSchedules_UserId",
                table: "UserSchedule",
                newName: "IX_UserSchedule_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserModules_UserId",
                table: "UserModule",
                newName: "IX_UserModule_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLessons_UserId",
                table: "UserLesson",
                newName: "IX_UserLesson_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLessons_LessonId",
                table: "UserLesson",
                newName: "IX_UserLesson_LessonId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCourses_UserId",
                table: "UserCourse",
                newName: "IX_UserCourse_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignments_UserId",
                table: "UserAssignment",
                newName: "IX_UserAssignment_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAnswers_AnswerId",
                table: "UserAnswer",
                newName: "IX_UserAnswer_AnswerId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_MentorId",
                table: "Schedule",
                newName: "IX_Schedule_MentorId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_UserId",
                table: "Review",
                newName: "IX_Review_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_CourseId",
                table: "Review",
                newName: "IX_Review_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_ReportEvents_EventId",
                table: "ReportEvent",
                newName: "IX_ReportEvent_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentPlans_PlanId",
                table: "PaymentPlan",
                newName: "IX_PaymentPlan_PlanId");

            migrationBuilder.RenameIndex(
                name: "IX_Modules_CourseId",
                table: "Module",
                newName: "IX_Module_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Mentors_UserId",
                table: "Mentor",
                newName: "IX_Mentor_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_MentorCertificates_MentorId",
                table: "MentorCertificate",
                newName: "IX_MentorCertificate_MentorId");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_ModuleId",
                table: "Lesson",
                newName: "IX_Lesson_ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_courseCoupons_CourseId",
                table: "CourseCoupon",
                newName: "IX_CourseCoupon_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_QuestionId",
                table: "Answer",
                newName: "IX_Answer_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSchedule",
                table: "UserSchedule",
                columns: new[] { "ScheduleId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserModule",
                table: "UserModule",
                columns: new[] { "ModuleId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLesson",
                table: "UserLesson",
                column: "UserLessonId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCourse",
                table: "UserCourse",
                columns: new[] { "CourseId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAssignment",
                table: "UserAssignment",
                columns: new[] { "AssignmentId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAnswer",
                table: "UserAnswer",
                columns: new[] { "UserLessonId", "AnswerId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Schedule",
                table: "Schedule",
                column: "ScheduleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Review",
                table: "Review",
                column: "ReviewId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportEvent",
                table: "ReportEvent",
                columns: new[] { "ReportId", "EventId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentPlan",
                table: "PaymentPlan",
                columns: new[] { "PaymentId", "PlanId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Module",
                table: "Module",
                column: "ModuleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mentor",
                table: "Mentor",
                column: "MentorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MentorCertificate",
                table: "MentorCertificate",
                column: "MentorCertificateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MembershipPlan",
                table: "MembershipPlan",
                column: "PlanId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lesson",
                table: "Lesson",
                column: "LessonId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Event",
                table: "Event",
                column: "EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseCoupon",
                table: "CourseCoupon",
                columns: new[] { "CouponId", "CourseId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Answer",
                table: "Answer",
                column: "AnswerId");

            migrationBuilder.CreateTable(
                name: "Certificate",
                columns: table => new
                {
                    CertificateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CertificateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssuedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificate", x => x.CertificateId);
                    table.ForeignKey(
                        name: "FK_Certificate_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Certificate_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_CourseId",
                table: "Certificate",
                column: "CourseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_UserId",
                table: "Certificate",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_Question_QuestionId",
                table: "Answer",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_Lesson_LessonId",
                table: "Assignment",
                column: "LessonId",
                principalTable: "Lesson",
                principalColumn: "LessonId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseCoupon_Coupon_CouponId",
                table: "CourseCoupon",
                column: "CouponId",
                principalTable: "Coupon",
                principalColumn: "CouponId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseCoupon_Courses_CourseId",
                table: "CourseCoupon",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Mentor_MentorId",
                table: "Courses",
                column: "MentorId",
                principalTable: "Mentor",
                principalColumn: "MentorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lesson_Module_ModuleId",
                table: "Lesson",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "ModuleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mentor_users_UserId",
                table: "Mentor",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MentorCertificate_Mentor_MentorId",
                table: "MentorCertificate",
                column: "MentorId",
                principalTable: "Mentor",
                principalColumn: "MentorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Mentor_MentorsMentorId",
                table: "Messages",
                column: "MentorsMentorId",
                principalTable: "Mentor",
                principalColumn: "MentorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Module_Courses_CourseId",
                table: "Module",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentPlan_MembershipPlan_PlanId",
                table: "PaymentPlan",
                column: "PlanId",
                principalTable: "MembershipPlan",
                principalColumn: "PlanId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentPlan_Payment_PaymentId",
                table: "PaymentPlan",
                column: "PaymentId",
                principalTable: "Payment",
                principalColumn: "PaymentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Question_Lesson_LessonId",
                table: "Question",
                column: "LessonId",
                principalTable: "Lesson",
                principalColumn: "LessonId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportEvent_Event_EventId",
                table: "ReportEvent",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportEvent_report_ReportId",
                table: "ReportEvent",
                column: "ReportId",
                principalTable: "report",
                principalColumn: "ReportId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Courses_CourseId",
                table: "Review",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_users_UserId",
                table: "Review",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Mentor_MentorId",
                table: "Schedule",
                column: "MentorId",
                principalTable: "Mentor",
                principalColumn: "MentorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswer_Answer_AnswerId",
                table: "UserAnswer",
                column: "AnswerId",
                principalTable: "Answer",
                principalColumn: "AnswerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswer_UserLesson_UserLessonId",
                table: "UserAnswer",
                column: "UserLessonId",
                principalTable: "UserLesson",
                principalColumn: "UserLessonId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignment_Assignment_AssignmentId",
                table: "UserAssignment",
                column: "AssignmentId",
                principalTable: "Assignment",
                principalColumn: "AssignmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignment_users_UserId",
                table: "UserAssignment",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourse_Courses_CourseId",
                table: "UserCourse",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourse_users_UserId",
                table: "UserCourse",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLesson_Lesson_LessonId",
                table: "UserLesson",
                column: "LessonId",
                principalTable: "Lesson",
                principalColumn: "LessonId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLesson_users_UserId",
                table: "UserLesson",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserModule_Module_ModuleId",
                table: "UserModule",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "ModuleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserModule_users_UserId",
                table: "UserModule",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSchedule_Schedule_ScheduleId",
                table: "UserSchedule",
                column: "ScheduleId",
                principalTable: "Schedule",
                principalColumn: "ScheduleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSchedule_users_UserId",
                table: "UserSchedule",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkShop_Mentor_MentorId",
                table: "WorkShop",
                column: "MentorId",
                principalTable: "Mentor",
                principalColumn: "MentorId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
