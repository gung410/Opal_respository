import {
  BaseFormComponent,
  DateUtils,
  IFormBuilderDefinition,
  IFormControlDefinition,
  ModuleFacadeService,
  TranslationMessage,
  Utils
} from '@opal20/infrastructure';
import {
  BlockoutDateRepository,
  ClassRun,
  ClassRunRepository,
  GetBlockoutDateDependenciesModel,
  IGetBlockoutDateDependenciesRequest,
  MetadataTagModel,
  Session,
  SessionRepository,
  TaggingRepository
} from '@opal20/domain-api';
import { BlockoutDateViewModel, RescheduleClassRunDetailViewModel } from '@opal20/domain-components';
import { Component, QueryList, ViewChildren } from '@angular/core';
import { DatePickerComponent, TimePickerComponent } from '@progress/kendo-angular-dateinputs';
import { Observable, Subscription, combineLatest, of } from 'rxjs';
import {
  classRunEndDateRescheduleWithSessionValidator,
  validateClassRunEndDateRescheduleWithSessionType
} from '../../validators/classrun-end-date-reschedule-with-session.validator';
import {
  classRunStartDateRescheduleWithSessionValidator,
  validateClassRunStartDateRescheduleWithSessionType
} from '../../validators/classrun-start-date-reschedule-with-session.validator';
import { ifValidator, startEndValidator, validateFutureDateType } from '@opal20/common-components';

import { ClassRunRescheduleInput } from '../../models/classrun-reschedule-request-input.model';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { SessionValidator } from '../../validators/session/session-validator';
import { Validators } from '@angular/forms';
import { checkDuplicatedSessionDateValidator } from '../../validators/session/check-duplicated-session-date-validator';
import { validateExistedSessionDateType } from '../../validators/session/check-existed-session-date-validator';

@Component({
  selector: 'reschedule-request-dialog',
  templateUrl: './reschedule-request-dialog.component.html'
})
export class RescheduleRequestDialogComponent extends BaseFormComponent {
  @ViewChildren('sessionDatePicker') public sessionsDatePickers!: QueryList<DatePickerComponent>;
  @ViewChildren('sessionStartTimePicker') public sessionsStartTimePickers!: QueryList<TimePickerComponent>;
  @ViewChildren('sessionEndTimePicker') public sessionsEndTimePickers!: QueryList<TimePickerComponent>;

  public title: string = '';
  public classRunReschedule: RescheduleClassRunDetailViewModel = new RescheduleClassRunDetailViewModel();
  public classRunId: string;
  public loadingData: boolean = true;
  public sessionControlsBuilderDefinition: IFormControlDefinition;
  public sessions: Session[] = [];
  public blockOutDates: BlockoutDateViewModel[] = [];
  public courseServiceSchemeIds: string[] = [];

  private _loadClassRunInfoSub: Subscription = new Subscription();
  private _blockoutDateSub: Subscription = new Subscription();
  private _metadataTagDict: Dictionary<MetadataTagModel> = {};
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public dialogRef: DialogRef,
    private classRunRepository: ClassRunRepository,
    private sessionRepository: SessionRepository,
    private taggingRepository: TaggingRepository,
    private blockOutDateRepository: BlockoutDateRepository
  ) {
    super(moduleFacadeService);
  }

  public onCancel(): void {
    this.dialogRef.close();
  }

  public onProceed(): void {
    this.validate().then(valid => {
      if (valid) {
        const rescheduleClassRunRequest: ClassRunRescheduleInput = {
          comment: this.classRunReschedule.comment,
          startDateTime: DateUtils.buildDateTime(this.classRunReschedule.startDate, this.classRunReschedule.classRunData.planningStartTime),
          endDateTime: DateUtils.buildDateTime(this.classRunReschedule.endDate, this.classRunReschedule.classRunData.planningEndTime),
          rescheduleSessions: this.classRunReschedule.sessionsData.map(session => {
            return {
              id: session.id,
              startDateTime: DateUtils.buildDateTime(session.sessionDate, session.startTime),
              endDateTime: DateUtils.buildDateTime(session.sessionDate, session.endTime)
            };
          })
        };
        this.dialogRef.close(rescheduleClassRunRequest);
      }
    });
  }

  public loadClassRunInfo(): void {
    this._loadClassRunInfoSub.unsubscribe();
    const classRunObs: Observable<ClassRun | null> =
      this.classRunId != null ? this.classRunRepository.loadClassRunById(this.classRunId) : of(null);
    const sessionObs = this.classRunId != null ? this.sessionRepository.loadSessionsByClassRunId(this.classRunId) : of(null);
    const metaDataTagObs = this.taggingRepository.loadAllMetaDataTags();
    this.loadingData = true;
    this._loadClassRunInfoSub = combineLatest(classRunObs, sessionObs, metaDataTagObs)
      .pipe(this.untilDestroy())
      .subscribe(
        ([classRun, sessions, metaDataTags]) => {
          if (this.loadingData) {
            this._metadataTagDict = Utils.toDictionary(metaDataTags, p => p.id);
            this.classRunReschedule = new RescheduleClassRunDetailViewModel(classRun, sessions.items);
            this.sessions = this.classRunReschedule.sessionsData;
            this.sessionControlsBuilderDefinition = this.prepareSessionControlsBuilderDefinition();
            this.sessions.forEach(p => this.checkBlockoutDateDependencies(p));
            this.initFormData();
          }

          this.loadingData = false;
        },
        () => {
          this.loadingData = false;
        }
      );
  }

  public onSessionDateChanged(sessionDate: Date, session: Session): void {
    if (Utils.isDifferent(session.sessionDate, sessionDate)) {
      session.sessionDate = sessionDate;
      this.checkBlockoutDateDependencies(session);
    }
  }

  protected onInit(): void {
    this.loadClassRunInfo();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    const classRunControlsBuilderDefinition: IFormControlDefinition = {
      startDate: {
        defaultValue: this.classRunReschedule.originClassRunData.startDate,
        validators: [
          {
            validator: Validators.required,
            validatorType: 'required',
            message: new TranslationMessage(this.moduleFacadeService.translator, 'Class Start Date is mandatory field')
          },
          {
            validator: ifValidator(
              p => Utils.isDifferent(p.value, this.classRunReschedule.originClassRunData.startDate),
              () =>
                startEndValidator(
                  validateFutureDateType,
                  p => new Date(),
                  p => DateUtils.buildDateTime(p.value, this.classRunReschedule.planningStartTime),
                  true,
                  'default'
                )
            ),
            validatorType: validateFutureDateType,
            message: new TranslationMessage(this.moduleFacadeService.translator, 'Class Start Date cannot be in the past')
          },
          {
            validator: startEndValidator('classRunStartDate', p => p.value, p => this.classRunReschedule.endDate, true, 'dateOnly'),
            validatorType: 'classRunStartDate',
            message: new TranslationMessage(this.moduleFacadeService.translator, 'Class Start Date cannot be greater than Class End Date')
          },
          {
            validator: classRunStartDateRescheduleWithSessionValidator(() => this.classRunReschedule),
            validatorType: validateClassRunStartDateRescheduleWithSessionType,
            message: new TranslationMessage(this.moduleFacadeService.translator, 'The date is not in the period of the class run')
          }
        ]
      },
      endDate: {
        defaultValue: this.classRunReschedule.originClassRunData.endDate,
        validators: [
          {
            validator: Validators.required,
            validatorType: 'required',
            message: new TranslationMessage(this.moduleFacadeService.translator, 'Class End Date is mandatory field')
          },
          {
            validator: startEndValidator('classRunEndDate', p => this.classRunReschedule.startDate, p => p.value, true, 'dateOnly'),
            validatorType: 'classRunEndDate',
            message: new TranslationMessage(this.moduleFacadeService.translator, 'Class End Date cannot be less than Class Start Date')
          },
          {
            validator: classRunEndDateRescheduleWithSessionValidator(() => this.classRunReschedule),
            validatorType: validateClassRunEndDateRescheduleWithSessionType,
            message: new TranslationMessage(this.moduleFacadeService.translator, 'The date is not in the period of the class run')
          }
        ]
      },
      planningStartTime: {
        defaultValue: this.classRunReschedule.originClassRunData.planningStartTime,
        validators: [
          {
            validator: startEndValidator(
              'classRunStartTime',
              p => DateUtils.buildDateTime(this.classRunReschedule.startDate, p.value),
              p => DateUtils.buildDateTime(this.classRunReschedule.endDate, this.classRunReschedule.planningEndTime),
              true,
              'default'
            ),
            validatorType: 'classRunStartTime',
            message: new TranslationMessage(
              this.moduleFacadeService.translator,
              'Class Start Date Time cannot be greater than Class End Date Time'
            )
          },
          {
            validator: ifValidator(
              p => DateUtils.compareOnlyDate(this.classRunReschedule.originClassRunData.startDate, new Date()) === 0,
              () =>
                startEndValidator(
                  'classRunPlanningStartTimeWithCurrentTime',
                  p => new Date(),
                  p => DateUtils.buildDateTime(this.classRunReschedule.startDate, p.value),
                  true,
                  'default'
                )
            ),
            validatorType: 'classRunPlanningStartTimeWithCurrentTime',
            message: new TranslationMessage(this.moduleFacadeService.translator, 'Class Start Time must be in the future')
          }
        ]
      },
      planningEndTime: {
        defaultValue: this.classRunReschedule.originClassRunData.planningEndTime,
        validators: [
          {
            validator: startEndValidator(
              'classRunEndTime',
              p => DateUtils.buildDateTime(this.classRunReschedule.startDate, this.classRunReschedule.planningStartTime),
              p => DateUtils.buildDateTime(this.classRunReschedule.endDate, p.value),
              true,
              'default'
            ),
            validatorType: 'classRunEndTime',
            message: new TranslationMessage(
              this.moduleFacadeService.translator,
              'Class End Date Time cannot be less than Class Start Date Time'
            )
          }
        ]
      },
      comment: {
        defaultValue: null,
        validators: [
          {
            validator: Validators.required,
            validatorType: 'required'
          }
        ]
      }
    };
    const validateByGroupDate = ['startDate', 'endDate', 'planningStartTime', 'planningEndTime'];

    this.sessions.forEach(session => {
      validateByGroupDate.push(session.id + 'SessionDate', session.id + 'SessionStartTime', session.id + 'SessionEndTime');
    });

    return {
      formName: 'form',
      validateByGroupControlNames: [validateByGroupDate],
      controls: this.sessionControlsBuilderDefinition
        ? { ...classRunControlsBuilderDefinition, ...this.sessionControlsBuilderDefinition }
        : classRunControlsBuilderDefinition
    };
  }

  private checkBlockoutDateDependencies(session: Session): void {
    if (!session.sessionDate) {
      return;
    }
    this._blockoutDateSub.unsubscribe();
    this._blockoutDateSub = this.blockOutDateRepository
      .loadBlockoutDateDependencies(<IGetBlockoutDateDependenciesRequest>{
        fromDate: session.sessionDate,
        serviceSchemes: this.courseServiceSchemeIds
      })
      .pipe(this.untilDestroy())
      .subscribe((blockOutDateDependencies: GetBlockoutDateDependenciesModel) => {
        this.blockOutDates = blockOutDateDependencies.matchedBlockoutDates.map(p =>
          BlockoutDateViewModel.createFromModel(p, null, {}, this._metadataTagDict)
        );
      });
  }

  private prepareSessionControlsBuilderDefinition(): IFormControlDefinition {
    const controls: IFormControlDefinition = {};
    this.sessions.forEach(session => {
      controls[session.id + 'SessionDate'] = {
        defaultValue: session.sessionDate,
        validators: [
          ...SessionValidator.getSessionDateValidators(
            () => DateUtils.buildDateTime(this.classRunReschedule.startDate, this.classRunReschedule.planningStartTime),
            () => DateUtils.buildDateTime(this.classRunReschedule.endDate, this.classRunReschedule.planningEndTime),
            () => session,
            this.moduleFacadeService
          ),
          {
            validator: checkDuplicatedSessionDateValidator(() => this.sessions.filter(p => p.id !== session.id).map(p => p.sessionDate)),
            validatorType: validateExistedSessionDateType,
            message: new TranslationMessage(this.moduleFacadeService.translator, 'There is already a session rescheduled to the same date')
          }
        ]
      };

      controls[session.id + 'SessionStartTime'] = {
        defaultValue: session.startTime,
        validators: SessionValidator.getSessionStartTimeValidators(
          () => DateUtils.buildDateTime(this.classRunReschedule.startDate, this.classRunReschedule.planningStartTime),
          () => DateUtils.buildDateTime(this.classRunReschedule.endDate, this.classRunReschedule.planningEndTime),
          () => session,
          () => this.classRunReschedule.originSessionDataDict[session.id],
          this.moduleFacadeService
        )
      };

      controls[session.id + 'SessionEndTime'] = {
        defaultValue: session.endTime,
        validators: SessionValidator.getSessionEndTimeValidators(
          () => DateUtils.buildDateTime(this.classRunReschedule.startDate, this.classRunReschedule.planningStartTime),
          () => DateUtils.buildDateTime(this.classRunReschedule.endDate, this.classRunReschedule.planningEndTime),
          () => session,
          this.moduleFacadeService
        )
      };
    });

    return controls;
  }
}
