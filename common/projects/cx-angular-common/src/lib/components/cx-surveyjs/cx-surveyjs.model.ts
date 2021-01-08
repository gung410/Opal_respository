import { SurveyModel } from 'survey-angular';
export class CxSurveyjsVariable {
    name: string;
    value: any;
    constructor(data?: Partial<CxSurveyjsVariable>) {
        if (!data) { return; }
        this.name = data.name ? data.name : '';
        this.value = data.value;
    }
}

export class CxSurveyjsEventModel {
    survey: SurveyModel;
    options: any;
    constructor(data?: Partial<CxSurveyjsEventModel>) {
        if (!data) { return; }
        this.survey = data.survey;
        this.options = data.options;
    }
}
