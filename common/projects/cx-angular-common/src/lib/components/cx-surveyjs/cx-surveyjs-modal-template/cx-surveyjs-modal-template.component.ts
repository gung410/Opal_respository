import { Component, ViewEncapsulation, ElementRef, Renderer2, Input, ChangeDetectorRef, ViewChild } from '@angular/core';
import { CxSurveyjsComponent } from '../cx-surveyjs.component';
import { CxSurveyjsService } from '../cx-surveyjs.service';
import { CxSurveyjsEventModel } from '../cx-surveyjs.model';
import { isEmpty } from 'lodash';
import * as Survey from 'survey-angular';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
@Component({
    selector: 'cx-surveyjs-modal-template',
    templateUrl: './cx-surveyjs-modal-template.component.html',
    styleUrls: ['./cx-surveyjs-modal-template.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class CxSurveyjsModalTemplateComponent extends CxSurveyjsComponent {
    @Input() showModalHeader: boolean;
    @Input() modalHeaderText: string;
    @Input() previousName = 'Previous';
    @Input() nextName =  'Next';
    @Input() fixedButtonsFooter =  false;
    @Input() fixedHeight =  false;

    isShowingMandatoryText: boolean;
    hideFooterFlag = true;
    surveyObject: Survey.SurveyModel;
    isFirstPage = true;
    isLastPage = false;
    multiplePage = false;

    @ViewChild('cxSurveyComponent') cxSurveyComponent: CxSurveyjsComponent;
    constructor(
        ngbModal: NgbModal,
        changeDetectorRef: ChangeDetectorRef,
        element: ElementRef,
        renderer: Renderer2,
        surveyjsService: CxSurveyjsService
        ) {
        super(
          ngbModal,
          changeDetectorRef,
          element,
          renderer,
          surveyjsService
          );
    }

    onSubmit(surveyData: any) {
        this.submit.emit(surveyData);
    }

    onChangePage(event: CxSurveyjsEventModel) {
      this.isFirstPage = event.survey.isFirstPage;
      this.isLastPage = event.survey.isLastPage;
      this.changePage.emit(event);
    }

    onChangeValue(event: CxSurveyjsEventModel) {
        this.changeValue.emit(event);
    }

    onClickCancel() {
        this.cancel.emit();
    }

    onSubmitting(event: CxSurveyjsEventModel) {
        this.submitting.emit(event);
    }

    onChangedValue(event: CxSurveyjsEventModel) {
        this.valueChanged.emit(event);
    }

    onAfterSurveyRender(event: CxSurveyjsEventModel) {
      this.surveyObject = event.survey;
      this.multiplePage = event.survey.pageCount > 1;
      // looking for which the survey has field required
      this.isShowingMandatoryText = event.survey.getAllQuestions().findIndex((x: Survey.Question) => x.isRequired) > -1;

      if (this.showModalHeader && isEmpty(this.modalHeaderText)) {
        this.modalHeaderText = event.survey.title;
        event.survey.showTitle = false;
      }
      this.afterSurveyRender.emit(event);
    }

    onClickSubmit(): boolean {
      return this.cxSurveyComponent.doComplete();
    }

    onClickNextPage(): boolean {
      return this.cxSurveyComponent.nextPage();
    }

    onClickPrevPage(): boolean {
      return this.cxSurveyComponent.prevPage();
    }
}
