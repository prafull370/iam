using Microsoft.Extensions.Configuration;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using UtilityLayer.Interfaces;

namespace UtilityLayer.Services
{
    public class GenerateAvatarService : IGenerateAvatarService
    {
        private readonly IConfiguration _config;
        public GenerateAvatarService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> GenerateLetterAvatar(string name, string fileExtension = "png")
        {
            // Default colors for avatar background
            var colors = new[] { "#FF5722", "#9C27B0", "#3F51B5", "#009688", "#795548" };
            var firstChar = string.IsNullOrEmpty(name) ? "?" : name[0].ToString().ToUpper();
            var bgColorHex = colors[firstChar[0] % colors.Length];
            Color bgColor = Color.ParseHex(bgColorHex);

            const int size = 200;

            using var image = new Image<Rgba32>(size, size);

            // Solid background fill (poora square, no circle — jaisa original code)
            image.Mutate(ctx => ctx.Clear(bgColor));

            // Font
            Font font;
            try
            {
                font = SystemFonts.CreateFont("Arial", 80, FontStyle.Bold);
            }
            catch
            {
                // Linux fallback — Arial available nahi hone par system ka pehla available font use karo
                font = SystemFonts.CreateFont(SystemFonts.Families.First().Name, 80, FontStyle.Bold);
            }

            // Draw letter (center mein)
            image.Mutate(ctx =>
            {
                var textOptions = new RichTextOptions(font)
                {
                    Origin = new PointF(size / 2f, size / 2f),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                ctx.DrawText(textOptions, firstChar, Color.White);
            });

            // Generate file path using your utility
            UtilityLayer.Helper.FileDirectory.GenerateFilePath(
                destFilePath: _config["AppSettings:DestinationFilePath"],
                GlobalPath: "~/Files/UserLogo/",
                FolderPath: string.Empty,
                FileName: "User_Image",
                FileExtension: fileExtension,
                FullFilePath: out string fullfilepath,
                AbsoulteFilePath: out string absoultefilepath,
                ReturnFileName: out string returnfilename
            );

            // Save the image
            await image.SaveAsPngAsync(fullfilepath);

            return "/Assets/Files/UserLogo" + absoultefilepath;
        }
    }
}