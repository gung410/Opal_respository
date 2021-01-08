import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, TranslationMessage } from '@opal20/infrastructure';
import { Component, ElementRef, Input, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormDetailMode, FormEditorPageService } from '@opal20/domain-components';
import { FormModel, FormStatus, FormType, IDueDate, ResourceModel } from '@opal20/domain-api';
import { ScrollableMenu, futureDateValidator, ifValidator, startEndValidator, validateFutureDateType } from '@opal20/common-components';

import { FormEditModeService } from '../services/form-edit-mode.service';
import { Validators } from '@angular/forms';
import { combineLatest } from 'rxjs';

enum AdditionalInformationTab {
  General = 't-general',
  Metadata = 't-metadata'
}

@Component({
  selector: 'form-additional-information-tab',
  templateUrl: './form-additional-information-tab.component.html',
  encapsulation: ViewEncapsulation.None
})
export class FormAdditionalInformationTabComponent extends BaseFormComponent {
  @ViewChild('generalTab', { static: false })
  public generalTab: ElementRef;

  @ViewChild('metadataTab', { static: false })
  public metadataTab: ElementRef;

  public tabs: ScrollableMenu[] = [
    {
      id: AdditionalInformationTab.General,
      title: 'General',
      elementFn: () => {
        return this.generalTab;
      }
    },
    {
      id: AdditionalInformationTab.Metadata,
      title: 'Metadata',
      elementFn: () => {
        return this.metadataTab;
      }
    }
  ];

  @Input('formData') public formData: FormModel = new FormModel();
  @Input()
  public resource: ResourceModel = new ResourceModel();
  public mode: FormDetailMode = this.formEditModeService.initMode;
  public FORM_TYPE = FormType;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    protected formEditorPageService: FormEditorPageService,
    private formEditModeService: FormEditModeService
  ) {
    super(moduleFacadeService);
  }

  public get disableMode(): boolean {
    return this.mode === FormDetailMode.View;
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
    this.subscribe(this.formEditModeService.modeChanged, mode => {
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
        surveyType: {
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
                  return this.formData.status === FormStatus.Draft;
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
                  return this.formData.status === FormStatus.Draft;
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
        },
        searchTags: {
          defaultValue: []
        }
      }
    };
  }

  protected initData(): void {
    combineLatest([
      this.formEditorPageService.formAutoSaveInformer$.pipe(this.untilDestroy()),
      this.formEditorPageService.formData$.pipe(this.untilDestroy())
    ]).subscribe(([isAutoSaveRequest, formData]) => {
      if (!isAutoSaveRequest) {
        this.patchInitialFormValue({
          primaryApprovingOfficerId: formData.primaryApprovingOfficerId || null,
          alternativeApprovingOfficerId: formData.alternativeApprovingOfficerId || null,
          surveyType: formData.surveyType || null,
          isAllowedDisplayPollResult: formData.isAllowedDisplayPollResult || null,
          isSurveyTemplate: formData.isSurveyTemplate || null,
          sqRatingType: formData.sqRatingType || null,
          startDate: formData.startDate || null,
          endDate: formData.endDate || null
        });
      }
    });
  }
}
