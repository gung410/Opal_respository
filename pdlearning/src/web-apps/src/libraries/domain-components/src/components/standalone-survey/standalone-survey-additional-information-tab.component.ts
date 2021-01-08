import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, TranslationMessage } from '@opal20/infrastructure';
import { Component, ElementRef, Input, ViewChild } from '@angular/core';
import { IDueDate, StandaloneSurveyModel, SurveyStatus } from '@opal20/domain-api';
import { ScrollableMenu, futureDateValidator, ifValidator, startEndValidator, validateFutureDateType } from '@opal20/common-components';

import { StandaloneSurveyDetailMode } from '../../models/standalone-survey-detail-mode.model';
import { StandaloneSurveyEditModeService } from '../../services/standalone-survey-edit-mode.service';
import { StandaloneSurveyEditorPageService } from '../../services/standalone-survey-editor-page.service';
import { Validators } from '@angular/forms';
import { combineLatest } from 'rxjs';

enum AdditionalInformationTab {
  General = 't-general'
}

@Component({
  selector: 'standalone-survey-additional-information-tab',
  templateUrl: './standalone-survey-additional-information-tab.component.html'
})
export class StandaloneSurveyAdditionalInformationTabComponent extends BaseFormComponent {
  @ViewChild('generalTab', { static: false })
  public generalTab: ElementRef;

  public tabs: ScrollableMenu[] = [
    {
      id: AdditionalInformationTab.General,
      title: 'General',
      elementFn: () => {
        return this.generalTab;
      }
    }
  ];

  @Input('formData') public formData: StandaloneSurveyModel = new StandaloneSurveyModel();
  public mode: StandaloneSurveyDetailMode = this.editModeService.initMode;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    protected editorPageService: StandaloneSurveyEditorPageService,
    private editModeService: StandaloneSurveyEditModeService
  ) {
    super(moduleFacadeService);
  }

  public get disableMode(): boolean {
    return this.mode === StandaloneSurveyDetailMode.View;
  }

  public onArchiveDateInputChange(date: Date): void {
    this.formData.archiveDate = date;
  }

  public eventDueDateDataChange(dueDateData: IDueDate): void {
    this.formData.formRemindDueDate = dueDateData.formRemindDueDate;
    this.formData.isSendNotification = dueDateData.isSendNotification;
    this.formData.remindBeforeDays = dueDateData.remindBeforeDays;
  }

  protected onInit(): void {
    this.subscribe(this.editModeService.modeChanged, mode => {
      this.mode = mode;
    });
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        primaryApprovingOfficerId: {
          defaultValue: null,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        },
        alternativeApprovingOfficerId: {
          defaultValue: null,
          validators: null
        },
        sqRatingType: {
          defaultValue: null,
          validators: null
        },
        isSurveyTemplate: {
          defaultValue: false,
          validators: null
        },
        isAllowedDisplayPollResult: {
          defaultValue: false,
          validators: null
        },
        startDate: {
          defaultValue: null,
          validators: [
            {
              validator: futureDateValidator(),
              validatorType: validateFutureDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'The Start Date cannot be in the past.')
            },
            {
              validator: startEndValidator(
                'formAdditionalInformationStartDate',
                p => p.value,
                p => this.formData.endDate,
                true,
                'dateOnly'
              ),
              validatorType: 'formAdditionalInformationStartDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'The Start date cannot be after the End date.')
            }
          ]
        },
        endDate: {
          defaultValue: null,
          validators: [
            {
              validator: futureDateValidator(),
              validatorType: validateFutureDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'The End Date cannot be in the past.')
            },
            {
              validator: startEndValidator(
                'formAdditionalInformationEndDate',
                p => this.formData.startDate,
                p => p.value,
                true,
                'dateOnly'
              ),
              validatorType: 'formAdditionalInformationEndDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'The End date cannot be before the Start date.')
            }
          ]
        },
        archiveDate: {
          defaultValue: null,
          validators: [
            {
              validator: ifValidator(
                () => {
                  return this.formData.status === SurveyStatus.Draft;
                },
                () => startEndValidator('validArchiveDate', p => new Date(), p => p.value, false, 'dateOnly')
              ),
              validatorType: 'validArchiveDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'The Archival Date cannot be in the past')
            }
          ]
        },
        formRemindDueDate: {
          defaultValue: null,
          validators: [
            {
              validator: ifValidator(
                () => {
                  return this.formData.status === SurveyStatus.Draft;
                },
                () => startEndValidator('validFormRemindDueDate', p => new Date(), p => p.value, true, 'dateOnly')
              ),
              validatorType: 'validFormRemindDueDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'The due date can not be in the past.')
            }
          ]
        },
        isSendNotification: {
          defaultValue: null,
          validators: null
        },
        remindBeforeDays: {
          defaultValue: null,
          validators: null
        }
      }
    };
  }

  protected initData(): void {
    combineLatest([
      this.editorPageService.formAutoSaveInformer$.pipe(this.untilDestroy()),
      this.editorPageService.formData$.pipe(this.untilDestroy())
    ]).subscribe(([isAutoSaveRequest, formData]) => {
      if (!isAutoSaveRequest) {
        this.patchInitialFormValue({
          isAllowedDisplayPollResult: formData.isAllowedDisplayPollResult || null,
          sqRatingType: formData.sqRatingType || null,
          startDate: formData.startDate || null,
          endDate: formData.endDate || null
        });
      }
    });
  }
}
