import { CxSurveyjsVariable } from 'projects/cx-angular-common/src/public_api';

export class CxSurveyDocComponentModel {
  json: object;
  data: object;
  validationFunctions?: any[];
  submitName?: string;
  cancelName?: string;
  variables?: CxSurveyjsVariable[];
  hideSubmitBtn?: boolean;
  disableSubmitBtn?: boolean;
  constructor(data?: Partial<CxSurveyDocComponentModel>) {
    if (!data) { return; }
    this.json = data.json;
    this.data = data.data;
    this.validationFunctions = data.validationFunctions ? data.validationFunctions : [];
    this.submitName = data.submitName;
    this.cancelName = data.cancelName;
    this.variables = data.variables ? data.variables : [];
    this.hideSubmitBtn = data.hideSubmitBtn;
    this.disableSubmitBtn = data.disableSubmitBtn;
  }
}
