import { UploadParameters, Utils } from '@opal20/infrastructure';

import { CropImageDialogComponent } from '../components/crop-image-dialog/crop-image-dialog.component';
import { OpalDialogService } from '../services/dialog/dialog.service';

export class FileUploaderUtils {
  public static graphicsExtensions: string[] = ['jpeg', 'jpg', 'gif', 'png', 'svg'];
  public static videoExtensions: string[] = ['m4v', 'ogv'];
  public static audioExtensions: string[] = ['mp3', 'ogg', 'mp4'];
  public static documentExtensions: string[] = ['pdf', 'docx', 'xlsx', 'pptx', 'zip'];

  public static uploadContentAllowedExtensions: string[] = [
    ...FileUploaderUtils.documentExtensions,
    ...FileUploaderUtils.graphicsExtensions,
    ...FileUploaderUtils.videoExtensions,
    ...FileUploaderUtils.audioExtensions
  ];

  public static allowedExtensions: string[] = [
    ...FileUploaderUtils.graphicsExtensions,
    ...FileUploaderUtils.videoExtensions,
    ...FileUploaderUtils.audioExtensions
  ];

  public static allowedAnswerAttachmentExts: string[] = [...FileUploaderUtils.videoExtensions, ...FileUploaderUtils.audioExtensions];

  public static wrappingFile(fileParameter: UploadParameters, width: string, height: string): string {
    const src = `${AppGlobal.environment.cloudfrontUrl}/${fileParameter.fileLocation}`;
    switch (fileParameter.fileExtension) {
      case 'jpeg':
      case 'jpg':
      case 'gif':
      case 'png':
      case 'svg':
        return `<img width="${width}" height="${height}" src="${src}" style="max-width:100%" />`;
      case 'mp3':
      case 'ogg':
        return `<audio width="${width}" height="${height}" style="max-width:100%" controls controlsList="nodownload">
                  <source type="audio/ogg"
                          src="${src}">
                  <source type="audio/mpeg"
                          src="${src}">
                  <source type="audio/wav"
                          src="${src}">
                </audio>`;
      case 'mp4':
      case 'm4v':
      case 'ogv':
        return `<video width="${width}"
                        height="${height}"
                        style="max-width:100%"
                        controls
                        controlsList="nodownload">
                  <source type="video/mp4"
                          src="${src}">
                  <source type="video/ogg"
                          src="${src}">
                  <source type="video/m4v"
                          src="${src}">
                  <source type="video/avi"
                          src="${src}">
                </video>`;
      default:
        return null;
    }
  }

  public static canCropImage(fileExtension: string, canCropImage: boolean = true): boolean {
    return (
      canCropImage &&
      Utils.includesAll(FileUploaderUtils.graphicsExtensions, [fileExtension]) &&
      fileExtension !== 'svg' &&
      fileExtension !== 'gif'
    );
  }

  public static showCropImageDialog(
    file: File,
    opalServ: OpalDialogService,
    callback: (file: File) => Promise<File | void>
  ): Promise<File | void> {
    const dialogRef = opalServ.openDialogRef(CropImageDialogComponent, {
      file: file,
      aspectRatio: 16 / 9,
      maintainAspectRatio: true
    });

    return dialogRef.result.toPromise().then(
      (cropedfile: File): Promise<File | void> => {
        if (callback) {
          return callback(cropedfile);
        }
      }
    );
  }

  public static exceedLimitFileSize(file: File, sizeInMb: number): boolean {
    return file.size > sizeInMb * 1024 * 1024;
  }
}
