using System;

namespace API.Extensions;

public static class DateTimeExtension
{
    public static int CalculateAge(this DateOnly dob)
    {
       var tody = DateOnly.FromDateTime(DateTime.Now);

       var age = tody.Year - dob.Year;

       if(dob > tody.AddYears(-age)) age--;
       return age;
    }
}
