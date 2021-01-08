import {
  Component, Input, Output, EventEmitter, ElementRef, Renderer2, ViewChild,
  ViewEncapsulation, ChangeDetectionStrategy, OnChanges,
  ChangeDetectorRef, SimpleChanges
} from '@angular/core';
import * as Survey from 'survey-angular';
import * as widgets from 'surveyjs-widgets';
import {
  clone,
  forEach,
  isEmpty,
  uniqueId
} from 'lodash';
import { CxSurveyjsService } from './cx-surveyjs.service';
import { CxSurveyjsVariable, CxSurveyjsEventModel } from './cx-surveyjs.model';
import { SurveyModel } from 'survey-angular';
import {
  mapObjectsToProp, handleErrorWhenLoadChoicesByUrl, getTimeString, compareDates, ifClause, getThisYear, subString
} from './surveyjs-function';
import { cxSurveyInput } from './custom-widget/cx-survey-input';
import { cxSelect2TagBox } from './custom-widget/cx-select2-tag-box';
import { cxInputmask } from './custom-widget/cx-inputmask';
import { MatTabGroup } from '@angular/material/tabs';
import { CxSurveyJsModeEnum } from './cx-surveyjs.enum';
import { CxSurveyjsHelper } from './cx-surveyjs.helper';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CxSurveyStylesManager } from './styles/cx-survey-styles-manager';
import { CxSurveyInputHelper } from './custom-widget/cx-survey-input.helper';
import { cxTextarea } from './custom-widget/cx-textarea';
import { cxPeoplePicker } from './custom-widget/cx-people-picker';

widgets.icheck(Survey);
widgets.select2(Survey);
widgets.inputmask(Survey);
widgets.jquerybarrating(Survey);
widgets.jqueryuidatepicker(Survey);
widgets.nouislider(Survey);
widgets.select2tagbox(Survey);
widgets.sortablejs(Survey);
widgets.ckeditor(Survey);
widgets.autocomplete(Survey);
widgets.bootstrapslider(Survey);
widgets.prettycheckbox(Survey);
Survey.JsonObject.metaData.addProperty('questionbase', 'popupdescription:text');
Survey.JsonObject.metaData.addProperty('questionbase', 'popoverText:text');
Survey.JsonObject.metaData.addProperty('matrix', 'firstHeadingText:text');
Survey.JsonObject.metaData.addProperty('matrixdropdown', 'firstHeadingText:text');
Survey.JsonObject.metaData.addProperty('page', 'popupdescription:text');

CxSurveyStylesManager.applyTheme();

Survey.FunctionFactory.Instance.register('mapObjectsToProp', mapObjectsToProp);
Survey.FunctionFactory.Instance.register('getTimeString', getTimeString);
Survey.FunctionFactory.Instance.register('compareDates', compareDates);
Survey.FunctionFactory.Instance.register('ifClause', ifClause);
Survey.FunctionFactory.Instance.register('getThisYear', getThisYear);
Survey.FunctionFactory.Instance.register('subString', subString);

Survey.CustomWidgetCollection.Instance.addCustomWidget(cxSurveyInput, 'customtype');
Survey.CustomWidgetCollection.Instance.addCustomWidget(cxSelect2TagBox, 'customtype');
Survey.CustomWidgetCollection.Instance.addCustomWidget(cxInputmask, 'customtype');
Survey.CustomWidgetCollection.Instance.addCustomWidget(cxTextarea, 'customtype');
Survey.CustomWidgetCollection.Instance.addCustomWidget(cxPeoplePicker, 'customtype');

const SurveySelectors = {
  CompletedButton: '.sv_complete_btn',
  PanelFooter: '.panel-footer',
  CxSurveyjs: '.cx-surveyjs',
  CxSurveyFooter: 'surveyFooter'
};

@Component({
  selector: 'cx-surveyjs',
  templateUrl: './cx-surveyjs.component.html',
  styleUrls: ['./cx-surveyjs.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CxSurveyjsComponent implements OnChanges {
  @Input() json: any;
  @Input() data: any;
  @Input() validationFunctions: any[];
  @Input() submitName = 'Submit';
  @Input() cancelName: string;
  @Input() variables: CxSurveyjsVariable[];
  @Input() hideSubmitBtn: boolean;
  @Input() disableSubmitBtn: boolean;
  @Input() hideFooter = false;

  @Output() cancel: EventEmitter<undefined> = new EventEmitter<undefined>();
  @Output() submitting: EventEmitter<CxSurveyjsEventModel> = new EventEmitter<CxSurveyjsEventModel>();
  @Output() submit: EventEmitter<any> = new EventEmitter<any>();
  @Output() changePage: EventEmitter<CxSurveyjsEventModel> = new EventEmitter<CxSurveyjsEventModel>();
  @Output() changeValue: EventEmitter<CxSurveyjsEventModel> = new EventEmitter<CxSurveyjsEventModel>();
  @Output() valueChanged: EventEmitter<CxSurveyjsEventModel> = new EventEmitter<CxSurveyjsEventModel>();
  @Output() afterSurveyRender: EventEmitter<CxSurveyjsEventModel> = new EventEmitter<CxSurveyjsEventModel>();
  @Output() afterQuestionsRender: EventEmitter<CxSurveyjsEventModel> = new EventEmitter<CxSurveyjsEventModel>();

  @ViewChild('cancelButton') private cancelButton: ElementRef;
  @ViewChild('surveyTabGroup') private surveyTabGroupElRef: MatTabGroup;
  @ViewChild('dummyTextbox') private dummyTextbox: ElementRef;

  private survey: Survey.Model;
  public get surveyModel() {
    return this.survey;
  }

  private fixedFooter: boolean;
  private visibleOnlyValidTabs: boolean;
  private autoMoveToIncompletePage: boolean;
  public showPageAsTab: boolean;
  public invalidPageIndex = 0;
  public lastValidTabIndex = 0; // only use for tabs

  public pages: string[] = [];
  public uniqueId: string = uniqueId();
  public isCompleted: boolean;
  public get surveyjsElementId() {
    return `surveyElement_${this.uniqueId}`;
  }
  constructor(
    private ngbModal: NgbModal,
    private changeDetectorRef: ChangeDetectorRef,
    private element: ElementRef,
    private renderer: Renderer2,
    private surveyjsService: CxSurveyjsService
  ) {
  }

  private onCompleting = (survey: SurveyModel, options: any) => {
    this.submitting.emit(new CxSurveyjsEventModel({ survey, options }));
  }

  private onComplete = (survey: SurveyModel) => {
    this.isCompleted = true;
    this.submit.emit(survey.data);
  }

  private onCurrentPageChanged = (survey: SurveyModel, options: { oldCurrentPage: any, newCurrentPage: any }) => {
    const currentPage = survey.currentPageNo;
    if (options.newCurrentPage && options.newCurrentPage.visibleIndex > this.invalidPageIndex) {
      this.invalidPageIndex = options.newCurrentPage.visibleIndex;
      if (this.visibleOnlyValidTabs) {
        this.lastValidTabIndex = this.invalidPageIndex;
      }
    }
    if (this.showPageAsTab) {
      this.surveyTabGroupElRef.selectedIndex = currentPage;
      this.changeDetectorRef.detectChanges();
    }
    this.changePage.emit(new CxSurveyjsEventModel({ survey, options }));
  }

  public onTabChanged(tabIndex) {
    this.survey.currentPage = tabIndex;
  }

  private moveToIncompletePage() {
    if (!this.survey || this.survey.visiblePageCount <= 1) { return; }
    const incompleteQuestions = this.getIncompleteQuestions();
    if (!incompleteQuestions || !incompleteQuestions.length) { return; }
    this.survey.currentPage = incompleteQuestions[0].page.visibleIndex;
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.json) {
      this.renderSurvey();
      return;
    }
    if (changes.variables) {
      this.onLocalSurveyVariablesChanged(this.survey, changes.variables.currentValue);
    }
    setTimeout(() => {
      this.changeSurveyjsConfig(changes);
      this.refreshCompleteButton(changes);
    });
  }

  private renderSurvey() {
    this.initSurveyModel().then(() =>
      Survey.SurveyNG.render(this.surveyjsElementId, { model: this.survey })
    );
  }

  private initSurveyModel(): Promise<any> {
    this.isCompleted = false;
    if (!this.json) { return; }
    this.survey = new Survey.Model(this.json);
    if (this.surveyjsService.locale) { this.survey.locale = this.surveyjsService.locale; }

    this.setItemValueIfEmpty(this.survey);
    this.changeSurveyjsConfig();
    this.initValidateFunction();
    this.initSurveyVariables(this.survey);
    // cx surveyjs configuration
    this.showPageAsTab = !!this.json.showPageAsTab;
    if (this.showPageAsTab) { this.visibleOnlyValidTabs = !!this.json.visibleOnlyValidTabs; }
    this.fixedFooter = !!this.json.fixedFooter;
    this.autoMoveToIncompletePage = !!this.json.autoMoveToIncompletePage;

    this.pages = this.getTitleList();
    const incompleteQuestions = this.getIncompleteQuestions();
    this.invalidPageIndex = incompleteQuestions && incompleteQuestions.length
      ? incompleteQuestions[0].page.visibleIndex : this.pages.length;

    if (this.showPageAsTab) {
      this.lastValidTabIndex = this.visibleOnlyValidTabs ? this.invalidPageIndex : this.pages.length;
    }

    this.survey.showQuestionNumbers = 'off';
    this.survey.showCompletedPage = false;
    this.survey.onComplete.add(this.onComplete);
    this.survey.onCompleting.add(this.onCompleting);
    this.survey.onCurrentPageChanged.add(this.onCurrentPageChanged);
    this.survey.onValueChanging.add(this.onValueChanging);
    this.survey.onValueChanged.add(this.onValueChanged);
    this.survey.onLoadChoicesFromServer.add(handleErrorWhenLoadChoicesByUrl);
    this.survey.onAfterRenderSurvey.add(this.onAfterRenderSurvey);
    this.survey.onAfterRenderQuestion.add(this.onAfterRenderQuestion);

    return Promise.resolve();
  }

  private changeSurveyjsConfig(changes?: SimpleChanges) { // onchanges component input
    if (changes) {
      if (changes.data) {
        this.survey.data = changes.data.currentValue;
      }
      if (changes.submitName) { this.survey.completeText = changes.submitName.currentValue; }
    } else {
      if (!isEmpty(this.data)) { this.survey.data = this.data; }
      if (this.submitName) { this.survey.completeText = this.submitName; }
    }
  }

  private initValidateFunction() {
    if (this.validationFunctions && this.validationFunctions.length) {
      this.validationFunctions
        .forEach(validationFunction => {
          this.survey.onServerValidateQuestions.add(validationFunction);
        });
    }
  }

  private onAfterRenderQuestion = (survey, options) => {
    const question = options.question;
    const htmlElement = options.htmlElement;
    const questionType = question.getType();

    if (questionType === 'cxsurveyinput') {
      question.onRemoveItem = CxSurveyInputHelper.onRemoveItem.bind(this, this.ngbModal, question);
    }

    if (question.removePanelUI) {
      question.removePanelUI = CxSurveyjsHelper.removePanelUI.bind(this, this.ngbModal, question);
    }

    if (htmlElement && question) {
      CxSurveyjsHelper.moveSelectAllToTop(htmlElement);

      if (questionType === 'text') {
        CxSurveyjsHelper.removeAutocomplete(htmlElement, question);
      }

      if (questionType === 'datepicker') {
        CxSurveyjsHelper.disableDatePickerInput(htmlElement, question);
      }

      if (question.popoverText) {
        CxSurveyjsHelper.addPopOverButton(htmlElement, question);
      }

      if (questionType === 'matrix' || questionType === 'matrixdropdown') {
        CxSurveyjsHelper.addFirstHeadingTextToMatrixTable(htmlElement, question);
      }

      this.removeFocusableOnSVGTags(htmlElement);
    }

    this.afterQuestionsRender.emit(new CxSurveyjsEventModel({ survey, options }));
  }

  private removeFocusableOnSVGTags(questionHtmlElement: any): void {
    const svgElements = questionHtmlElement.getElementsByTagName('svg');
    if (svgElements.length > 0) {
      forEach(svgElements, svgElement => {
        svgElement.setAttribute('focusable', 'false');
      });
    }
  }

  private onAfterRenderSurvey = (survey, options) => {
    this.refreshCompleteButton();
    this.setupFooter();

    if (this.showPageAsTab && this.surveyTabGroupElRef) {
      this.surveyTabGroupElRef.selectedIndex = 0;
      this.changeDetectorRef.detectChanges();
    }

    if (this.autoMoveToIncompletePage) {
      this.moveToIncompletePage();
    }
    this.afterSurveyRender.emit(new CxSurveyjsEventModel({ survey, options }));
  }

  private refreshCompleteButton(changes?: SimpleChanges) {
    const completeBtn = this.element.nativeElement.querySelector(SurveySelectors.CompletedButton);
    if (completeBtn) {
      if (changes) {
        if (changes.disableSubmitBtn) {
          completeBtn.disabled = changes.disableSubmitBtn.currentValue;
        }
        if (changes.hideSubmitBtn) {
          completeBtn.hidden = changes.hideSubmitBtn.currentValue;
        }
      } else {
        completeBtn.disabled = this.disableSubmitBtn;
        completeBtn.hidden = this.hideSubmitBtn;
      }
    }
  }

  public changeMode(mode: CxSurveyJsModeEnum) {
    this.survey.mode = mode;
    this.changeDetectorRef.detectChanges();
  }

  private setItemValueIfEmpty(survey: Survey.Survey) {
    (survey.getAllQuestions().filter((question: any) =>
      question.choicesByUrl && (!question.choicesByUrl.valueName || question.storeWholeObject)) as Survey.Question[])
      .forEach(question => {
        question.choicesByUrl.getItemValueCallback = (itemValue) => itemValue;
      });
  }

  public doComplete(): boolean {
    const numberOfPages = this.survey.visiblePageCount;
    for (let pageIndex = 0; pageIndex < numberOfPages; pageIndex++) {
      if (this.survey.getPage(pageIndex).hasErrors(true)) {
        this.survey.currentPage = pageIndex;
        return false;
      }
    }
    return this.survey.completeLastPage();
  }

  nextPage(): boolean {
    return this.survey.nextPage();
  }

  prevPage(): boolean {
    return this.survey.prevPage();
  }

  public onCancel() {
    this.cancel.emit();
  }

  private onValueChanging = (survey: SurveyModel, options: any) => {
    this.changeValue.emit(new CxSurveyjsEventModel({ survey, options }));
  }

  /**
   * Default: When value changed, survey will re-validate that question if there is an error.
   * Set options.ignoreRevalidation to ignore to feature.
   * @see validateQuestion
   */
  private onValueChanged = (survey: SurveyModel, options: any) => {
    this.valueChanged.emit(new CxSurveyjsEventModel({ survey, options }));
    if (options.ignoreRevalidation) { return; }
    this.validateQuestion(options.question);
  }


  private validateQuestion(question) {
    if (question.currentErrorCount > 0) {
      question.hasErrors(true);
    }
  }

  public getTitleList(visibleOnly = true): string[] {
    if (!this.survey) { return []; }

    const pages = visibleOnly ? this.survey.visiblePages : this.survey.pages;
    if (!pages || !pages.length) { return []; }

    return pages.map(page => {
      return page.title ? page.title : page.name;
    });
  }

  public getIncompleteQuestions(visibleOnly = true): any[] {
    if (!this.survey) { return []; }
    const currentQuestions = this.survey.getAllQuestions(visibleOnly);
    if (!currentQuestions || !currentQuestions.length) { return []; }
    return currentQuestions.filter((ques: Survey.Question) => ques.hasErrors(false) || (ques.isRequired && ques.isEmpty()));
  }

  public resetSurvey(): void {
    if (!this.survey) {
      return;
    }

    this.survey.clear();
    this.survey.render();
  }

  private setupFooter() {
    const nativeElement = this.element.nativeElement;
    const surveyFooter = nativeElement.querySelector(SurveySelectors.PanelFooter);
    if (!surveyFooter) {
      return;
    }

    if (this.hideFooter) {
      surveyFooter.hidden = true;
      return;
    }

    if (this.cancelButton) {
      this.renderer.insertBefore(surveyFooter, this.cancelButton.nativeElement, surveyFooter.firstChild);
    }

    if (this.fixedFooter) {
      const footer = nativeElement.querySelector(`#${SurveySelectors.CxSurveyFooter}_${this.uniqueId}`);
      if (footer) { footer.remove(); }
      surveyFooter.id = `${SurveySelectors.CxSurveyFooter}_${this.uniqueId}`;
      this.renderer.appendChild(nativeElement.querySelector(SurveySelectors.CxSurveyjs), surveyFooter);
    }

    // This is for fixing the survey form which will auto fire the 'submit' event
    //  when hitting enter if the survey form has only one Text-box field.
    this.renderer.insertBefore(surveyFooter, this.dummyTextbox.nativeElement, surveyFooter.lastChild);
  }

  private onLocalSurveyVariablesChanged(
    survey: Survey.Survey,
    currentVariables: any
  ): void {
    if (currentVariables && currentVariables.length) {
      currentVariables.forEach(currentVariable => {
        // TODO: Check previous variable to see if it is different from the current value.
        // - Since we didn't find why the previousVariables and the currentVariables always equals so we always set new variable.
        survey.setVariable(currentVariable.name, currentVariable.value);
      });
    }
  }

  /**
   * Initialize the survey js variables.
   * @param survey The survey model
   */
  private initSurveyVariables(survey: Survey.Survey): void {
    /**
     * Contains a list of variables which will be set to the survey js. The local variables (this.variables) should be prioritized.
     */
    const processingVariables =
      this.variables && this.variables.length
        ? clone(this.variables)
        : [];

    // Add global variable which doesn't exist in the local variable.
    if (
      this.surveyjsService.variables &&
      this.surveyjsService.variables.length
    ) {
      this.surveyjsService.variables.forEach(globalVariable => {
        const localVariableIndex = this.variables
          ? this.variables.findIndex(v => v.name === globalVariable.name)
          : -1;
        if (localVariableIndex === -1) {
          processingVariables.push(globalVariable);
        }
      });
    }

    // Set survey js variables
    processingVariables.forEach(addingVariable => {
      survey.setVariable(addingVariable.name, addingVariable.value);
    });
  }
}
