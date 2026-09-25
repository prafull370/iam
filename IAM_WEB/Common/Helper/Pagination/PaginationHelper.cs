using ModelLayer.Models.Common;

namespace IAM_WEB.Common.Helper.Pagination
{
    public class PaginationHelper
    {
        public static void PagingData(IFormCollection page, ref PaginationModel pageBase)
        {
            // Find paging info
            pageBase.Start = Convert.ToInt32(page["start"].FirstOrDefault());
            pageBase.Length = Convert.ToInt32(page["length"].FirstOrDefault());

            // Find and convert sort column name to lowercase
            string columnIndex = page["order[0][column]"].FirstOrDefault();
            string columnName = page[$"columns[{columnIndex}][name]"].FirstOrDefault();
            pageBase.SortColumn = columnName?.ToLower();

            // Get sort direction
            pageBase.SortColumnDirection = page["order[0][dir]"].FirstOrDefault();
        }


    }
}