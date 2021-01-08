import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { IMyLearningPackageModel, MyLearningPackageModel, MyLearningPackageType } from '../models/my-learning-package.model';

import { ICreateOrUpdateLearningPackageRequest } from '../dtos/my-learning-backend-service.dto';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

@Injectable()
export class MyLearningBackendService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.learnerApiUrl + '/me/learning';
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getMyLearningPackage(myLectureId: string, myDigitalContentId: string): Promise<MyLearningPackageModel> {
    if (myLectureId) {
      return this.get<IMyLearningPackageModel>(`/getLearningPackage`, { myLectureId: myLectureId })
        .pipe(map(result => new MyLearningPackageModel(result)))
        .toPromise();
    }
    return this.get<IMyLearningPackageModel>(`/getLearningPackage`, { myDigitalContentId: myDigitalContentId })
      .pipe(map(result => new MyLearningPackageModel(result)))
      .toPromise();
  }

  public createMyLearningPackageInMyLecture(
    myLectureId: string,
    state: string,
    type: MyLearningPackageType = MyLearningPackageType.SCORM
  ): Promise<MyLearningPackageModel> {
    const requestBody: ICreateOrUpdateLearningPackageRequest = {
      myLectureId: myLectureId,
      type: type,
      state: state
    };
    return this.post<ICreateOrUpdateLearningPackageRequest, IMyLearningPackageModel>('/createLearningPackage', requestBody)
      .pipe(map(result => new MyLearningPackageModel(result)))
      .toPromise();
  }

  public updateMyLearningPackageInMyLecture(
    myLectureId: string,
    state: string,
    type: MyLearningPackageType = MyLearningPackageType.SCORM
  ): Promise<MyLearningPackageModel> {
    const requestBody: ICreateOrUpdateLearningPackageRequest = {
      myLectureId: myLectureId,
      type: type,
      state: state
    };
    return this.post<ICreateOrUpdateLearningPackageRequest, IMyLearningPackageModel>('/updateLearningPackage', requestBody)
      .pipe(map(result => new MyLearningPackageModel(result)))
      .toPromise();
  }
}
