import { AfterViewInit, Component, ElementRef, EventEmitter, Input, Output, QueryList, SimpleChanges, ViewChildren } from '@angular/core';
import { BaseFormComponent, DomUtils, Guid, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { FormDetailMode, FormEditorPageService } from '@opal20/domain-components';
import {
  FormModel,
  FormQuestionModel,
  FormSection,
  FormSectionsQuestions,
  FormType,
  IFormSection,
  IQuestionBankSelection,
  ModelMappingHelper,
  QuestionType
} from '@opal20/domain-api';

import { FormQuestionEditorComponent } from './form-question-editor.component';
import { FormSectionEditorComponent } from './form-section-editor.component';

@Component({
  selector: 'form-editor',
  templateUrl: './form-editor.component.html'
})
export class FormEditorComponent extends BaseFormComponent implements AfterViewInit {
  @Input('mode') public mode: FormDetailMode = FormDetailMode.View;

  public readonly FormType: typeof FormType = FormType;
  public readonly QuestionType: typeof QuestionType = QuestionType;
  public isShowExplanationNote: boolean = false;

  public set isAddQuestionFreeTextInPoll(v: boolean) {
    this.formData.isShowFreeTextQuestionInPoll = v;

    if (this.formData.isShowFreeTextQuestionInPoll === true && this.orderedFormQuestionsData.length === 1) {
      this.insertNewQuestion(QuestionType.ShortText, 2, 0);
    }
    if (this.formData.isShowFreeTextQuestionInPoll === false && this.orderedFormQuestionsData.length === 2) {
      const id = this.orderedFormQuestionsData.find(x => x.questionType === QuestionType.ShortText).id;
      this.processDeleteQuestion(id);
    }

    this.formDataChange.emit(this.formData);
  }

  public get isAddQuestionFreeTextInPoll(): boolean {
    return this.formData.isShowFreeTextQuestionInPoll;
  }

  @ViewChildren(FormQuestionEditorComponent) public questionEditors: QueryList<FormQuestionEditorComponent>;
  @ViewChildren(FormSectionEditorComponent) public sectionEditors: QueryList<FormSectionEditorComponent>;

  @Input() public formData: FormModel = new FormModel();

  public _formSectionsQuestions: FormSectionsQuestions = new FormSectionsQuestions({ formQuestions: [], formSections: [] });
  public get formSectionsQuestions(): FormSectionsQuestions {
    return this._formSectionsQuestions;
  }

  public get disableSetCorrectAnswer(): boolean {
    return this.formData.type === FormType.Survey || this.formData.type === FormType.Poll;
  }

  @Input()
  public set formSectionsQuestions(v: FormSectionsQuestions) {
    this._formSectionsQuestions = v;
    this.questionsDicByFormSection = Utils.toDictionaryGroupBy(
      this._formSectionsQuestions.formQuestions.filter(question => question.formSectionId && !question.isDeleted),
      question => question.formSectionId
    );
    this.formSectionDic = Utils.toDictionary(this._formSectionsQuestions.formSections, section => section.id);
  }

  public _selectedQuestionId: string | undefined;
  public get selectedQuestionId(): string | undefined {
    return this._selectedQuestionId;
  }
  @Input()
  public set selectedQuestionId(v: string | undefined) {
    if (this._selectedQuestionId === v) {
      return;
    }
    this._selectedQuestionId = v;
    if (v !== undefined && this.viewInitiated) {
      Utils.delay(() => this.scrollToSelectedQuestion());
    }
  }

  @Output() public formSectionsQuestionsChange: EventEmitter<FormSectionsQuestions> = new EventEmitter<FormSectionsQuestions>();
  @Output() public formSectionsChange: EventEmitter<FormSection[]> = new EventEmitter<FormSection[]>();
  @Output() public formDataChange: EventEmitter<FormModel> = new EventEmitter<FormModel>();
  @Output() public formQuestionsDataChange: EventEmitter<FormQuestionModel[]> = new EventEmitter<FormQuestionModel[]>();
  @Output() public selectedQuestionIdChange: EventEmitter<string | undefined> = new EventEmitter<string | undefined>();
  @Output() public selectedQuestion: EventEmitter<string | undefined> = new EventEmitter<string | undefined>();
  @Output('moveQuestion') public moveQuestionEvent: EventEmitter<string> = new EventEmitter();

  public branchingOptionQuestions: FormQuestionModel[] = [];
  public orderedFormQuestionsData: FormQuestionModel[] = [];
  public uneditableFormQuestionsLength: number | undefined;
  public questionsDicByFormSection: Dictionary<FormQuestionModel[]>;
  public formSectionDic: Dictionary<FormSection>;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private elementRef: ElementRef,
    private formEditorPageService: FormEditorPageService
  ) {
    super(moduleFacadeService);
  }

  public ngAfterViewInit(): void {
    super.ngAfterViewInit();
  }

  public updateBranchingOptions(): void {
    let branchingOptions = this.formSectionsQuestions.formQuestions.filter(question => question.formSectionId == null);
    this.formSectionsQuestions.formSections.forEach(section => {
      const sectionQuestion = new FormQuestionModel();
      sectionQuestion.questionType = QuestionType.Structure;
      sectionQuestion.priority = section.priority;
      sectionQuestion.id = section.id;
      branchingOptions.push(sectionQuestion);
    });
    branchingOptions = Utils.orderBy(branchingOptions, option => option.priority);
    this.branchingOptionQuestions = branchingOptions;
  }

  public getOrderedQuestionsBySection(sectionId: string): FormQuestionModel[] {
    const questionOfSection = this.questionsDicByFormSection[sectionId];
    return questionOfSection ? Utils.orderBy(questionOfSection, question => question.minorPriority) : [];
  }

  public updateSectionsQuestionsData(actionFn: (data: FormSectionsQuestions) => void): void {
    this.formSectionsQuestions = Utils.clone(this.formSectionsQuestions, sections => {
      actionFn(sections);
    });
    this.formSectionsQuestionsChange.emit(this.formSectionsQuestions);
  }

  public onSectionQuestionDataChange(newData: FormQuestionModel): void {
    this.updateSectionsQuestionsData(sectionsQuestions => {
      const questionIndex = sectionsQuestions.formQuestions.findIndex(question => question.id === newData.id);
      if (questionIndex > -1) {
        switch (this.formData.type) {
          case FormType.Survey:
          case FormType.Poll:
            newData = newData.markQuestionAsNoRequireAnswer();
            break;
        }
        sectionsQuestions.formQuestions[questionIndex] = newData;
      }
    });
  }

  public onSectionQuestionDelete(questionId: string): void {
    this.updateSectionsQuestionsData(sectionsQuestions => {
      const deletingQuestionIndex = sectionsQuestions.formQuestions.findIndex(question => question.id === questionId);
      if (deletingQuestionIndex === -1) {
        return;
      }
      sectionsQuestions.formQuestions.forEach(question => {
        if (
          question.formSectionId === sectionsQuestions.formQuestions[deletingQuestionIndex].formSectionId &&
          question.minorPriority > sectionsQuestions.formQuestions[deletingQuestionIndex].minorPriority
        ) {
          question.minorPriority--;
        }
      });
      sectionsQuestions.formQuestions[deletingQuestionIndex].isDeleted = true;
    });
  }

  public onSectionQuestionMove(questionId: string, step: number): void {
    this.updateSectionsQuestionsData(sectionsQuestions => {
      const questionIndex = sectionsQuestions.formQuestions.findIndex(question => question.id === questionId);
      if (questionIndex === -1) {
        return;
      }
      const questionNearByIndex = sectionsQuestions.formQuestions.findIndex(
        question =>
          question.formSectionId === sectionsQuestions.formQuestions[questionIndex].formSectionId &&
          question.minorPriority === sectionsQuestions.formQuestions[questionIndex].minorPriority + step
      );
      if (questionNearByIndex === -1) {
        return;
      }
      sectionsQuestions.formQuestions[questionIndex].minorPriority += step;
      sectionsQuestions.formQuestions[questionNearByIndex].minorPriority -= step;
    });
  }

  public onSectionDelete(id: string): void {
    this.updateSectionsQuestionsData(sectionsQuestions => {
      const sectionIndex = sectionsQuestions.formSections.findIndex(section => section.id === id);
      if (sectionIndex > -1) {
        const deleteSection = sectionsQuestions.formSections[sectionIndex];
        sectionsQuestions.formSections[sectionIndex].isDeleted = true;
        sectionsQuestions.formSections.forEach(section => {
          if (section.priority > deleteSection.priority) {
            section.priority--;
          }
        });
        sectionsQuestions.formQuestions.forEach(question => {
          if (question.formSectionId === id) {
            question.isDeleted = true;
          }
          if (question.priority > deleteSection.priority) {
            question.priority--;
          }
        });
        sectionsQuestions.formQuestions = Utils.orderBy(
          Utils.orderBy(sectionsQuestions.formQuestions, question => question.minorPriority),
          question => question.priority
        );
      }
    });
  }

  public onSectionChanged(newFormSection: FormSection): void {
    const sectionIndex = this.formSectionsQuestions.formSections.findIndex(section => section.id === newFormSection.id);
    this.updateSectionsQuestionsData(sectionsQuestions => (sectionsQuestions.formSections[sectionIndex] = newFormSection));
  }

  public onSectionMove(event: { id: string; step: number }): void {
    const sectionIndex = this.formSectionsQuestions.formSections.findIndex(section => section.id === event.id);
    const movingSection = this.formSectionsQuestions.formSections[sectionIndex];
    const newPriority = movingSection.priority + event.step;
    const sectionNearByIndex = this.formSectionsQuestions.formSections.findIndex(section => section.priority === newPriority);
    this.updateSectionsQuestionsData(sectionsQuestions => {
      if (sectionNearByIndex !== -1) {
        sectionsQuestions.formSections[sectionNearByIndex].priority -= event.step;
        sectionsQuestions.formSections[sectionIndex].priority += event.step;
      } else {
        sectionsQuestions.formSections[sectionIndex].priority = movingSection.priority + event.step;
      }
      sectionsQuestions.formQuestions.forEach(question => {
        if (!question.isDeleted) {
          if (question.priority === newPriority) {
            question.priority -= event.step;
          } else if (question.priority === newPriority - event.step) {
            question.priority += event.step;
          }
        }
      });
      sectionsQuestions.formQuestions = Utils.orderBy(
        Utils.orderBy(sectionsQuestions.formQuestions, question => question.minorPriority),
        question => question.priority
      );
    });
  }

  public onQuestionChanged(newData: FormQuestionModel): void {
    switch (this.formData.type) {
      case FormType.Survey:
      case FormType.Poll:
        newData = newData.markQuestionAsNoRequireAnswer();
        break;
    }
    this.updateSectionsQuestionsData(sectionsQuestions => {
      const changeQuestionIndex = sectionsQuestions.formQuestions.findIndex(question => question.id === newData.id);
      sectionsQuestions.formQuestions[changeQuestionIndex] = newData;
    });
  }

  public updateNextQuestionIdAfterDelete(questionId: string): void {
    this.updateSectionsQuestionsData(sectionsQuestions => {
      sectionsQuestions.formQuestions.forEach(question => {
        if (question.nextQuestionId === questionId) {
          question.nextQuestionId = null;
        }
        if (question.questionOptions) {
          question.questionOptions.forEach(x => {
            if (x.nextQuestionId === questionId) {
              x.nextQuestionId = null;
            }
          });
        }
      });
      sectionsQuestions.formSections.forEach((section, index) => {
        if (section.nextQuestionId === questionId) {
          section.nextQuestionId = null;
        }
      });
    });
  }

  public formQuestionsDataTrackByFn(index: number, item: FormQuestionModel): string | FormQuestionModel {
    return item.id;
  }

  public onQuestionTemplateSelect(event: { type: QuestionType; priority: number; minorPriority: number }, formSectionId?: string): void {
    if (event.type === QuestionType.Structure) {
      const newSection: IFormSection = {
        id: Guid.create().toString(),
        formId: this.formData.id,
        priority: event.priority,
        isDeleted: false
      };
      this.updateSectionsQuestionsData(sectionsQuestions => {
        sectionsQuestions.formSections.forEach(section => {
          if (section.priority >= event.priority) {
            section.priority++;
          }
        });
        sectionsQuestions.formQuestions.forEach(question => {
          if (question.priority >= event.priority) {
            question.priority++;
          }
        });
        sectionsQuestions.formSections.push(newSection);
        sectionsQuestions.formSections = Utils.orderBy(sectionsQuestions.formSections, section => section.priority);
      });
    } else {
      this.insertNewQuestion(event.type, event.priority, event.minorPriority, formSectionId);
    }
  }

  public selectQuestion(question: FormQuestionModel): void {
    this.selectedQuestion.emit();
    if (this.selectedQuestionId === question.id) {
      return;
    }
    this.selectedQuestionId = question.id;
    this.selectedQuestionIdChange.emit(this.selectedQuestionId);
  }

  public onQuestionDelete(id: string): void {
    if (this.formData.type === FormType.Poll) {
      this.orderedFormQuestionsData.forEach(item => {
        this.processDeleteQuestion(item.id);
      });
      this.formData.isShowFreeTextQuestionInPoll = false;
      this.formDataChange.emit(this.formData);
      return;
    }
    this.processDeleteQuestion(id);
  }

  public onQuestionMoveUp(id: string): void {
    this.moveQuestion(id, -1);
  }

  public onQuestionMoveDown(id: string): void {
    this.moveQuestion(id, 1);
  }

  public moveQuestion(id: string, step: number): void {
    const movingQuestionIndex = this.formSectionsQuestions.formQuestions.findIndex(question => question.id === id);
    if (movingQuestionIndex > -1) {
      const movingQuestion = this.formSectionsQuestions.formQuestions[movingQuestionIndex];
      const newPriority = movingQuestion.priority + step;
      this.updateSectionsQuestionsData(sectionsQuestions => {
        sectionsQuestions.formQuestions.forEach(question => {
          if (!question.isDeleted) {
            if (question.priority === newPriority) {
              question.priority -= step;
            } else if (question.priority === newPriority - step) {
              question.priority += step;
            }
          }
        });
        const sectionNearByIndex = sectionsQuestions.formSections.findIndex(section => section.priority === newPriority);
        if (sectionNearByIndex > -1) {
          sectionsQuestions.formSections[sectionNearByIndex].priority -= step;
        }
      });
    }
    if (this.selectedQuestionId === id) {
      Utils.delay(() => this.scrollToSelectedQuestion());
    }
    this.moveQuestionEvent.emit(id);
  }

  public getSelectedQuestionContainer(): HTMLElement | Element | undefined {
    if (this.viewInitiated) {
      return DomUtils.findFirstElement(<HTMLElement>this.elementRef.nativeElement, p =>
        p.matches('.form-editor__question-container.-selected')
      );
    }
  }

  public scrollToSelectedQuestion(): void {
    const selectedQuestionContainer = this.getSelectedQuestionContainer();
    if (selectedQuestionContainer !== undefined) {
      DomUtils.scrollToView(selectedQuestionContainer);
    }
  }

  public showExplanationNote(): void {
    this.isShowExplanationNote = !this.isShowExplanationNote;
  }

  public toggleFreeTextQuestionInPoll(): void {
    this.isAddQuestionFreeTextInPoll = !this.isAddQuestionFreeTextInPoll;
  }

  public onQuestionImported(data: IQuestionBankSelection, formSectionId?: string): void {
    if (data && data.listQuestion && data.listQuestion.length) {
      const formQuestions = data.listQuestion.map((questionBank, index) => {
        const formQuestion = ModelMappingHelper.questionBankToFormQuestion(questionBank, this.formData.type);
        const newPriority = data.priority + index;
        const newMinorPriority = !Utils.isNullOrUndefined(data.minorPriority) ? data.minorPriority + index : data.minorPriority;
        formQuestion.priority = formSectionId ? data.priority : newPriority;
        formQuestion.minorPriority = formSectionId ? newMinorPriority : data.minorPriority;
        formQuestion.formId = this.formData.id;
        formQuestion.formSectionId = formSectionId;
        return formQuestion;
      });

      this.updateSectionsQuestionsData(sectionsQuestions => {
        sectionsQuestions.formQuestions.forEach(question => {
          if (!formSectionId && question.priority >= data.priority) {
            question.priority += data.listQuestion.length;
          } else if (formSectionId && question.formSectionId === formSectionId && question.minorPriority >= data.minorPriority) {
            question.minorPriority += data.listQuestion.length;
          }
        });
        sectionsQuestions.formQuestions = sectionsQuestions.formQuestions.concat(formQuestions);
        sectionsQuestions.formQuestions = Utils.orderBy(
          Utils.orderBy(sectionsQuestions.formQuestions, question => question.minorPriority),
          question => question.priority
        );
      });
    }
  }

  protected onChanges(changes: SimpleChanges): void {
    if (changes.formSectionsQuestions && changes.formSectionsQuestions.previousValue !== changes.formSectionsQuestions.currentValue) {
      this.formSectionsQuestions = Utils.cloneDeep(changes.formSectionsQuestions.currentValue);
      this.processQuestionsPriority(this.formSectionsQuestions.formQuestions);
      this.updateBranchingOptions();
      this.renderQuestions();
      this.formEditorPageService.formAutoSaveInformer$.pipe(this.untilDestroy()).subscribe(isAutoSaveRequest => {
        if (
          isAutoSaveRequest &&
          changes.formSectionsQuestions.previousValue &&
          changes.formSectionsQuestions.previousValue === changes.formSectionsQuestions.currentValue
        ) {
          return;
        }
        this.renderQuestions();
      });
    }
  }

  protected additionalCanSaveCheck(): Promise<boolean> {
    return Promise.all([
      ...this.questionEditors
        .toArray()
        .reverse()
        .map(p => p.validate()),
      ...this.sectionEditors
        .toArray()
        .reverse()
        .map(p => p.validate())
    ]).then(finalResult => {
      return !finalResult.includes(false);
    });
  }

  private createNewQuestion(type: QuestionType, priority: number, minorPriority: number): FormQuestionModel {
    const questionTitle = '';
    switch (type) {
      case QuestionType.FillInTheBlanks:
        return FormQuestionModel.CreateNewFillInTheBlanksQuestion(this.formData.id, priority, minorPriority, questionTitle, 1);
      case QuestionType.LongText:
        return FormQuestionModel.CreateNewLongTextQuestion(this.formData.id, priority, minorPriority, questionTitle, 1);
      case QuestionType.ShortText:
        return FormQuestionModel.CreateNewShortTextQuestion(this.formData.id, priority, minorPriority, questionTitle, 1);
      case QuestionType.TrueFalse:
        return FormQuestionModel.CreateNewTrueFalseQuestion(this.formData.id, priority, minorPriority, questionTitle, 1);
      case QuestionType.SingleChoice:
        return FormQuestionModel.CreateNewSingleChoiceQuestion(this.formData.id, priority, minorPriority, questionTitle, 1);
      case QuestionType.MultipleChoice:
        return FormQuestionModel.CreateNewMultipleChoiceQuestion(this.formData.id, priority, minorPriority, questionTitle, 1);
      case QuestionType.DropDown:
        return FormQuestionModel.CreateNewDropDownQuestion(this.formData.id, priority, minorPriority, questionTitle, 1);
      case QuestionType.DatePicker:
        return FormQuestionModel.CreateNewDatePickerQuestion(this.formData.id, priority, minorPriority, questionTitle, 1);
      case QuestionType.DateRangePicker:
        return FormQuestionModel.CreateNewDateRangePickerQuestion(this.formData.id, priority, minorPriority, questionTitle, 1);
      case QuestionType.FreeResponse:
        return FormQuestionModel.CreateNewFreeResponseQuestion(this.formData.id, priority, minorPriority, questionTitle, 1);
      default:
        return undefined;
    }
  }

  private insertNewQuestion(questionType: QuestionType, priority: number, minorPriority: number, formSectionId?: string): void {
    let newQuestion = this.createNewQuestion(questionType, priority, minorPriority);
    switch (this.formData.type) {
      case FormType.Survey:
      case FormType.Poll:
        newQuestion = newQuestion.markQuestionAsNoRequireAnswer();
        break;
      case FormType.Quiz:
        newQuestion = newQuestion.enableQuestionScore();
    }
    newQuestion.formSectionId = formSectionId;

    this.updateSectionsQuestionsData(sectionsQuestions => {
      if (!formSectionId) {
        sectionsQuestions.formSections.forEach(section => {
          if (section.priority >= priority) {
            section.priority++;
          }
        });
      }
      sectionsQuestions.formQuestions.forEach(question => {
        if (!formSectionId && question.priority >= priority) {
          question.priority++;
        } else if (formSectionId && question.formSectionId === formSectionId && question.minorPriority >= minorPriority) {
          question.minorPriority++;
        }
      });
      sectionsQuestions.formQuestions.push(newQuestion);
      sectionsQuestions.formQuestions = Utils.orderBy(
        Utils.orderBy(sectionsQuestions.formQuestions, question => question.minorPriority),
        question => question.priority
      );
    });
    setTimeout(() => {
      this.selectQuestion(newQuestion);
    });
  }

  private processDeleteQuestion(questionId: string): void {
    const questionIndex = this.formSectionsQuestions.formQuestions.findIndex(p => p.id === questionId);
    if (questionIndex > -1) {
      const deleteQuestion = this.formSectionsQuestions.formQuestions[questionIndex];
      this.updateSectionsQuestionsData(sectionsQuestions => {
        this.selectedQuestionId = undefined;
        this.selectedQuestionIdChange.emit(this.selectedQuestionId);
        sectionsQuestions.formQuestions[questionIndex].isDeleted = true;
        sectionsQuestions.formQuestions.forEach(question => {
          if (question.priority > sectionsQuestions.formQuestions[questionIndex].priority) {
            question.priority--;
          }
        });
        sectionsQuestions.formSections.forEach(section => {
          if (section.priority > deleteQuestion.priority) {
            section.priority--;
          }
        });
      });
    }
    if (this.formData.type === FormType.Survey) {
      this.updateNextQuestionIdAfterDelete(questionId);
    }
  }

  private renderQuestions(): void {
    this.orderedFormQuestionsData = Utils.orderBy(
      Utils.orderBy(this.formSectionsQuestions.formQuestions, question => question.minorPriority),
      question => question.priority
    ).filter(p => !p.isDeleted && p.formSectionId == null);
    const orderedFormSections = Utils.orderBy(
      this.formSectionsQuestions.formSections.filter(section => !section.isDeleted),
      section => section.priority
    );
    orderedFormSections.forEach(section => {
      const sectionQuestion = new FormQuestionModel();
      sectionQuestion.questionType = QuestionType.Structure;
      sectionQuestion.priority = section.priority;
      sectionQuestion.formSectionId = section.id;
      this.orderedFormQuestionsData.splice(section.priority, 0, sectionQuestion);
    });
    this.uneditableFormQuestionsLength = this.orderedFormQuestionsData.filter(question => question.isSurveyTemplateQuestion).length;
  }

  private processQuestionsPriority(formQuestionData: FormQuestionModel[]): void {
    let newFormQuestions = Utils.cloneDeep(formQuestionData);
    this.formSectionsQuestions.formSections.forEach((section, index) => {
      if (this.questionsDicByFormSection[section.id] == null) {
        const sectionQuestion = new FormQuestionModel();
        sectionQuestion.questionType = QuestionType.Structure;
        sectionQuestion.priority = section.priority;
        sectionQuestion.formSectionId = section.id;
        newFormQuestions.splice(section.priority, 0, sectionQuestion);
      }
    });
    let priority = 0,
      minorPriority = 0;
    newFormQuestions.forEach((question, i) => {
      // set priority for not deleted question
      if (!question.isDeleted) {
        question.priority = priority;
        if (
          question.formSectionId == null ||
          (newFormQuestions[i + 1] && question.formSectionId !== newFormQuestions[i + 1].formSectionId)
        ) {
          priority++;
        }
        // set minor priority for question for section only
        if (question.formSectionId) {
          question.minorPriority = minorPriority;
        }
        minorPriority =
          question.formSectionId && newFormQuestions[i + 1] && question.formSectionId === newFormQuestions[i + 1].formSectionId
            ? minorPriority + 1
            : 0;
      }
    });
    newFormQuestions = newFormQuestions.filter(question => question.questionType !== QuestionType.Structure);
    formQuestionData = Utils.cloneDeep(newFormQuestions);
  }
}
