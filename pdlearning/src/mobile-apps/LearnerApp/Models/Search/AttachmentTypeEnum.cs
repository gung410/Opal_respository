using System.Collections.Generic;
using System.Linq;

namespace LearnerApp.Models.Search
{
    public enum AttachmentTypeEnum
    {
        DocumentFiles,
        DigitalGraphics,
        Audio,
        Video,
        InteractiveObjects,
        LearningPackages
    }

    public static class AttachmentTypeEnumHelpers
    {
        private static readonly Dictionary<AttachmentTypeEnum, string> _apiValueDictionary = new Dictionary<AttachmentTypeEnum, string>()
        {
            { AttachmentTypeEnum.DocumentFiles, "DocumentFiles" },
            { AttachmentTypeEnum.DigitalGraphics, "DigitalGraphics" },
            { AttachmentTypeEnum.Audio, "Audio" },
            { AttachmentTypeEnum.Video, "Video" },
            { AttachmentTypeEnum.InteractiveObjects, "InteractiveObjects" },
            { AttachmentTypeEnum.LearningPackages, "LearningPackages" },
        };

        private static readonly Dictionary<AttachmentTypeEnum, string> _friendlyStringValueDictionary = new Dictionary<AttachmentTypeEnum, string>()
        {
            { AttachmentTypeEnum.DocumentFiles, "Document Files" },
            { AttachmentTypeEnum.DigitalGraphics, "Digital Graphics" },
            { AttachmentTypeEnum.Audio, "Audio" },
            { AttachmentTypeEnum.Video, "Video" },
            { AttachmentTypeEnum.InteractiveObjects, "Interactive Objects" },
            { AttachmentTypeEnum.LearningPackages, "Learning Packages" },
        };

        public static AttachmentTypeEnum? ToAttachmentTypeSearchTypeEnum(this string category)
        {
            if (category == null)
            {
                return null;
            }

            if (_apiValueDictionary.ContainsValue(category) == false)
            {
                return null;
            }

            return _apiValueDictionary.FirstOrDefault(x => x.Value == category).Key;
        }

        public static string ToApiString(this AttachmentTypeEnum attachmentType)
        {
            if (_apiValueDictionary.ContainsKey(attachmentType) == false)
            {
                return null;
            }

            return _apiValueDictionary[attachmentType];
        }

        public static string ToFriendlyString(this AttachmentTypeEnum attachmentType)
        {
            if (_friendlyStringValueDictionary.ContainsKey(attachmentType) == false)
            {
                return null;
            }

            return _friendlyStringValueDictionary[attachmentType];
        }
    }
}
