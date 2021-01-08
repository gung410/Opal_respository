import { FormParticipantStatus, FormStandaloneMode, FormStatus, FormType, IFormParticipantForm } from '@opal20/domain-api';
import { ILearningItemModel, LearningType } from './learning-item.model';

export interface IStandaloneFormItemModel extends ILearningItemModel {
  id: string;
  title: string;
  imageUrl?: string | undefined;
  type: LearningType;
  isBookmark: boolean;

  formId?: string;
  userId?: string;
  assignedDate?: Date;
  submittedDate?: Date;
  formParticipantStatus?: FormParticipantStatus;
  isStarted?: string;

  formType: FormType;
  formStatus: FormStatus;
  originalObjectId: string | undefined;
  isStandalone?: boolean;
  standaloneMode?: FormStandaloneMode;
}
export class StandaloneFormItemModel implements IStandaloneFormItemModel {
  public id: string;
  public title: string;
  public imageUrl?: string | undefined;
  public type: LearningType;
  public isBookmark: boolean = false;

  public formId?: string;
  public userId?: string;
  public assignedDate?: Date;
  public submittedDate?: Date;
  public formParticipantStatus?: FormParticipantStatus;
  public isStarted?: string;

  public formType: FormType;
  public formStatus: FormStatus;
  public originalObjectId: string | undefined;
  public isStandalone?: boolean;
  public standaloneMode?: FormStandaloneMode;

  public static createStandaloneFormItemModel(data: IFormParticipantForm): StandaloneFormItemModel {
    return new StandaloneFormItemModel({
      id: data.form ? data.form.id : undefined,
      title: data.form ? data.form.title : undefined,
      imageUrl: `assets/images/icons/${data.form.type.toLowerCase()}.svg`,
      type: LearningType.StandaloneForm,
      isBookmark: false,
      formId: data.form ? data.form.id : undefined,
      userId: data.formParticipant ? data.formParticipant.userId : undefined,
      assignedDate: data.formParticipant ? data.formParticipant.assignedDate : undefined,
      submittedDate: data.formParticipant ? data.formParticipant.submittedDate : undefined,
      formParticipantStatus: data.formParticipant ? data.formParticipant.status : undefined,
      isStarted: data.formParticipant ? data.formParticipant.isStarted : undefined,
      formType: data.form ? data.form.type : undefined,
      formStatus: data.form ? data.form.status : undefined,
      originalObjectId: data.form.originalObjectId ? data.form.originalObjectId : undefined
    });
  }

  constructor(data?: IStandaloneFormItemModel) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.title = data.title;
    this.imageUrl = data.imageUrl;
    this.type = data.type;
    this.formId = data.formId;
    this.userId = data.userId;
    this.assignedDate = data.assignedDate;
    this.submittedDate = data.submittedDate;
    this.formParticipantStatus = data.formParticipantStatus;
    this.isStarted = data.isStarted;
    this.formType = data.formType;
    this.formStatus = data.formStatus;
    this.originalObjectId = data.originalObjectId;
    this.isStandalone = data.isStandalone;
    this.standaloneMode = data.standaloneMode;
  }
}
