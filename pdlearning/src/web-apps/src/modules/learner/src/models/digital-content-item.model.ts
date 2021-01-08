import { DigitalContentStatus, DigitalContentType, MyDigitalContentStatus } from '@opal20/domain-api';
import { ILearningItemModel, LearningType } from './learning-item.model';

import { IMyDigitalContentDetail } from './my-digital-content-detail.model';
import { getDigitalContenType } from '../learner-utils';

export interface IDigitalContentItemModel extends ILearningItemModel {
  id: string;
  originalId: string;
  title: string;
  imageUrl: string | undefined;
  isBookmark: boolean;
  createdDate?: Date | undefined;
  uploadedDate: Date;
  type: LearningType;
  itemType: DigitalContentType;
  status: DigitalContentStatus;
  learningStatus: MyDigitalContentStatus;
  tags: string[];
  rating: number;
  reviewsCount: number;
}
export class DigitalContentItemModel implements IDigitalContentItemModel {
  public id: string;
  public originalId: string;
  public title: string;
  public imageUrl: string | undefined;
  public isBookmark: boolean;
  public createdDate?: Date | undefined;
  public uploadedDate: Date = new Date();
  public type: LearningType = LearningType.DigitalContent;
  public status: DigitalContentStatus;
  public learningStatus: MyDigitalContentStatus;
  public itemType: DigitalContentType;
  public tags: string[];
  public rating: number;
  public reviewsCount: number;
  public static createDigitalContentItemModel(myDcDetail: IMyDigitalContentDetail): DigitalContentItemModel {
    return new DigitalContentItemModel({
      type: LearningType.DigitalContent,
      id: myDcDetail.digitalContent.id,
      originalId: myDcDetail.digitalContent.originalObjectId,
      title: myDcDetail.digitalContent.title,
      imageUrl: `assets/images/icons/sm/${getDigitalContenType(myDcDetail.digitalContent)}.svg`,
      isBookmark: !!myDcDetail.bookmarkInfo,
      createdDate: myDcDetail.createdDate ? new Date(myDcDetail.createdDate) : undefined,
      uploadedDate: myDcDetail.digitalContent.createdDate ? new Date(myDcDetail.digitalContent.createdDate) : new Date(),
      itemType: myDcDetail.digitalContent.type as DigitalContentType,
      status: myDcDetail.digitalContent && myDcDetail.digitalContent.status,
      learningStatus: myDcDetail.myDigitalContent && myDcDetail.myDigitalContent.status,
      tags: [getDigitalContenType(myDcDetail.digitalContent)],
      rating: myDcDetail.rating,
      reviewsCount: myDcDetail.reviewsCount
    });
  }

  constructor(data?: IDigitalContentItemModel) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.originalId = data.originalId;
    this.title = data.title;
    this.imageUrl = data.imageUrl;
    this.isBookmark = data.isBookmark;
    this.createdDate = data.createdDate ? new Date(data.createdDate) : undefined;
    this.uploadedDate = data.uploadedDate;
    this.status = data.status;
    this.learningStatus = data.learningStatus;
    this.itemType = data.itemType;
    this.tags = data.tags;
    this.rating = data.rating;
    this.reviewsCount = data.reviewsCount;
  }
}
