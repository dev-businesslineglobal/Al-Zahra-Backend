using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GardeningAPI.Helper
{
    public class HelperService
    {
        #region CSV Generation and Download Link
        public string GenerateCsvFile<T>(List<T> items, string fileName)
        {
            if (items == null || items.Count == 0)
                return string.Empty;

            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string outFilePath = Path.Combine(projectDirectory, "files");

            if (!Directory.Exists(outFilePath))
                Directory.CreateDirectory(outFilePath);

            string imagesFolder = Path.Combine(outFilePath, "images");
            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            foreach (dynamic item in items)
            {
                if (!string.IsNullOrWhiteSpace(item.PictureBase64))
                {
                    string imageFileName = $"{item.ItemCode}.png"; // or jpg depending on your data
                    string imagePath = Path.Combine(imagesFolder, imageFileName);
                    byte[] imageBytes = Convert.FromBase64String(item.PictureBase64);
                    File.WriteAllBytes(imagePath, imageBytes);

                    item.PictureBase64 = imageFileName;
                }
            }

            string finalName = $"{fileName}.csv";
            string filePath = Path.Combine(outFilePath, finalName);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                ShouldQuote = args => true,
                TrimOptions = TrimOptions.None,
                BadDataFound = null,
                IgnoreBlankLines = true
            };

            // Write CSV
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(items);
            }

            return "fileName=" + finalName;
        }



        #endregion


        internal string ConvertCodeToLanguage(string? code)
        {
            return code switch
            {
                "3" => "English",
                "32" => "Arabic",
                "40" => "Kurdish",
                _ => "English" // default
            };
        }

        internal int ConvertLanguageToCode(string language)
        {
            return language?.Trim().ToLower() switch
            {
                "english" => 3,
                "arabic" => 32,
                "kurdish" => 40,
                _ => 3   // default English
            };
        }



    }
}
