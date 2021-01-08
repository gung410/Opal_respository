import { MyLearningPackageType } from '../models/my-learning-package.model';

export interface ICreateOrUpdateLearningPackageRequest {
  myLectureId?: string | undefined;
  myDigitalContentId?: string | undefined;
  type: MyLearningPackageType;
  state: string;
}
