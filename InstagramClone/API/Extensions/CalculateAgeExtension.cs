using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class CalculateAgeExtension
    {
        public static int CalculateAge(this DateTime dateOfBirth)
        {
            var today = DateTime.Now;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age))
            {
                // they havent had the bday yet this year
                --age;
            }

            return age;
        }
    }
}
