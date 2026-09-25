using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityLayer.Interfaces
{
	public interface IGenerateAvatarService
	{
		Task<string> GenerateLetterAvatar(string name, string fileExtension = "png");
	}
}
