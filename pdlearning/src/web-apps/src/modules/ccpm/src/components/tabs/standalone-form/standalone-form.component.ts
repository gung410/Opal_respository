import { BasePageComponent, ClipboardUtil, ModuleFacadeService, NotificationType } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import {
  FormAnswerApiService,
  FormModel,
  FormParticipantApiService,
  FormParticipantStatus,
  FormParticipantType,
  FormParticipantViewModel,
  FormQuestionModel,
  FormSectionViewModel,
  FormSectionsQuestions,
  FormStandaloneMode,
  FormStatus,
  IDeleteFormParticipantsRequest,
  IFormParticipantViewModel,
  IFormSectionViewModel,
  IRemindFormParticipantsRequest,
  SystemRoleEnum,
  UserInfoModel
} from '@opal20/domain-api';
import {
  FormDetailMode,
  ListParticipantGridComponent,
  StandaloneSurveyDetailMode,
  StandaloneSurveyEditModeService,
  WebAppLinkBuilder
} from '@opal20/domain-components';

import { AddParticipantDialogComponent } from '../../dialogs/add-participant-dialog.component';
import { ButtonAction } from '@opal20/common-components';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { FormEditModeService } from '../../../services/form-edit-mode.service';
import { StandaloneFormAnswerReviewDialogComponent } from './standalone-form-answer-review-dialog.component';
import { orderBy } from 'lodash-es';

@Component({
  selector: 'standalone-form',
  templateUrl: './standalone-form.component.html',
  encapsulation: ViewEncapsulation.None
})
export class StandaloneFormComponent extends BasePageComponent {
  @Input() public isFromLNASurveys: boolean = false;
  @Input() public originalObjectId: string = '';
  @Input() public formData: FormModel;
  @Input() public formSectionsQuestions: FormSectionsQuestions;
  @Output() public formDataChange: EventEmitter<FormModel> = new EventEmitter<FormModel>();

  @Input() public set formParticipantType(value: FormParticipantType) {
    this.formParticipantApiService.initApiService(value);
  }

  /*** Listing variables ***/
  public groupButtons: ButtonAction<IFormParticipantViewModel>[] = [
    {
      id: 'remind ',
      text: this.translateCommon('Remind'),
      conditionFn: () => this.canRemind,
      actionFn: dataItems => Promise.resolve(this.remindParticipants(dataItems)),
      hiddenFn: null
    },
    {
      id: 'remove',
      text: this.translateCommon('Remove'),
      conditionFn: () => this.canEditQuestion,
      actionFn: dataItems => Promise.resolve(this.deleteParticipants(dataItems)),
      hiddenFn: null
    }
  ];

  public checkedParticipants: FormParticipantViewModel[] = [];
  public formDetailMode: FormDetailMode = this.formEditModeService.initMode;
  public standaloneSurveyDetailMode: StandaloneSurveyDetailMode = this.standaloneSurveyEditModeService.initMode;
  public currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();
  public get isPublicMode(): boolean {
    return this.formData.standaloneMode === FormStandaloneMode.Public;
  }

  @ViewChild('listParticipant', { static: false })
  private listParticipantComponent: ListParticipantGridComponent;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private formParticipantApiService: FormParticipantApiService,
    private formEditModeService: FormEditModeService,
    private formAnswerApiService: FormAnswerApiService,
    private standaloneSurveyEditModeService: StandaloneSurveyEditModeService
  ) {
    super(moduleFacadeService);
  }

  public get isSystemAdmin(): boolean {
    return this.currentUser.hasRole(SystemRoleEnum.SystemAdministrator);
  }

  public get canEditQuestion(): boolean {
    return this.isFromLNASurveys
      ? this.standaloneSurveyDetailMode !== StandaloneSurveyDetailMode.View
      : this.formDetailMode !== FormDetailMode.View;
  }

  public get canRemind(): boolean {
    return this.formData.status === FormStatus.Published;
  }

  public getCheckedItemsForMassAction(): FormParticipantViewModel[] {
    return this.checkedParticipants;
  }

  public onIsStandaloneChange(value: boolean): void {
    if (!this.formData.standaloneMode) {
      this.formData.standaloneMode = FormStandaloneMode.Restricted;
    }
    this.formDataChange.emit(this.formData);
  }

  public onStandaloneModeChange(value: boolean): void {
    this.formData.standaloneMode = value ? FormStandaloneMode.Public : FormStandaloneMode.Restricted;
    this.formDataChange.emit(this.formData);
  }

  public openAddParticipantDialog(): void {
    const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: AddParticipantDialogComponent });
    const configurationPopup = dialogRef.content.instance as AddParticipantDialogComponent;

    configurationPopup.participantSelectedIds = this.listParticipantComponent.gridData
      ? this.listParticipantComponent.gridData.data.map(participant => participant.userId)
      : [];

    dialogRef.result.toPromise().then((result: string[]) => {
      if (result.length > 0) {
        this.formParticipantApiService
          .assignFormParticipants({
            userIds: result,
            formId: this.formData.id,
            formOriginalObjectId: this.originalObjectId
          })
          .then(() => this.listParticipantComponent.refreshList());
      }
    });
  }

  public deleteParticipants(items: IFormParticipantViewModel[]): void {
    this.modalService.showConfirmMessage(this.translate('Are you sure you want to remove the participant?'), () => {
      const request: IDeleteFormParticipantsRequest = {
        ids: items.map(participant => participant.id),
        formId: this.formData.id
      };
      this.formParticipantApiService.deleteFormParticipants(request).then(() => {
        this.listParticipantComponent.refreshList();
        this.showNotification(`Delete participants successfully`, NotificationType.Success);
      });
    });
  }

  public remindParticipants(items: IFormParticipantViewModel[]): void {
    const usernameList: string = items.map(user => user.participant.fullName).join('\n • ');
    this.modalService.showConfirmMessage(
      this.translate(`Are you sure you want to send a reminder to: \n • ${usernameList}`),
      () => {
        const request: IRemindFormParticipantsRequest = {
          participantIds: items.map(participant => participant.id),
          formId: this.formData.id
        };
        this.formParticipantApiService.remindFormParticipants(request).then(() => {
          this.showNotification(`Remind participants successfully`, NotificationType.Success);
          this.checkedParticipants = [];
        });
      },
      null,
      null,
      640
    );
  }

  public copyStandaloneFormUrl(): void {
    ClipboardUtil.copyTextToClipboard(this.standaloneFormUrl);
    const copyStandaloneFormUrlMessage = this.moduleFacadeService.translator.translate('Copied successfully');
    this.showNotification(copyStandaloneFormUrlMessage);
  }

  public showFormAnswerReview(formParticipantViewModel: FormParticipantViewModel): void {
    if (formParticipantViewModel.status === FormParticipantStatus.Completed) {
      this.subscribe(
        this.formAnswerApiService.getByFormId(this.formData.id, null, null, null, null, formParticipantViewModel.userId),
        result => {
          const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: StandaloneFormAnswerReviewDialogComponent });
          const answerDialog = dialogRef.content.instance as StandaloneFormAnswerReviewDialogComponent;

          answerDialog.formQuestion = this.formSectionsQuestions.formQuestions;
          answerDialog.formDataType = this.formData.type;
          answerDialog.sectionsQuestion = this.createOrderedSectionsList(this.formSectionsQuestions.formQuestions);

          const formAnswers = result && orderBy(result, p => p.submitDate, 'desc');
          const formAnswer = formAnswers.find(p => p.isCompleted);

          answerDialog.formAnswer = formAnswer;
        }
      );
    }
  }

  public createOrderedSectionsList(formQuestion: FormQuestionModel[]): FormSectionViewModel[] {
    const sectionsQuestion: FormSectionViewModel[] = [];

    formQuestion.forEach(question => {
      // If question doesn't exist any section, pushing a new section item into the section list with sectionId = null
      if (!question.formSectionId) {
        const sectionVm: IFormSectionViewModel = {
          sectionId: null,
          questions: [question]
        };

        sectionsQuestion.push(sectionVm);
      } else {
        // If the question exist in a section, check this section exists
        const section = sectionsQuestion.find(s => s.sectionId === question.formSectionId);
        const sectionIdx = sectionsQuestion.findIndex(s => s.sectionId === question.formSectionId);

        // If the section exist in the section list, pushing the question into its question list
        if (section) {
          sectionsQuestion[sectionIdx].questions.push(question);
        } else {
          // If the section doesn't exist in list section, pushing a new section item into the section list with sectionId = formSectionId
          const sectionFormInfo = this.formSectionsQuestions.formSections.find(s => s.id === question.formSectionId);
          // Get title and description for each section
          const title = sectionFormInfo.mainDescription ? sectionFormInfo.mainDescription : undefined;
          const desc = sectionFormInfo.additionalDescription ? sectionFormInfo.additionalDescription : undefined;
          const sectionVm: IFormSectionViewModel = {
            sectionId: question.formSectionId,
            questions: [question],
            title: title,
            description: desc
          };

          sectionsQuestion.push(sectionVm);
        }
      }
    });

    return sectionsQuestion;
  }

  public get standaloneFormUrl(): string {
    return WebAppLinkBuilder.buildStandaloneFormUrl(this.formData.originalObjectId, this.isFromLNASurveys ? 'lnaform' : 'form');
  }

  protected onInit(): void {
    if (this.isFromLNASurveys) {
      // Always allow to access if working on LNA Surveys
      this.formData.standaloneMode = FormStandaloneMode.Public;

      this.subscribe(this.standaloneSurveyEditModeService.modeChanged, newMode => {
        this.standaloneSurveyDetailMode = newMode;
      });
    } else {
      this.subscribe(this.formEditModeService.modeChanged, newMode => {
        this.formDetailMode = newMode;
      });
    }
  }
}
