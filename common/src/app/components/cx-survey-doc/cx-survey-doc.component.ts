import { Component, OnInit, ViewChild, ChangeDetectionStrategy } from '@angular/core';
import { CxFormModal, CxSurveyjsFormModalOptions, CxSurveyjsComponent, CxSurveyjsEventModel, CxConfirmationDialogComponent } from 'projects/cx-angular-common/src';
import { CxSurveyDocComponentModel } from './cx-survey-doc.component.model';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'cx-survey-doc',
  templateUrl: './cx-survey-doc.component.html',
  styleUrls: ['./cx-survey-doc.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CxSurveyDocComponent implements OnInit {

  surveyDocViewModel: CxSurveyDocComponentModel;

  private localstorageNameItem = 'cx-surveyjs-doc';
  @ViewChild('cxsurvey') cxSurveyjsComponent: CxSurveyjsComponent;
  surveyJson: string;
  surveyData: string;
  ngOnInit(): void {
  }

  constructor(
    private formModal: CxFormModal,
    private ngbModal: NgbModal
  ) {
    const docTemp = localStorage.getItem(this.localstorageNameItem);
    this.surveyDocViewModel = new CxSurveyDocComponentModel(JSON.parse(docTemp));
    this.surveyData = JSON.stringify(this.surveyDocViewModel.data);
    this.surveyJson = JSON.stringify(this.surveyDocViewModel.json);
  }

  open() {
    this.updateSurveyModel();
    if (this.formModal.hasOpenModals()) { return; }
    if (!this.surveyDocViewModel.json) { return; }
    const modalRef = this.formModal.openSurveyJsForm(
      this.surveyDocViewModel.json,
      this.surveyDocViewModel.data,
      [

      ],
      new CxSurveyjsFormModalOptions({
        fixedButtonsFooter: false,
        fixedHeight: true,
        cancelName: '12321312',
        showModalHeader: true,
        modalHeaderText: 'External PDO'
      }),
      { size: 'lg', centered: true });
    this.formModal.submit.subscribe(event => {
      modalRef.close();
    });
  }

  render() {
    this.updateSurveyModel();
  }

  onSubmit(event: CxSurveyjsEventModel) {
    console.log(event);
  }

  onCancel() {
    // console.log();
  }

  onChangePage(event: CxSurveyjsEventModel) {
    // console.log(event);
  }

  onChangeValue(event: CxSurveyjsEventModel) {
    // console.log(event);
  }

  onSubmitting(event: CxSurveyjsEventModel) {
    // console.log(event);
  }

  onChangedValue(event: CxSurveyjsEventModel) {
    // console.log(event);
  }

  onAfterSurveyRender(event: CxSurveyjsEventModel) {
    // console.log(event);
  }

  showConfirmDialog() {
    const modalRef = this.ngbModal.open(CxConfirmationDialogComponent, {
      size: 'sm'
    });
    const component = modalRef.componentInstance as CxConfirmationDialogComponent;
    component.header = 'Confirmation';
    component.content = 'Test confirm dialog';
    component.isDanger = true;
    component.cancelButtonText = 'CANCEL';
    component.confirmButtonText = 'OK';
    component.cancel.subscribe(() => {
      modalRef.close();
    });
    component.confirm.subscribe(() => {
      modalRef.close();
    });
  }

  private parseJSON(jsonString: string): any {
    try {
      if (!jsonString) {
        return {};
      }
      return JSON.parse(jsonString);
    } catch (error) {
      console.error('cannot parse json');
      return {};
    }
  }

  private updateSurveyModel() {
    this.surveyDocViewModel = null;
    this.surveyDocViewModel = {
      json: this.parseJSON(this.surveyJson),
      data: this.parseJSON(this.surveyData),
    };

    localStorage.setItem(this.localstorageNameItem, JSON.stringify(this.surveyDocViewModel));
  }
}
