export interface IQuestionOptionImageUploadSettings {
  allowedUploadImage: boolean;
  imageUrl: string;
}

export class QuestionOptionImageUploadSettings implements IQuestionOptionImageUploadSettings {
  public allowedUploadImage: boolean = false;
  public imageUrl: string = '';

  public setImageUrl(imageUrl: string): QuestionOptionImageUploadSettings {
    this.imageUrl = imageUrl;
    return this;
  }
}
