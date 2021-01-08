using System.Collections.Generic;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Common
{
    public static class FileExtensionMapper
    {
        private static readonly IReadOnlyDictionary<string, FileExtensionType> FileExtensionMap
            = new Dictionary<string, FileExtensionType>
            {
                { FileExtensionType.Pdf.ToLower(), FileExtensionType.Pdf },
                { FileExtensionType.Docx.ToLower(), FileExtensionType.Docx },
                { FileExtensionType.Xlsx.ToLower(), FileExtensionType.Xlsx },
                { FileExtensionType.Pptx.ToLower(), FileExtensionType.Pptx },
                { FileExtensionType.Jpeg.ToLower(), FileExtensionType.Jpeg },
                { FileExtensionType.Jpg.ToLower(), FileExtensionType.Jpg },
                { FileExtensionType.Gif.ToLower(), FileExtensionType.Gif },
                { FileExtensionType.Png.ToLower(), FileExtensionType.Png },
                { FileExtensionType.Svg.ToLower(), FileExtensionType.Svg },
                { FileExtensionType.Mp3.ToLower(), FileExtensionType.Mp3 },
                { FileExtensionType.Ogg.ToLower(), FileExtensionType.Ogg },
                { FileExtensionType.Mp4.ToLower(), FileExtensionType.Mp4 },
                { FileExtensionType.M4v.ToLower(), FileExtensionType.M4v },
                { FileExtensionType.Ogv.ToLower(), FileExtensionType.Ogv },
                { FileExtensionType.Scorm.ToLower(), FileExtensionType.Scorm },
                { FileExtensionType.Zip.ToLower(), FileExtensionType.Zip }
            };

        public static FileExtensionType? MapFromFileExtension(string type)
        {
            return FileExtensionMap[type];
        }

        private static string ToLower(this FileExtensionType type)
        {
            return type.ToString().ToLower();
        }
    }
}
