using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper;
using System.Globalization;

namespace Task_Backend.Utils
{
    public class DateConverter : ITypeConverter
    {
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (DateTime.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date.Date;
            }
            else
            {
                throw new InvalidOperationException($"Nieprawidłowy format daty: {text}");
            }
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value is DateTime date)
            {
                return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            else
            {
                throw new InvalidOperationException($"Nieoczekiwana wartość: {value}");
            }
        }
    }
}
