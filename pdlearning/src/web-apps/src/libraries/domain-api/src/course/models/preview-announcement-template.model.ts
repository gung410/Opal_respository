export interface IPreviewAnnouncementTemplate {
  message: string;
}

export class PreviewAnnouncementTemplate implements IPreviewAnnouncementTemplate {
  public message: string = '';

  constructor(data?: PreviewAnnouncementTemplate) {
    if (data != null) {
      this.message = data.message;
    }
  }
}
