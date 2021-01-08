import { FileType } from "@opal20/domain-api";

export class FileUploaderHelpers {
    public static getFileType(fileExtension: string): FileType {
        switch (fileExtension) {
          case 'pdf':
          case 'docx':
          case 'xlsx':
          case 'pptx':
            return FileType.Document;
          case 'jpeg':
          case 'jpg':
          case 'gif':
          case 'png':
          case 'svg':
            return FileType.DigitalGraphic;
          case 'mp3':
          case 'ogg':
            return FileType.Audio;
          case 'mp4':
          case 'm4v':
          case 'ogv':
            return FileType.Video;
          case 'scorm':
            return FileType.LearningPackage;
          default:
            return;
        }
      }

      public static getFileTypeDisplay(fileType: FileType): string {
        switch (fileType) {
          case FileType.DigitalGraphic:
            return 'Digital Graphic';
          case FileType.LearningPackage:
            return 'Learning Package';
          default:
            return fileType.toString();
        }
      }
}
