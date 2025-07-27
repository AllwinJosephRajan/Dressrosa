using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace Dressrosa.Core.Extensions
{
    public static class DateTimeExtension
    {
        /// <param name="request"></param>
        /// <param name="timeZone"></param>

        public static DateTime ConvertUTCToLocalTimeBasedOnTimeZone(this DateTime request, string timeZone)
        {
            try
            {
                //var utcDateTime = Convert.ToDateTime(request);
                TimeZoneInfo nzTimeZone = TZConvert.GetTimeZoneInfo(timeZone);
                DateTime nzDateTime = TimeZoneInfo.ConvertTimeFromUtc(request, nzTimeZone);
                return nzDateTime;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
