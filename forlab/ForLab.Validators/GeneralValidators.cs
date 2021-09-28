using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ForLab.Validators
{
    public class GeneralValidators
    {
        public GeneralValidators()
        {

        }
        public bool IsPhoneValid(string input)
        {
            if (Regex.IsMatch(input, @"^[a-zA-Z]+$")) // Only letters
            {
                return false;
            }
            return true;
        }
        public bool IsValidUrl(string input)
        {
            string pattern = @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return reg.IsMatch(input);
        }
        public bool IsValidDate(string input, string format = "MMMM-yy")
        {
            // MMMM-yy => January-20

            DateTime result;
            return DateTime.TryParseExact(input, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out result);
        }
    }
}
