import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import { AppConstant } from 'app/shared/app.constant';
@Injectable()
export class LearningNeedService {
  constructor(private httpHelper: HttpHelpers) {}

  public getLearningNeedConfig() {
    return this.httpHelper.get(
      `${AppConstant.api.competence}/idp/needs/config`
    );
  }
}
