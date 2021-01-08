import { Input, Injectable } from '@angular/core';
import { NgbModal, NgbModalOptions, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { CxSurveyjsModalTemplateComponent } from '../components/cx-surveyjs/cx-surveyjs-modal-template/cx-surveyjs-modal-template.component';
import { CxSurveyjsVariable, CxSurveyjsEventModel } from '../components/cx-surveyjs/cx-surveyjs.model';

export class CxSurveyjsFormModalOptions {
    public showModalHeader: boolean;
    public modalHeaderText: string;
    public cancelName: string;
    public submitName: string;
    public variables: CxSurveyjsVariable[];
    public fixedButtonsFooter?: boolean;
    public fixedHeight?: boolean;

    constructor(data?: Partial<CxSurveyjsFormModalOptions>) {
      if (!data) {
        this.showModalHeader = true;
        this.cancelName = 'Cancel';
        this.submitName = 'Submit';
        this.variables = [];
        return;
      }
      this.showModalHeader = data.showModalHeader != null ? data.showModalHeader : false;
      this.modalHeaderText = data.modalHeaderText;
      this.cancelName = data.cancelName ? data.cancelName : 'Cancel';
      this.submitName = data.submitName ? data.submitName : 'Submit';
      this.variables = data.variables ? data.variables : [];
      this.fixedButtonsFooter = data.fixedButtonsFooter;
      this.fixedHeight = data.fixedHeight;
    }
}
@Injectable()
export class CxFormModal extends NgbModal {
    public json: any;
    public data: any;
    public validationFunctions: any[];
    public submitName: string;
    public cancelName: string;
    public variables: CxSurveyjsVariable[] = [];
    public submit: Subject<any> = new Subject();
    public changePage: Subject<number> = new Subject();
    public changeValue: Subject<CxSurveyjsEventModel> = new Subject();
    public submitting: Subject<CxSurveyjsEventModel> = new Subject();
    public valueChanged: Subject<CxSurveyjsEventModel> = new Subject();
    public afterSurveyRender: Subject<CxSurveyjsEventModel> = new Subject();

    public openSurveyJsForm(
        json: any, data: any, validationFunctions: any[],
        surveyjsFormModalOption: CxSurveyjsFormModalOptions = new CxSurveyjsFormModalOptions(),
        options?: NgbModalOptions): NgbModalRef {
        if (!surveyjsFormModalOption) { surveyjsFormModalOption = new CxSurveyjsFormModalOptions(); }
        const modalRef = super.open(CxSurveyjsModalTemplateComponent, options);
        const componentInstanceTemplate = (modalRef.componentInstance as CxSurveyjsModalTemplateComponent);
        componentInstanceTemplate.json = json;
        componentInstanceTemplate.data = data;
        componentInstanceTemplate.validationFunctions = validationFunctions;
        componentInstanceTemplate.variables = surveyjsFormModalOption.variables;
        componentInstanceTemplate.showModalHeader = surveyjsFormModalOption.showModalHeader;
        componentInstanceTemplate.modalHeaderText = surveyjsFormModalOption.modalHeaderText;
        componentInstanceTemplate.cancelName = surveyjsFormModalOption.cancelName;
        componentInstanceTemplate.submitName = surveyjsFormModalOption.submitName;
        componentInstanceTemplate.fixedButtonsFooter = surveyjsFormModalOption.fixedButtonsFooter;
        componentInstanceTemplate.fixedHeight = surveyjsFormModalOption.fixedHeight;
        componentInstanceTemplate.cancel.subscribe(event => { modalRef.close(); });
        componentInstanceTemplate.submit.subscribe(event => this.onSubmit(event));
        componentInstanceTemplate.changePage.subscribe(event => { this.onChangePage(event); });
        componentInstanceTemplate.changeValue.subscribe(event => { this.onChangeValue(event); });
        componentInstanceTemplate.submitting.subscribe(event => { this.onSubmitting(event); });
        componentInstanceTemplate.valueChanged.subscribe(event => { this.onChangedValue(event); });
        componentInstanceTemplate.afterSurveyRender.subscribe(event => { this.onAfterSurveyRender(event); });
        return modalRef;
    }

    private onSubmit(event) {
        this.submit.next(event);
    }

    private onChangePage(event) {
        this.changePage.next(event);
    }

    private onChangeValue(event: CxSurveyjsEventModel) {
        this.changeValue.next(event);
    }

    private onSubmitting(event: CxSurveyjsEventModel) {
        this.submitting.next(event);
    }

    private onChangedValue(event: CxSurveyjsEventModel) {
        this.valueChanged.next(event);
    }

    private onAfterSurveyRender(event: CxSurveyjsEventModel) {
        this.afterSurveyRender.next(event);
    }
}
