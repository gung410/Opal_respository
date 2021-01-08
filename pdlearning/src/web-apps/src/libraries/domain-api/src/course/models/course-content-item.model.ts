import { Assignment, IAssignment } from './assignment.model';
import { ILectureModel, LectureModel } from './lecture.model';
import { ISectionModel, SectionModel } from './section.model';
import { MAX_INT, Utils } from '@opal20/infrastructure';

import { LMM_PERMISSIONS } from '../../share/permission-keys/lmm-permission-key';
import { UserInfoModel } from './../../share/models/user-info.model';

export interface ICourseContentItemModel {
  id: string;
  parentId?: string;
  title: string;
  icon?: string | undefined;
  type: CourseContentItemType;
  items?: ICourseContentItemModel[] | undefined;
  order?: number;
  additionalInfo?: ILectureModel | ISectionModel | IAssignment;
  createdBy?: string;
}

// @dynamic
export class CourseContentItemModel implements ICourseContentItemModel {
  public id: string;
  public parentId?: string;
  public title: string;
  public icon?: string | undefined;
  public type: CourseContentItemType;
  public items: CourseContentItemModel[] = [];
  public order?: number;
  public additionalInfo?: LectureModel | SectionModel | Assignment;
  public expanded?: boolean;
  public editing?: boolean;
  public createdBy?: string;

  public static removeItem(items: CourseContentItemModel[], itemId: string): CourseContentItemModel[] {
    return Utils.clone(items, _ => {
      const removeItem = _.find(p => p.id === itemId);
      if (removeItem != null) {
        _.filter(p => removeItem && removeItem.order != null && p.order >= removeItem.order).forEach(p => {
          p.order -= 1;
        });
        _ = _.filter(p => p.id !== itemId);
        return _;
      }

      return _.map(p => p.removeChild(itemId));
    });
  }

  public static addItem(items: CourseContentItemModel[], item: CourseContentItemModel): CourseContentItemModel[] {
    const parentItemIndex = items.findIndex(p => p.id === item.parentId);
    if (parentItemIndex >= 0) {
      return Utils.clone(items, _ => {
        _[parentItemIndex] = _[parentItemIndex].addChild(item);
      });
    }

    return Utils.orderBy(
      Utils.clone(items, _ => {
        _.filter(p => item.order != null && p.order >= item.order).forEach(p => {
          p.order += 1;
        });
        return _.concat(item);
      }),
      p => (p.order != null ? p.order : MAX_INT)
    );
  }

  public static fromAssignment(data: Assignment): CourseContentItemModel {
    return new CourseContentItemModel({
      ...data,
      type: CourseContentItemType.Assignment,
      id: data.id ? data.id : ''
    });
  }

  public static fromLecture(data: LectureModel): CourseContentItemModel {
    return new CourseContentItemModel({
      ...data,
      type: CourseContentItemType.Lecture,
      id: data.id ? data.id : '',
      parentId: data.sectionId
    });
  }

  public static fromSection(data: SectionModel): CourseContentItemModel {
    return new CourseContentItemModel({
      ...data,
      type: CourseContentItemType.Section,
      id: data.id ? data.id : ''
    });
  }

  public static buildAdditionalInfo(data: ICourseContentItemModel): LectureModel | SectionModel | Assignment | undefined {
    if (data.type === CourseContentItemType.Lecture) {
      return new LectureModel(<ILectureModel>data.additionalInfo);
    }
    if (data.type === CourseContentItemType.Section) {
      return new SectionModel(<ISectionModel>data.additionalInfo);
    }
    if (data.type === CourseContentItemType.Assignment) {
      return new Assignment(<IAssignment>data.additionalInfo);
    }
    return null;
  }

  public static updateAssignmentContentItem(contentItems: CourseContentItemModel[], assignment: Assignment): CourseContentItemModel[] {
    return Utils.clone(contentItems, p => {
      const contentItem = p.find(item => item.id === assignment.id);
      if (contentItem) {
        Object.assign(contentItem, CourseContentItemModel.fromAssignment(assignment));
      }
    });
  }

  public static updateLectureContentItem(items: CourseContentItemModel[], lecture: LectureModel): CourseContentItemModel[] {
    return Utils.clone(items, p => {
      const contentItem = p.find(_ => _.id === lecture.id);
      if (contentItem) {
        Object.assign(contentItem, CourseContentItemModel.fromLecture(lecture));
      }
    });
  }

  public static updateSectionContentItem(contentItems: CourseContentItemModel[], data: SectionModel): CourseContentItemModel[] {
    return Utils.clone(contentItems, p => {
      const contentItem = p.find(_ => _.id === data.id);
      if (contentItem) {
        Object.assign(contentItem, CourseContentItemModel.fromSection(data));
      }
    });
  }

  public static hasCreateCourseContentPermission(user: UserInfoModel): boolean {
    return user.hasPermissionPrefix(LMM_PERMISSIONS.CreateCourseContent);
  }

  public static hasEditDeleteMineCourseContentPermission(user: UserInfoModel): boolean {
    return user.hasPermissionPrefix(LMM_PERMISSIONS.EditDeleteMineCourseContent);
  }

  public static hasEditDeleteOthersCourseContentPermission(user: UserInfoModel): boolean {
    return user.hasPermissionPrefix(LMM_PERMISSIONS.EditDeleteOthersCourseContent);
  }

  constructor(data?: ICourseContentItemModel) {
    if (data == null) {
      return;
    }
    this.id = data.id;
    this.parentId = data.parentId;
    this.title = data.title;
    this.icon = data.icon;
    this.type = data.type;
    this.items = data.items != null ? data.items.map(p => new CourseContentItemModel(p)) : [];
    this.order = data.order;
    this.additionalInfo = CourseContentItemModel.buildAdditionalInfo(data);
    this.createdBy = data.createdBy;
  }

  public get isMine(): boolean {
    return this.createdBy != null && UserInfoModel.getMyUserInfo().extId === this.createdBy.toLocaleLowerCase();
  }

  public getFirstNotSectionId(): string | null {
    if (this.type === CourseContentItemType.Lecture || this.type === CourseContentItemType.Assignment) {
      return this.id;
    } else if (this.type === CourseContentItemType.Section && this.items.length > 0) {
      return this.items[0].id;
    }
    return null;
  }

  public removeChild(id: string): CourseContentItemModel {
    return Utils.clone(this, _ => {
      if (_.items != null) {
        const removeItem = _.items.find(p => p.id === id);
        if (removeItem != null) {
          _.items
            .filter(p => removeItem && removeItem.order != null && p.order >= removeItem.order)
            .forEach(p => {
              p.order -= 1;
            });
          _.items = _.items.filter(p => p.id !== id);
        }
        return _;
      }
    });
  }

  public addChild(item: CourseContentItemModel): CourseContentItemModel {
    return Utils.clone(this, _ => {
      if (_.items != null) {
        _.items
          .filter(p => item.order != null && p.order >= item.order)
          .forEach(p => {
            p.order += 1;
          });
        _.items = Utils.orderBy(_.items.concat([item]), p => (p.order != null ? p.order : MAX_INT));
      }
    });
  }

  public isSection(): boolean {
    return this.type === CourseContentItemType.Section;
  }
}

export enum CourseContentItemType {
  Course = 'Course',
  Section = 'Section',
  Lecture = 'Lecture',
  Assignment = 'Assignment'
}
