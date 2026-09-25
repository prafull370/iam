using System.ComponentModel;
using System.Data;
using ClosedXML.Excel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UtilityLayer.Helper.Convert;

namespace UtilityLayer.Helper.Export
{

    public static class ExportHelper
    {
        public static byte[] Export_ListToExcel<T>(List<T>? data)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                if (data != null && data.Count > 0)
                {
                    var dataTable = ConvertHelper.Convert_ListToDataTable(data);
                    worksheet.Cell(1, 1).InsertTable(dataTable);
                }
                else
                {
                    // If data is empty, create headers only
                    var properties = typeof(T).GetProperties();
                    for (int i = 0; i < properties.Length; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = properties[i].Name; // Set header names
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray(); // Return file as byte array
                }
            }
        }
        public static byte[] Export_Datatable_ToExcel(DataTable data)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                if (data != null && data.Rows.Count > 0)
                {
                    worksheet.Cell(1, 1).InsertTable(data);
                }
                else
                {
                    worksheet.Cell(1, 1).Value = "No record found";
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray(); // Return file as byte array
                }
            }
        }
        public static List<Dictionary<string, string>> ReadExcel(string filePath)
        {
            var result = new List<Dictionary<string, string>>();

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            IWorkbook workbook;

            if (filePath.EndsWith(".xlsx"))
                workbook = new XSSFWorkbook(fs);
            else
                workbook = new HSSFWorkbook(fs); // .xls

            var sheet = workbook.GetSheetAt(0);

            var headerRow = sheet.GetRow(0);
            int colCount = headerRow.LastCellNum;

            var headers = new List<string>();
            for (int i = 0; i < colCount; i++)
                headers.Add(headerRow.GetCell(i)?.ToString().Trim());

            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row == null) continue;

                var dict = new Dictionary<string, string>();

                for (int j = 0; j < colCount; j++)
                {
                    var cell = row.GetCell(j);
                    dict[headers[j]] = cell?.ToString().Trim();
                }

                result.Add(dict);
            }

            return result;
        }
        public static List<T> Map<T>(List<Dictionary<string, string>> rows) where T : new()
        {
            var list = new List<T>();
            var props = typeof(T).GetProperties();

            foreach (var row in rows)
            {
                var obj = new T();

                foreach (var prop in props)
                {
                    if (row.ContainsKey(prop.Name))
                        prop.SetValue(obj, row[prop.Name]);
                }

                list.Add(obj);
            }

            return list;
        }
    }
}
