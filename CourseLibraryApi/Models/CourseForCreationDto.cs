using CourseLibraryApi.ValidationAttributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseLibraryApi.Models
{
    [CourseTitleMustBeDifferentFromDescription(ErrorMessage = "Title must be different from Description")]
    public class CourseForCreationDto
    {
        [Required(ErrorMessage = "Must input Course Title")]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Must input Course Description")]
        [MaxLength(1500)]
        public string Description { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (Title == Description)
        //    {
        //        yield return new ValidationResult("The Title must not match the description",
        //            new[] { "CourseCreationDto" });
        //    }
        //}
    }
}