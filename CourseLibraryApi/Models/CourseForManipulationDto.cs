using CourseLibraryApi.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibraryApi.Models
{
    [CourseTitleMustBeDifferentFromDescription(ErrorMessage = "Title must be different from Description")]
    public abstract class CourseForManipulationDto
    {
        [Required(ErrorMessage = "Must input Course Title")]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Must input Course Description")]
        [MaxLength(1500)]
        public virtual string Description { get; set; }
    }
}