import { CxSurveyjsVariable } from '@conexus/cx-angular-common';

export class CxSurveyDataModel {
  json: object;
  data: object;
  variables: CxSurveyjsVariable[];
  cancelName: string;
  submitName: string;
}
