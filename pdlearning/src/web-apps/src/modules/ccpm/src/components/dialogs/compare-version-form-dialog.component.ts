import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import {
  FormApiService,
  FormDataModel,
  FormQuestionModel,
  FormType,
  IFormQuestionModel,
  QuestionAnswerSingleValue,
  QuestionAnswerValue,
  QuestionOptionType,
  QuestionType,
  VersionTrackingViewModel
} from '@opal20/domain-api';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { DiffHelper } from '@opal20/domain-components';

@Component({
  selector: 'compare-version-form',
  templateUrl: './compare-version-form-dialog.component.html'
})
export class CompareVersionFormDialogComponent extends BaseComponent {
  @Input() public oldVersionTrackingVm: VersionTrackingViewModel | undefined;
  @Input() public newVersionTrackingVm: VersionTrackingViewModel | undefined;
  @Input() public type: FormType;

  public compareParameter: CompareFormParameter = new CompareFormParameter();
  public nothingToCompare: boolean = false;

  public formQuestionOld: IFormQuestionModel[] = [];
  public formQuestionNew: IFormQuestionModel[] = [];
  public isFinishCompareProcess: boolean = false;

  public readonly compareSide = CompareSide;

  constructor(public moduleFacadeService: ModuleFacadeService, public formApiService: FormApiService, public dialogRef: DialogRef) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.processCompareQuestion();
  }

  public onCancel(): void {
    this.dialogRef.close();
  }

  public getVersionString(version: string): string {
    return version.replace('v', 'Version ');
  }

  public processCompareQuestion(): void {
    Promise.resolve(this.compareParameter)
      .then(() => this.initCompareData(this.compareParameter))
      .then(() => this.comparingSections(this.compareParameter))
      .then(() => this.comparingQuestions(this.compareParameter))
      .then(() => this.processingQuestionsTitle(this.compareParameter))
      .then(() => this.comparingQuestionsOption(this.compareParameter))
      .then(() => this.processingOptionDetail(this.compareParameter))
      .then(() => (this.isFinishCompareProcess = true))
      .catch(err => {
        return;
      });
  }

  public canShowQuestionComparison(): boolean {
    return this.type === FormType.Poll || this.type === FormType.Quiz || this.type === FormType.Survey;
  }

  private initCompareData(parameter: CompareFormParameter): Promise<[void, void]> {
    const getOldVersionTrackingData = this.formApiService
      .getFormDataByVersionTrackingId(this.oldVersionTrackingVm.id, true)
      .then(result => {
        parameter.oldComparisonFormViewModels = this.buildSectionComparisonModel(result);
      });

    const getNewVersionTrackingData = this.formApiService
      .getFormDataByVersionTrackingId(this.newVersionTrackingVm.id, true)
      .then(result => {
        parameter.newComparisonFormViewModels = this.buildSectionComparisonModel(result);
      });

    return Promise.all([getOldVersionTrackingData, getNewVersionTrackingData]);
  }

  // Comparision Level 1
  private comparingSections(parameter: CompareFormParameter): Promise<void> {
    if (!parameter.oldComparisonFormViewModels.length && !parameter.newComparisonFormViewModels.length) {
      this.markAsNothingToCompare();
      return Promise.reject();
    }

    if (!parameter.oldComparisonFormViewModels.length && parameter.newComparisonFormViewModels.length) {
      this.markAsAllIsNew();
      return Promise.reject();
    }

    if (parameter.oldComparisonFormViewModels.length && !parameter.newComparisonFormViewModels.length) {
      this.markAsAllIsRemove();
      return Promise.reject();
    }

    if (JSON.stringify(parameter.oldComparisonFormViewModels) === JSON.stringify(parameter.newComparisonFormViewModels)) {
      this.markAsNothingHaveBeenChanged();
      return Promise.reject();
    }

    const oldVm: IComparisonFormViewModel[] = [];
    const newVm: IComparisonFormViewModel[] = [];

    parameter.oldComparisonFormViewModels.forEach((item, index) => {
      const newItemAtSameIndex = parameter.newComparisonFormViewModels[index];

      // Condition that ensure 100% comparison items are changed
      // -- Question have been removed
      if (newItemAtSameIndex === undefined) {
        const vm = item;
        vm.changeType = ItemChangeType.Removed;
        oldVm.push(vm);
        newVm.push(vm);
        return;
      }

      // -- Section -> Question (opposite)
      const isSectionOrQuestionChanged = item.isSection !== newItemAtSameIndex.isSection;
      // -- Question and Question Type have been changed
      const isQuestionTypeChanged =
        !item.isSection &&
        !newItemAtSameIndex.isSection &&
        item.formQuestions[0].questionType !== newItemAtSameIndex.formQuestions[0].questionType;

      if (isSectionOrQuestionChanged || isQuestionTypeChanged) {
        const removeVm = item;
        removeVm.changeType = ItemChangeType.Removed;
        oldVm.push(removeVm);
        newVm.push(removeVm);

        const addNewVm = newItemAtSameIndex;
        addNewVm.changeType = ItemChangeType.AddNew;
        oldVm.push(addNewVm);
        newVm.push(addNewVm);
        return;
      }

      // Condition that comparison items are none changed
      if (item.id === newItemAtSameIndex.id || !isSectionOrQuestionChanged || !isQuestionTypeChanged) {
        item.changeType = ItemChangeType.NoneChange;
        newItemAtSameIndex.changeType = ItemChangeType.NoneChange;
        oldVm.push(item);
        newVm.push(newItemAtSameIndex);
        return;
      }
    });

    parameter.newComparisonFormViewModels.forEach((item, index) => {
      const oldItemAtSameIndex = parameter.oldComparisonFormViewModels[index];
      if (oldItemAtSameIndex === undefined) {
        const vm = item;
        vm.changeType = ItemChangeType.AddNew;
        oldVm.push(vm);
        newVm.push(vm);
        return;
      }
    });

    parameter.oldComparisonFormViewModels = oldVm;
    parameter.newComparisonFormViewModels = newVm;
    Promise.resolve();
  }

  // Comparison Level 2
  // Compare the questions inside a section that none changed after compare level 1;
  private comparingQuestions(parameter: CompareFormParameter): Promise<void> {
    parameter.oldComparisonFormViewModels.forEach((item, index) => {
      if (item.isSection && item.changeType === ItemChangeType.NoneChange) {
        const newSectionAtSameIndex = parameter.newComparisonFormViewModels[index];

        // Case don't have any question;
        if (!item.formQuestions.length && !newSectionAtSameIndex.formQuestions.length) {
          return;
        }

        // Case all questions is add new;
        if (!item.formQuestions.length && newSectionAtSameIndex.formQuestions.length) {
          newSectionAtSameIndex.formQuestions = newSectionAtSameIndex.formQuestions = newSectionAtSameIndex.formQuestions.map(x => {
            x.changeType.push(QuestionChangeType.AddNew);
            return x;
          });
          item.formQuestions = newSectionAtSameIndex.formQuestions;
          return;
        }

        // Case all questions is removed;
        if (item.formQuestions.length && !newSectionAtSameIndex.formQuestions.length) {
          item.formQuestions = item.formQuestions = item.formQuestions.map(x => {
            x.changeType.push(QuestionChangeType.AddNew);
            return x;
          });

          newSectionAtSameIndex.formQuestions = item.formQuestions;
          return;
        }

        const oldQuests: IComparisonQuestionViewModel[] = [];
        const newQuests: IComparisonQuestionViewModel[] = [];

        item.formQuestions.forEach((quest, priority) => {
          if (newSectionAtSameIndex.formQuestions[priority] === undefined) {
            quest.changeType.push(QuestionChangeType.Removed);
            oldQuests.push(quest);
            newQuests.push(quest);
            return;
          }

          if (quest.questionType !== newSectionAtSameIndex.formQuestions[priority].questionType) {
            quest.changeType.push(QuestionChangeType.Removed);
            oldQuests.push(quest);
            newQuests.push(quest);

            newSectionAtSameIndex.formQuestions[priority].changeType.push(QuestionChangeType.AddNew);
            oldQuests.push(newSectionAtSameIndex.formQuestions[priority]);
            newQuests.push(newSectionAtSameIndex.formQuestions[priority]);
            return;
          }

          if (
            quest.questionType === newSectionAtSameIndex.formQuestions[priority].questionType ||
            quest.id === newSectionAtSameIndex.formQuestions[priority].id
          ) {
            quest.changeType.push(QuestionChangeType.NoneChange);
            oldQuests.push(quest);
            newQuests.push(newSectionAtSameIndex.formQuestions[priority]);
            return;
          }
        });

        newSectionAtSameIndex.formQuestions.forEach((quest, ind) => {
          const oldQuest = item.formQuestions[ind];
          if (oldQuest === undefined) {
            quest.changeType.push(QuestionChangeType.AddNew);
            oldQuests.push(quest);
            newQuests.push(quest);
          }
        });

        item.formQuestions = oldQuests;
        newSectionAtSameIndex.formQuestions = newQuests;
      }
    });
    return Promise.resolve();
  }

  private processingQuestionsTitle(parameter: CompareFormParameter): Promise<void> {
    parameter.oldComparisonFormViewModels.forEach((item, index) => {
      if (item.changeType === ItemChangeType.NoneChange) {
        const newSectionInNewVersion = parameter.newComparisonFormViewModels[index];
        const diffHelper = new DiffHelper();
        if (item.isSection) {
          if (item.sectionTitle !== newSectionInNewVersion.sectionTitle) {
            const diffTitle = diffHelper.getDiffString(item.sectionTitle, newSectionInNewVersion.sectionTitle);
            newSectionInNewVersion.sectionTitle = diffTitle.newResource;
            item.sectionTitle = diffTitle.oldResource;
          }

          if (item.sectionDescription !== newSectionInNewVersion.sectionDescription) {
            const diffDescription = diffHelper.getDiffString(item.sectionDescription, newSectionInNewVersion.sectionDescription);
            newSectionInNewVersion.sectionDescription = diffDescription.newResource;
            item.sectionDescription = diffDescription.oldResource;
          }
        }

        item.formQuestions.forEach((question, priority) => {
          if (question.changeType.includes(QuestionChangeType.NoneChange) && question.questionType !== QuestionType.FillInTheBlanks) {
            if (question.questionTitle !== newSectionInNewVersion.formQuestions[priority].questionTitle) {
              const diffTitle = diffHelper.diffTwoHtml(
                question.questionTitle,
                newSectionInNewVersion.formQuestions[priority].questionTitle
              );

              question.questionTitle = diffTitle.newResource;
              newSectionInNewVersion.formQuestions[priority].questionTitle = diffTitle.oldResource;
            }
          }

          if (question.changeType.includes(QuestionChangeType.NoneChange)) {
            if (question.description !== newSectionInNewVersion.formQuestions[priority].description) {
              const diffTitle = diffHelper.diffTwoHtml(question.description, newSectionInNewVersion.formQuestions[priority].description);

              question.description = diffTitle.newResource;
              newSectionInNewVersion.formQuestions[priority].description = diffTitle.oldResource;
            }

            if (question.score !== newSectionInNewVersion.formQuestions[priority].score) {
              const diffTitle = diffHelper.diffTwoHtml(question.score, newSectionInNewVersion.formQuestions[priority].score);

              question.score = diffTitle.newResource;
              newSectionInNewVersion.formQuestions[priority].score = diffTitle.oldResource;
            }
          }
        });
      }
    });
    return Promise.resolve();
  }

  private comparingQuestionsOption(parameter: CompareFormParameter): Promise<void> {
    parameter.oldComparisonFormViewModels.forEach((item, index) => {
      const newItemAtSameIndex = parameter.newComparisonFormViewModels[index];

      if (item.changeType === ItemChangeType.NoneChange) {
        if (JSON.stringify(item.formQuestions) === JSON.stringify(newItemAtSameIndex.formQuestions)) {
          return;
        }

        item.formQuestions.forEach((question, questIndex) => {
          const newVerOptions = newItemAtSameIndex.formQuestions[questIndex].questionOptions;
          // Case all option is add new
          if (!question.questionOptions.length && newVerOptions.length) {
            const options = newItemAtSameIndex.formQuestions[questIndex].questionOptions.map(option => {
              option.changeType = [QuestionOptionChangeType.AddNew];
              return option;
            });

            newItemAtSameIndex.formQuestions[questIndex].questionOptions = options;
            question.questionOptions = options;
            return;
          }

          // Case all option is removed
          if (!newVerOptions && question.questionOptions.length) {
            const options = question.questionOptions.map(option => {
              option.changeType = [QuestionOptionChangeType.Removed];
              return option;
            });

            newItemAtSameIndex.formQuestions[questIndex].questionOptions = options;
            question.questionOptions = options;
            return;
          }

          const oldOptions: IComparisonQuestionOptionViewModel[] = [];
          const newOptions: IComparisonQuestionOptionViewModel[] = [];
          question.questionOptions.forEach((option, priority) => {
            const newOptionAtSameIndex = newItemAtSameIndex.formQuestions[questIndex].questionOptions[priority];
            if (newOptionAtSameIndex === undefined) {
              option.changeType = [QuestionOptionChangeType.Removed];
              oldOptions.push(option);
              newOptions.push(option);
              return;
            }

            option.changeType = [QuestionOptionChangeType.NoneChange];
            newOptionAtSameIndex.changeType = [QuestionOptionChangeType.NoneChange];
            oldOptions.push(option);
            newOptions.push(newOptionAtSameIndex);
          });

          newItemAtSameIndex.formQuestions[questIndex].questionOptions.forEach((option, priority) => {
            const oldOptionAtSameIndex = question.questionOptions[priority];
            if (oldOptionAtSameIndex === undefined) {
              option.changeType = [QuestionOptionChangeType.AddNew];
              oldOptions.push(option);
              newOptions.push(option);
              return;
            }
          });

          question.questionOptions = oldOptions;
          parameter.newComparisonFormViewModels[index].formQuestions[questIndex].questionOptions = newOptions;
        });
      }
    });

    return Promise.resolve();
  }

  // Getting and processing all question options detail with state None Change after 4 round comparison before
  private processingOptionDetail(parameter: CompareFormParameter): Promise<void> {
    const diffHelper = new DiffHelper();
    parameter.oldComparisonFormViewModels.forEach((oldComparesionVm, index) => {
      if (oldComparesionVm.changeType === ItemChangeType.NoneChange) {
        oldComparesionVm.formQuestions.forEach((question, questIndex) => {
          if (question.changeType.includes(QuestionChangeType.NoneChange)) {
            question.questionOptions.forEach((option, optionIndex) => {
              if (this.canCheckCorrectAnswerCheckedChange(question.questionType)) {
                if (
                  option.changeType.includes(QuestionOptionChangeType.NoneChange) &&
                  option.isCorrectAnswer !==
                    parameter.newComparisonFormViewModels[index].formQuestions[questIndex].questionOptions[optionIndex].isCorrectAnswer
                ) {
                  const changeType = option.isCorrectAnswer
                    ? QuestionOptionChangeType.CorrectAnswerRemoved
                    : QuestionOptionChangeType.CorrectAnswerChecked;
                  option.changeType.push(changeType);
                  parameter.newComparisonFormViewModels[index].formQuestions[questIndex].questionOptions[optionIndex].changeType.push(
                    changeType
                  );
                }
              }
              // Option Value Checking
              if (this.canCheckOptionValueChanged(question.questionType)) {
                const newOption = parameter.newComparisonFormViewModels[index].formQuestions[questIndex].questionOptions[optionIndex];

                const newValue = newOption.isEmptyValue ? '' : newOption.value.toString();
                const oldValue = option.isEmptyValue ? '' : option.value.toString();

                if (option.changeType.includes(QuestionOptionChangeType.NoneChange) && oldValue !== newValue) {
                  const diffTitle = diffHelper.getDiffString(oldValue, newValue);

                  parameter.newComparisonFormViewModels[index].formQuestions[questIndex].questionOptions[optionIndex].value =
                    diffTitle.newResource;
                  parameter.newComparisonFormViewModels[index].formQuestions[questIndex].questionOptions[optionIndex].isEmptyValue = false;

                  option.value = diffTitle.oldResource;
                  option.isEmptyValue = false;
                }
              }
              // Option Media Checking
              if (this.canCheckMediaChanged(question.questionType)) {
                const newImage =
                  parameter.newComparisonFormViewModels[index].formQuestions[questIndex].questionOptions[optionIndex].imageUrl;

                const newVideo =
                  parameter.newComparisonFormViewModels[index].formQuestions[questIndex].questionOptions[optionIndex].videoUrl;

                // Case Media Option None Changed
                if (option.imageUrl === newImage && option.videoUrl === newVideo) {
                  return;
                }

                // Case Remove Option Image
                if (option.imageUrl && !newImage && !newVideo) {
                  option.changeType.push(QuestionOptionChangeType.MediaOptionRemoved);

                  parameter.newComparisonFormViewModels[index].formQuestions[questIndex].questionOptions[optionIndex].imageUrl =
                    option.imageUrl;

                  parameter.newComparisonFormViewModels[index].formQuestions[questIndex].questionOptions[optionIndex].changeType.push(
                    QuestionOptionChangeType.MediaOptionRemoved
                  );
                }

                // Case Remove Option Video
                if (option.videoUrl && !newImage && !newVideo) {
                  option.changeType.push(QuestionOptionChangeType.MediaOptionRemoved);

                  parameter.newComparisonFormViewModels[index].formQuestions[questIndex].questionOptions[optionIndex].videoUrl =
                    option.videoUrl;

                  parameter.newComparisonFormViewModels[index].formQuestions[questIndex].questionOptions[optionIndex].changeType.push(
                    QuestionOptionChangeType.MediaOptionRemoved
                  );
                }

                // Case Media Option Changed
                if (
                  (newImage || newVideo) &&
                  (option.imageUrl || option.videoUrl) &&
                  (option.imageUrl !== newImage || option.videoUrl !== newVideo)
                ) {
                  option.changeType.push(QuestionOptionChangeType.MediaOptionChanged);

                  parameter.newComparisonFormViewModels[index].formQuestions[questIndex].questionOptions[optionIndex].changeType.push(
                    QuestionOptionChangeType.MediaOptionChanged
                  );
                }

                // Case Add New Option Image
                if (!option.imageUrl && !option.videoUrl && newImage) {
                  option.changeType.push(QuestionOptionChangeType.MediaOptionAddNew);

                  option.imageUrl = newImage;

                  parameter.newComparisonFormViewModels[index].formQuestions[questIndex].questionOptions[optionIndex].changeType.push(
                    QuestionOptionChangeType.MediaOptionAddNew
                  );
                }

                // Case Add New Option Video
                if (!option.imageUrl && !option.videoUrl && newVideo) {
                  option.changeType.push(QuestionOptionChangeType.MediaOptionAddNew);

                  option.videoUrl = newVideo;

                  parameter.newComparisonFormViewModels[index].formQuestions[questIndex].questionOptions[optionIndex].changeType.push(
                    QuestionOptionChangeType.MediaOptionAddNew
                  );
                }
              }
            });
            // Case Free Text
            if (this.isFreeTextQuestion(question.questionType)) {
              const oldCorrectAnswer = question.questionCorrectAnswer;
              const newCorrectAnswer = parameter.newComparisonFormViewModels[index].formQuestions[questIndex].questionCorrectAnswer;

              if (oldCorrectAnswer !== newCorrectAnswer) {
                const diffTitle = diffHelper.getDiffString(
                  question.questionCorrectAnswer ? question.questionCorrectAnswer.toString() : '',
                  newCorrectAnswer ? newCorrectAnswer.toString() : ''
                );

                question.questionCorrectAnswer = diffTitle.oldResource;
                parameter.newComparisonFormViewModels[index].formQuestions[questIndex].questionCorrectAnswer = diffTitle.newResource;
              }
            }
            // Case One Date Picker
            if (this.isDatePicker(question.questionType)) {
              const oldDate = (question.questionCorrectAnswer as unknown) as Date;
              const newDate = (parameter.newComparisonFormViewModels[index].formQuestions[questIndex]
                .questionCorrectAnswer as unknown) as Date;

              if (oldDate !== newDate) {
                question.changeType.push(QuestionChangeType.DatePickerChange);
                parameter.newComparisonFormViewModels[index].formQuestions[questIndex].changeType.push(QuestionChangeType.DatePickerChange);
              }
            }
            // Case Date Range Picker
            if (this.isDateRangePicker(question.questionType)) {
              const oldCorrectAnswer = question.questionCorrectAnswer;
              const newCorrectAnswer = parameter.newComparisonFormViewModels[index].formQuestions[questIndex].questionCorrectAnswer;

              const oldFromDate = oldCorrectAnswer ? ((oldCorrectAnswer[0] as unknown) as Date) : undefined;
              const newFromDate = newCorrectAnswer ? ((newCorrectAnswer[0] as unknown) as Date) : undefined;

              if (oldFromDate !== newFromDate) {
                question.changeType.push(QuestionChangeType.FromDateChange);
                parameter.newComparisonFormViewModels[index].formQuestions[questIndex].changeType.push(QuestionChangeType.FromDateChange);
              }

              const oldToDate = oldCorrectAnswer ? ((oldCorrectAnswer[1] as unknown) as Date) : undefined;
              const newToDate = newCorrectAnswer ? ((newCorrectAnswer[1] as unknown) as Date) : undefined;

              if (oldToDate !== newToDate) {
                question.changeType.push(QuestionChangeType.ToDateChange);
                parameter.newComparisonFormViewModels[index].formQuestions[questIndex].changeType.push(QuestionChangeType.ToDateChange);
              }
            }
          }
        });
      }
    });
    return Promise.resolve();
  }

  private canCheckCorrectAnswerCheckedChange(type: QuestionType): boolean {
    return (
      type === QuestionType.TrueFalse ||
      type === QuestionType.MultipleChoice ||
      type === QuestionType.DropDown ||
      type === QuestionType.SingleChoice
    );
  }

  private canCheckOptionValueChanged(type: QuestionType): boolean {
    return (
      type === QuestionType.MultipleChoice ||
      type === QuestionType.DropDown ||
      type === QuestionType.SingleChoice ||
      type === QuestionType.FillInTheBlanks ||
      type === QuestionType.Criteria
    );
  }

  private canCheckMediaChanged(type: QuestionType): boolean {
    return type === QuestionType.MultipleChoice || type === QuestionType.SingleChoice;
  }

  private isFreeTextQuestion(type: QuestionType): boolean {
    return type === QuestionType.ShortText;
  }

  private isDatePicker(type: QuestionType): boolean {
    return type === QuestionType.DatePicker;
  }

  private isDateRangePicker(type: QuestionType): boolean {
    return type === QuestionType.DateRangePicker;
  }

  private markAsAllIsNew(): void {
    const addNewItems = this.compareParameter.newComparisonFormViewModels.map(item => {
      item.changeType = ItemChangeType.AddNew;
      return item;
    });

    this.compareParameter.oldComparisonFormViewModels = addNewItems;
    this.compareParameter.newComparisonFormViewModels = addNewItems;

    this.isFinishCompareProcess = true;
  }

  private markAsAllIsRemove(): void {
    const removedItems = this.compareParameter.oldComparisonFormViewModels.map(item => {
      item.changeType = ItemChangeType.Removed;
      return item;
    });

    this.compareParameter.oldComparisonFormViewModels = removedItems;
    this.compareParameter.newComparisonFormViewModels = removedItems;

    this.isFinishCompareProcess = true;
  }

  private markAsNothingToCompare(): void {
    this.nothingToCompare = true;
    this.isFinishCompareProcess = true;
  }

  private markAsNothingHaveBeenChanged(): void {
    const items = this.compareParameter.oldComparisonFormViewModels.map(item => {
      item.changeType = ItemChangeType.NoneChange;
      return item;
    });

    this.compareParameter.oldComparisonFormViewModels = items;
    this.compareParameter.newComparisonFormViewModels = items;

    this.isFinishCompareProcess = true;
  }

  // Map Form Questions Model to Section Comparison Model
  private buildSectionComparisonModel(formData: FormDataModel): IComparisonFormViewModel[] {
    const sectionsGroup: IComparisonFormViewModel[] = [];
    const formQuestions = formData.formQuestions;
    const formSections = formData.formSections;

    for (const question of formQuestions) {
      const questionVm = this.buildComparisonQuestionViewModel(question);

      if (!question.formSectionId) {
        const sectionVm: IComparisonFormViewModel = {
          id: question.id,
          isSection: false,
          changeType: ItemChangeType.NoneChange,
          formQuestions: [questionVm]
        };

        sectionsGroup.push(sectionVm);
      } else {
        const section = sectionsGroup.find(s => s.id === question.formSectionId);
        const sectionIdx = sectionsGroup.findIndex(s => s.id === question.formSectionId);

        if (section) {
          sectionsGroup[sectionIdx].formQuestions.push(questionVm);
        } else {
          const sectionFormInfo = formData.formSections.find(s => s.id === question.formSectionId);
          const title = sectionFormInfo.mainDescription ? sectionFormInfo.mainDescription : '';
          const desc = sectionFormInfo.additionalDescription ? sectionFormInfo.additionalDescription : '';
          const priorty = sectionFormInfo.priority;
          const sectionVm: IComparisonFormViewModel = {
            id: question.formSectionId,
            formQuestions: [questionVm],
            isSection: true,
            sectionTitle: title,
            sectionPrioprity: priorty,
            sectionDescription: desc,
            changeType: ItemChangeType.NoneChange
          };

          sectionsGroup.push(sectionVm);
        }
      }
    }

    if (formSections.length) {
      formSections.forEach(section => {
        // For case if section not have any question
        const question = formQuestions.find(s => s.formSectionId === section.id);
        if (!question) {
          const title = section.mainDescription ? section.mainDescription : '';
          const desc = section.additionalDescription ? section.additionalDescription : '';
          const sectionVm: IComparisonFormViewModel = {
            id: section.id,
            formQuestions: [],
            isSection: true,
            sectionTitle: title,
            sectionPrioprity: section.priority,
            sectionDescription: desc
          };

          sectionsGroup.push(sectionVm);
        }
      });
    }

    return Utils.orderBy(sectionsGroup, p => p.sectionPrioprity);
  }

  private buildComparisonQuestionViewModel(question: FormQuestionModel): IComparisonQuestionViewModel {
    return {
      id: question.id,
      changeType: [QuestionChangeType.NoneChange],
      questionType: question.questionType,
      questionTitle: question.questionTitle,
      questionCorrectAnswer: question.questionCorrectAnswer,
      priority: question.priority,
      minorPriority: question.minorPriority,
      score: question.score ? question.score.toString() : undefined,
      description: question.description,
      questionOptions: this.buildComparisonQuestionOptionViewModel(question)
    } as IComparisonQuestionViewModel;
  }

  private buildComparisonQuestionOptionViewModel(question: FormQuestionModel): IComparisonQuestionOptionViewModel[] {
    const vmResult: IComparisonQuestionOptionViewModel[] = [];

    for (const option of question.questionOptions || []) {
      const vm = {
        code: option.code,
        value: option.value,
        type: option.type,
        feedback: option.feedback,
        imageUrl: option.imageUrl,
        videoUrl: option.videoUrl,
        isEmptyValue: option.isEmptyValue,
        scaleId: option.scaleId,
        changeType: [QuestionOptionChangeType.NoneChange]
      } as IComparisonQuestionOptionViewModel;

      vm.isCorrectAnswer = this.isAnswerValueCorrect(option.value, question);
      vmResult.push(vm);
    }

    return vmResult;
  }

  private isAnswerValueCorrect(optionValue: QuestionAnswerSingleValue, question: FormQuestionModel): boolean {
    if (Utils.isNullOrUndefined(question.questionCorrectAnswer) || Utils.isNullOrUndefined(optionValue)) {
      return false;
    }

    if (question.questionCorrectAnswer === optionValue) {
      return true;
    }

    if (question.questionType === QuestionType.MultipleChoice) {
      return this.isOptionValueCorrect(optionValue, question);
    }

    return false;
  }

  private isOptionValueCorrect(optionValue: QuestionAnswerSingleValue, question: FormQuestionModel): boolean {
    if (Utils.isNullOrUndefined(question.questionCorrectAnswer)) {
      return false;
    }

    if (question.questionCorrectAnswer === optionValue) {
      return true;
    }

    if (question.questionCorrectAnswer instanceof Array && question.questionCorrectAnswer.indexOf(optionValue) >= 0) {
      return true;
    }

    return false;
  }
}

export interface IComparisonFormViewModel {
  // Question or section id
  id: string;
  isSection: boolean;
  sectionTitle?: string;
  sectionDescription?: string;
  sectionPrioprity?: number;
  changeType?: ItemChangeType;
  formQuestions: IComparisonQuestionViewModel[];
}

export interface IComparisonQuestionViewModel {
  id?: string | undefined;
  formId?: string;
  questionType?: QuestionType;
  questionTitle?: string;
  questionCorrectAnswer?: QuestionAnswerValue | undefined;
  priority?: number;
  score: string | undefined;
  minorPriority?: number;
  questionLevel?: number | undefined;
  questionOptions?: IComparisonQuestionOptionViewModel[] | undefined;
  changeType?: QuestionChangeType[];
  description?: string;
}

export interface IComparisonQuestionOptionViewModel {
  code: number;
  value: QuestionAnswerSingleValue;
  type: QuestionOptionType | undefined;
  feedback: string | undefined;
  imageUrl: string | undefined;
  videoUrl: string | undefined;
  isEmptyValue: boolean | false;
  isCorrectAnswer: boolean | false;
  scaleId: string | undefined;
  changeType: QuestionOptionChangeType[];
}

export class CompareFormParameter {
  constructor(
    public oldComparisonFormViewModels: IComparisonFormViewModel[] = [],
    public newComparisonFormViewModels: IComparisonFormViewModel[] = []
  ) {}
}

export enum ItemChangeType {
  NoneChange = 'NoneChange',
  Removed = 'Removed',
  AddNew = 'AddNew'
}

export enum QuestionChangeType {
  NoneChange = 'NoneChange',
  Removed = 'Removed',
  AddNew = 'AddNew',
  DatePickerChange = 'DatePickerChange',
  FromDateChange = 'FromDateChange',
  ToDateChange = 'ToDateChange'
}

export enum QuestionOptionChangeType {
  NoneChange = 'NoneChange',
  Removed = 'Removed',
  AddNew = 'AddNew',
  MediaOptionChanged = 'MediaOptionChanged',
  MediaOptionRemoved = 'MediaOptionRemoved',
  MediaOptionAddNew = 'MediaOptionAddNew',
  CorrectAnswerRemoved = 'CorrectAnswerRemoved',
  CorrectAnswerChecked = 'CorrectAnswerChecked',
  ValueChange = 'ValueChange'
}

export enum CompareSide {
  Left = 'Left',
  Right = 'Right'
}
