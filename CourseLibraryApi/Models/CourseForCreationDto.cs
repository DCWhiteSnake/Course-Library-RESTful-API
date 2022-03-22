using CourseLibraryApi.ValidationAttributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseLibraryApi.Models
{
    public class CourseForCreationDto : CourseForManipulationDto
    {
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