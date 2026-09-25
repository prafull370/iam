using System.Data;

namespace UtilityLayer.Helper.Convert
{
    public static class ConvertHelper
    {
        public static DataTable Convert_ListToDataTable<T>(List<T> data)
        {
            DataTable table = new DataTable(typeof(T).Name);
            var props = typeof(T).GetProperties();
            foreach (var prop in props)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (var item in data)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(values);
            }

            return table;
        }
        //Convert File in WebP and Compress it
       
    }
}
