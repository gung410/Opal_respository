import {
  ContentRepository,
  CourseRepository,
  IPagedResultDto,
  IPagedResultRequestDto,
  ITrackingRequest,
  MyDigitalContentRepository,
  TrackingModel,
  TrackingSharedDetailByModel,
  UserTrackingAPIService
} from '@opal20/domain-api';
import { ILearningItemModel, LearningType } from '../models/learning-item.model';
import { Observable, combineLatest, from, of } from 'rxjs';
import { map, switchMap, take } from 'rxjs/operators';

import { CourseDataService } from '../services/course-data.service';
import { DigitalContentItemModel } from '../models/digital-content-item.model';
import { MyDigitalContentDetail } from '../models/my-digital-content-detail.model';
import { Utils } from '@opal20/infrastructure';
import { getDigitalContenType } from '../learner-utils';

export class UserTrackingService {
  constructor(
    private userTrackingAPIService: UserTrackingAPIService,
    private courseRepository: CourseRepository,
    private contentRepository: ContentRepository,
    private myDigitalContentRepository: MyDigitalContentRepository,
    private courseDataService: CourseDataService
  ) {}

  public getTrackingInfoByItemId(itemId: string, itemType: LearningType): Promise<TrackingModel> {
    const request: ITrackingRequest = {
      itemId: itemId,
      itemType: itemType
    };
    return this.userTrackingAPIService.getTrackingInfoByItemId(request);
  }

  public like(itemId: string, itemType: LearningType, isLike: boolean): Promise<TrackingModel> {
    const request: ITrackingRequest = {
      itemId: itemId,
      itemType: itemType,
      isLike: isLike
    };
    return this.userTrackingAPIService.likeEvent(request);
  }

  public share(itemId: string, itemType: LearningType, sharedUsers: string[]): Promise<TrackingModel> {
    const request: ITrackingRequest = {
      itemId: itemId,
      itemType: itemType,
      sharedUsers: sharedUsers
    };
    return this.userTrackingAPIService.shareEvent(request);
  }

  public getSharedTo(skipCount?: number, maxResultCount?: number): Promise<IPagedResultDto<TrackingSharedDetailByModel>> {
    const request: IPagedResultRequestDto = {
      skipCount: skipCount,
      maxResultCount: maxResultCount
    };
    return this.userTrackingAPIService.getSharedTo(request);
  }

  public getSharedToIncludedTag(skipCount?: number, maxResultCount?: number): Promise<IPagedResultDto<TrackingSharedDetailByModel>> {
    const request: IPagedResultRequestDto = {
      skipCount: skipCount,
      maxResultCount: maxResultCount
    };

    return this.userTrackingAPIService.getSharedTo(request).then(data => {
      const courseIds = data.items
        .filter(p => p.itemType === LearningType.Course || p.itemType === LearningType.Microlearning)
        .map(x => x.itemId);

      const dcIds = data.items.filter(p => p.itemType === LearningType.DigitalContent).map(x => x.itemId);

      return combineLatest(this.courseRepository.loadCourses(courseIds, false), this.contentRepository.loadDigitalContentByIds(dcIds))
        .pipe(
          map(([dataCourses, dataDc]) => {
            dataCourses.map(p => {
              data.items.find(m => m.itemId === p.id).tagIds = TrackingSharedDetailByModel.buildTags(p);
            });
            dataDc.map(p => {
              const item = data.items.find(m => m.itemId === p.id);
              item.tagIds = [getDigitalContenType(p)];
              item.thumbnailUrl = `assets/images/icons/sm/${item.tagIds}.svg`;
            });

            return data;
          }),
          take(1)
        )
        .toPromise();
    });
  }

  public getSharedToMeFromCatalogResult(
    skipCount?: number,
    maxResultCount?: number
  ): Observable<{ total: number; items: ILearningItemModel[] }> {
    return from(this.getSharedTo(skipCount, maxResultCount)).pipe(
      switchMap(data => {
        const courseIds = data.items
          .filter(p => p.itemType === LearningType.Course || p.itemType === LearningType.Microlearning)
          .map(x => x.itemId);

        const dcIds = Utils.distinct(data.items.filter(p => p.itemType === LearningType.DigitalContent).map(x => x.itemId));

        return combineLatest(
          courseIds.length ? this.courseDataService.getCourseLearningItem(courseIds) : of([]),
          dcIds.length ? this.contentRepository.loadDigitalContentByIds(dcIds) : of([]),
          dcIds.length ? this.myDigitalContentRepository.loadByDigitalContentIds(dcIds) : of([])
        ).pipe(
          map(([dataCourses, dataDc, dataMyDc]) => {
            const dataDcParse = dataDc.map(dc => {
              const myDigitalContent = dataMyDc.find(myDc => myDc.digitalContentId === dc.id);
              if (myDigitalContent) {
                return DigitalContentItemModel.createDigitalContentItemModel(
                  MyDigitalContentDetail.createMyDigitalContentDetail(myDigitalContent, dc)
                );
              } else {
                return DigitalContentItemModel.createDigitalContentItemModel(MyDigitalContentDetail.createMyDigitalContentDetail(null, dc));
              }
            });

            return {
              total: data.totalCount,
              items: this.sortMergeItems(data.items.map(r => r.itemId), [...dataCourses, ...dataDcParse])
            };
          })
        );
      })
    );
  }

  public sortMergeItems(originalSourceIds: string[], mergedItems: ILearningItemModel[]): ILearningItemModel[] {
    return originalSourceIds.map(v => mergedItems.find(i => i.id === v)).filter(v => v !== undefined);
  }
}
