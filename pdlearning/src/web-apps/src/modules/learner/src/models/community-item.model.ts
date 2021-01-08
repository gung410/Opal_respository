import { ILearningItemModel, LearningType } from './learning-item.model';

import { CommunityResultModel } from '@opal20/domain-api';

export interface ICommunityItemModel extends ILearningItemModel {
  id: string;
  title: string;
  imageUrl?: string | undefined;
  isBookmark: boolean;
  type: LearningType;
  description: string;
  members: number;
  urlDetail: string;
}

export class CommunityItemModel implements ICommunityItemModel {
  public id: string;
  public title: string;
  public imageUrl?: string | undefined;
  public isBookmark: boolean;
  public type: LearningType;
  public description: string;
  public members: number;
  public urlDetail: string;

  public static createCommunityItemModel(communityResult: CommunityResultModel): CommunityItemModel {
    return new CommunityItemModel({
      id: communityResult.guid,
      title: communityResult.name,
      isBookmark: communityResult.isBookmark,
      type: LearningType.Community,
      description: communityResult.description,
      members: communityResult.members,
      urlDetail: communityResult.url
    });
  }

  constructor(data?: ICommunityItemModel) {
    if (!data) {
      return;
    }

    this.id = data.id ? data.id : '';
    this.title = data.title ? data.title : '';
    this.imageUrl = data.imageUrl ? data.imageUrl : '';
    this.isBookmark = data.isBookmark ? data.isBookmark : false;
    this.type = data.type ? data.type : LearningType.Community;
    this.description = data.description ? data.description : '';
    this.members = data.members ? data.members : 0;
    this.urlDetail = data.urlDetail ? data.urlDetail : '';
  }
}
