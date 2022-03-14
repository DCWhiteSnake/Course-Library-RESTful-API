using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibraryApi.Helpers
{
    public static class DateTimeOffsetExtensions
    {
        public static int GetCurrentAge(this DateTimeOffset dob)
        {
            var currentDate = DateTime.UtcNow;
            int age = currentDate.Year - dob.Year;

            if (currentDate < dob.AddYears(age))
            {
                --age;
            }
            return age;
        }
    }
}