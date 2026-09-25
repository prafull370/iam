using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityLayer.Helper
{
	public class FileDirectory
	{
		#region Create Directory
		public static void CreateDirectory(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}
		#endregion

		#region Generate File Path
		public static string UnixTime()
		{
			Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			return unixTimestamp.ToString();
		}
		public static void GenerateFilePath(string destFilePath, string GlobalPath, string FolderPath, string FileName, string FileExtension, out string? FullFilePath, out string? AbsoulteFilePath, out string? ReturnFileName, bool unixTimeFlag = true)
		{
			GlobalPath = GlobalPath.Trim('~');
			FullFilePath = null;
			AbsoulteFilePath = null;

			string absoluteFolderPath = GlobalPath;

			if (!string.IsNullOrWhiteSpace(FolderPath))
				absoluteFolderPath = (GlobalPath + "/" + FolderPath).Replace("\\", "/");

			string fullFolderPath = (destFilePath + absoluteFolderPath).Replace("\\", "/");


			CreateDirectory(fullFolderPath);

			if (unixTimeFlag)
			{
				FileName = FileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + FileExtension;
			}
			else
			{
				FileName = FileName + "." + FileExtension;
			}

			ReturnFileName = FileName;
			FullFilePath = fullFolderPath + "/" + FileName;
			AbsoulteFilePath = "/" + FileName;
		}
		#endregion
	}
}
