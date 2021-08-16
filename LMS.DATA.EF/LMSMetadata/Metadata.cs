using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LMS.DATA.EF
{
    public class CourseMetadata
    {


        //public int CourseId { get; set; } -- PK

        [Required(ErrorMessage = "Course Name is required")]
        [StringLength(300, ErrorMessage = "Entry must be 300 characters or less")]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }


        [StringLength(500, ErrorMessage = "Entry must be 500 characters or less")]
        [Display(Name = "Course Description")]
        [UIHint("MultilineText")]
        public string CourseDescription { get; set; }

        [Required(ErrorMessage = "Course status is required")]
        [Display(Name = "Is Course Active?")]
        public bool IsActive { get; set; }

        [StringLength(200, ErrorMessage = "Entry must be 200 characters or less")]
        [Display(Name = "Course Image")]
        public string CourseImg { get; set; }
    }
    [MetadataType(typeof(CourseMetadata))]
    public partial class Course
    {
       public bool hasCompleted
        {
            get;
            set;
        }
    }

    public class CourseCompletionMetadata
    {


        //public int CourseCompletionId { get; set; } --PK

        [Required(ErrorMessage = "User ID is required")]
        [StringLength(128, ErrorMessage = "Entry must be 128 characters or less")]
        [Display(Name = "User ID")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Course ID is required")]
        [Display(Name = "Course ID")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "A date/time is required")]
        [Display(Name = "Date of Completion")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public System.DateTime DateCompleted { get; set; }

    }
    [MetadataType(typeof(CourseCompletionMetadata))]
    public partial class CourseCompletion
    {

    }

    public class LessonMetadata
    {
        //public int LessonId { get; set; } -- PK 

        [Required(ErrorMessage = "Lesson Title is required")]
        [StringLength(200, ErrorMessage = "Entry must be 200 characters or less")]
        [Display(Name = "Lesson Title")]
        public string LessonTitle { get; set; }

        [Required(ErrorMessage = "Course ID is required")]
        [Display(Name = "Course ID")]
        public int CourseId { get; set; }

        [StringLength(300, ErrorMessage = "Entry must be 300 characters or less")]
        [Display(Name = "Lesson Description")]
        [UIHint("MultilineText")]
        public string LessonDesc { get; set; }

        [StringLength(300, ErrorMessage = "Entry must be 300 characters or less")]
        [Display(Name = "Video URL")]
        public string VideoURL { get; set; }

        [StringLength(300, ErrorMessage = "Entry must be 300 characters or less")]
        [Display(Name = "PDF File")]
        public string PdfFile { get; set; }

        [Required(ErrorMessage = "Lesson Status is required")]
        [Display(Name = "Is Lesson Active?")]
        public bool IsActive { get; set; }
    }
    [MetadataType(typeof(LessonMetadata))]
    public partial class Lesson
    {
        public bool hasCompleted
        {
            get;
            set;
        }
    }

    public class LessonViewMetadata
    {

        //public int LessonViewedId { get; set; } -- PK

        [Required(ErrorMessage = "User ID is required")]
        [StringLength(128, ErrorMessage = "Entry must be 128 characters or less")]
        [Display(Name = "User ID")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Lesson ID is required")]
        [Display(Name = "Lesson ID")]
        public int LessonId { get; set; }

        [Required(ErrorMessage = "Date Viewed is required")]
        [Display(Name = "Date Viewed")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public System.DateTime DateViewed { get; set; }

    }

    [MetadataType(typeof(LessonViewMetadata))]
    public partial class LessonView
    {

    }

    public class UserDetailMetadata
    {

        //public string UserId { get; set; } -- PK

        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "Entry must be 50 characters or less")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "Entry must be 50 characters or less")]
        public string LastName { get; set; }
    }
    [MetadataType(typeof(UserDetailMetadata))]
    public partial class UserDetail
    {
        [Display(Name = "Full Name")]
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
    }


}//end namespace
