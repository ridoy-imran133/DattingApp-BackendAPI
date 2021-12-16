using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-error");
            response.Headers.Add("Access-control-Allow-Origin", "**");
        }

        //public static int CalculateAge(this DateTime? dateTime)
        //{
        //    if (dateTime == null)
        //    {
        //        var age = DateTime.Today.Year - dateTime.Year;

        //        if (dateTime.AddYears(age) > DateTime.Today)
        //            age--;

        //        return age;
        //    }
        //}
    }
}
