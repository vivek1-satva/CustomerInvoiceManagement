using System.Reflection;
using System.Text;
using DataAccess.Model;
namespace InvoiceCustomerAuthAPI.Utitlity
{
    public static class CommonFunction
    {
        public static string ToCsv<T>(this List<T> items, string delimiter = ",")
        {
            Type itemType = typeof(T);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(p => p.Name);

            var csv = new StringBuilder();

            // Write Headers
            csv.AppendLine(string.Join(delimiter, props.Select(p => p.Name)));

            // Write Rows
            foreach (var item in items)
            {
                // Write Fields
                csv.AppendLine(string.Join(delimiter, props.Select(p => GetCsvFieldasedOnValue(p, item))));
            }

            return csv.ToString();
        }


        private static object GetCsvFieldasedOnValue<T>(PropertyInfo p, T item)
        {
            string value = "";

            try
            {
                value = p.GetValue(item, null)?.ToString();
                if (value == null) return "NULL";  // Deal with nulls
                if (value.Trim().Length == 0) return ""; // Deal with spaces and blanks

                // Guard strings with "s, they may contain the delimiter!
                if (p.PropertyType == typeof(string))
                {
                    value = string.Format("\"{0}\"", value);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;
        }

    }
    
               
}

