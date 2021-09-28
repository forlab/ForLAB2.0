using ForLab.DTO.Common;
using System;
using System.Linq;

namespace ForLab.Services.Global.DataFilter
{
    public class DataFilterService<T> : IDataFilterService<T>
    {
        public DataFilterService()
        {

        }

        public IQueryable<T> Filter(IQueryable<T> data, object filterDto)
        {
            if(filterDto == null)
            {
                return data;
            }

            var baseFilterDto = new BaseFilterDto();
            var skipped = baseFilterDto.GetType().GetProperties().Select(x => x.Name);
            var newFilterDto = filterDto.GetType().GetProperties().Select(x => x.Name).Except(skipped);

            for (int j = 0; j < filterDto.GetType().GetProperties().Length; j++)
            {
                var key = filterDto.GetType().GetProperties()[j].Name;
                var val = filterDto.GetType().GetProperties()[j].GetValue(filterDto, null);

                if(val == null)
                {
                    continue;
                }

                var type = val.GetType();

                if (IsDateTime(val.ToString()))
                {
                    var date = Convert.ToDateTime(val.ToString());
                    //data = data.Where(x => x.)
                }
                else if(IsNumber(val))
                {

                }
                else if (val is bool)
                {

                }
                else if (val is string)
                {

                }
            }

            return data;
        }

        public static bool IsDateTime(string text)
        {
            DateTime dateTime;

            // Check for empty string.
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            return DateTime.TryParse(text, out dateTime);
        }
        public static bool IsNumber(object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }
    }
}
