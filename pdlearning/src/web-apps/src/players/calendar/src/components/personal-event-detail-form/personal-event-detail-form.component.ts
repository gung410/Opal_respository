import {
  AttendeeInfoOption,
  BaseUserInfo,
  EventDetailsModel,
  EventRepeatFrequency,
  EventSource,
  IBaseUserInfo,
  IEventDetailsModel,
  PersonalCalendarApiService,
  PublicUserInfo,
  SavePersonalEventRequest,
  SystemRoleEnum,
  UserInfoModel,
  UserRepository,
  UserUtils
} from '@opal20/domain-api';
import { BaseFormComponent, DateUtils, IFormBuilderDefinition, ModuleFacadeService, TranslationMessage } from '@opal20/infrastructure';
import { Component, Input, ViewChild } from '@angular/core';
import { equalValidator, ifValidator, requiredIfValidator, startEndValidator } from '@opal20/common-components';

import { DatePickerComponent } from '@progress/kendo-angular-dateinputs';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { Observable } from 'rxjs';
import { Validators } from '@angular/forms';
import { map } from 'rxjs/operators';

@Component({
  selector: 'personal-event-detail-form',
  templateUrl: './personal-event-detail-form.component.html'
})
export class PersonalEventDetailFormComponent extends BaseFormComponent {
  @ViewChild('dateStart', null) public dateStart: DatePickerComponent;
  @ViewChild('dateEnd', null) public dateEnd: DatePickerComponent;
  @ViewChild('timeStart', null) public timeStart: DatePickerComponent;
  @ViewChild('timeEnd', null) public timeEnd: DatePickerComponent;
  @ViewChild('repeatUntil', null) public repeatUntil: DatePickerComponent;

  @Input()
  public set selectedEvent(eventDetails: IEventDetailsModel) {
    this._selectedEvent = eventDetails;
    this.isEventOwner = this.selectedEvent.createdBy === this.currentUser.extId && this.selectedEvent.source === EventSource.SelfCreated;
  }
  public get selectedEvent(): IEventDetailsModel {
    return this._selectedEvent;
  }

  public isEventOwner: boolean = false;
  public defaultAvatar = 'assets/images/others/default-avatar.png';
  public today: Date = new Date();
  public allDay: boolean = false;
  public hasMoreUsersToFetch: boolean = true;
  public title: string;
  public acceptButtonName: string;
  public declineButtonName: string;
  public fetchUsersFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]>;
  public fetchUserItemsByIdsFn: (ids: string[]) => Observable<PublicUserInfo[]>;
  public currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();
  public selectedAttendees: IBaseUserInfo[] = [];
  public selectedAttendeesCount: number = 0;
  public defaultAttendeesShowingCount: number = 3;
  public attendeesMaxShowingCount: number = this.defaultAttendeesShowingCount;
  public attendeesToggleButtonName: string;
  public attendeesShowAll: boolean = false;
  private _selectedEvent: IEventDetailsModel = new EventDetailsModel();
  private timeSlotStart: Date = new Date();
  private timeSlotEnd: Date = new Date();
  private isSlotClick: boolean = false;
  private isAllDaySlot: boolean = false;
  private canAddAttenderRoles: SystemRoleEnum[] = [
    SystemRoleEnum.BranchAdministrator,
    SystemRoleEnum.DivisionAdministrator,
    SystemRoleEnum.SchoolAdministrator,
    SystemRoleEnum.SystemAdministrator,
    SystemRoleEnum.CourseAdministrator,
    SystemRoleEnum.CourseFacilitator,
    SystemRoleEnum.CourseContentCreator,
    SystemRoleEnum.UserAccount
  ];
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private personalCalendarApiService: PersonalCalendarApiService,
    private userRepository: UserRepository,
    private dialogRef: DialogRef
  ) {
    super(moduleFacadeService);
  }

  public ngOnInit(): void {
    this.title = this.selectedEvent.id ? 'Update Event' : 'New Event';
    this.acceptButtonName = this.selectedEvent.id ? 'Update' : 'Create';
    this.declineButtonName = this.isEventOwner ? 'Cancel' : 'Close';

    this.createFetchUsersFn();
    this.createFetchUsersByIdsFn();
    this.initForm();
  }
  /**
   * Save temporary time slot times when double click on scheduler time slot
   * @param start start time
   * @param end end time
   */
  public setTimeSlot(start: Date, end: Date): void {
    this.timeSlotStart = new Date(start);
    this.timeSlotEnd = new Date(end);
    this.isSlotClick = true;
  }
  /**
   * Save temporary time slot all day option when double click on scheduler time slot (all day)
   */
  public setAllDaySlot(): void {
    this.isAllDaySlot = true;
  }

  public get isDailyRepeat(): boolean {
    return this.form ? this.form.get('dailyRepeat').value : false;
  }

  public get isAllDay(): boolean {
    return this.form ? this.form.get('isAllDay').value : false;
  }

  public onDailyRepeatChanged(checked: boolean): void {
    if (checked) {
      this.form.get('repeatUntil').updateValueAndValidity();
      this.patchFormValue({
        repeatUntil: DateUtils.addDays(this.dateStart.value, 1)
      });
    } else {
      this.form.get('dateEndsAt').updateValueAndValidity();
      this.patchFormValue({
        dateEndsAt: this.dateStart.value
      });
    }
  }

  public get isAbleToAddAttenders(): boolean {
    return this.currentUser.hasRole(...this.canAddAttenderRoles);
  }

  public toggleShowingAttendees(): void {
    this.attendeesShowAll = !this.attendeesShowAll;
    this.updateAttendeesMaxShowingCount();
    this.updateAttendessToggleButtonText();
  }

  public onAttendeeChanged(): void {
    const currentForm = this.form;
    setTimeout(() => {
      const userIds: string[] = currentForm.get('userIds').value;
      if (userIds && userIds.length) {
        this.userRepository
          .loadPublicUserInfoList(
            {
              userIds: userIds
            },
            false
          )
          .subscribe(userList => {
            this.selectedAttendees = userList.slice().reverse();
            this.selectedAttendeesCount = this.selectedAttendees.length;
            this.updateAttendessToggleButtonText();
            this.updateAttendeesMaxShowingCount();
          });
      } else {
        this.selectedAttendees = [];
        this.selectedAttendeesCount = 0;
        this.updateAttendessToggleButtonText();
        this.updateAttendeesMaxShowingCount();
      }
    }, 0);
  }

  public onClearSelectedAttendee(attendee: IBaseUserInfo): void {
    if (this.isEventOwner) {
      this.selectedAttendees.splice(this.selectedAttendees.findIndex(a => a.userCxId === attendee.userCxId), 1);
      this.patchFormValue({
        userIds: this.selectedAttendees.map(p => p.id)
      });
      this.selectedAttendeesCount = this.selectedAttendees.length;
    }
  }

  public onCancel(): void {
    this.dialogRef.close(false);
  }

  public startDisabledDates(date: Date): boolean {
    if (this.selectedEvent.id && this.isEventOwner) {
      return DateUtils.removeTime(date) < DateUtils.removeTime(this.selectedEvent.createdAt);
    }
    return DateUtils.removeTime(date) < DateUtils.removeTime(new Date());
  }

  public endDisabledDates(date: Date): boolean {
    return this.isDailyRepeat ? false : DateUtils.removeTime(date) < DateUtils.removeTime(this.dateStart.value);
  }

  public repeatUntilDisabledDates(date: Date): boolean {
    return DateUtils.removeTime(date) <= DateUtils.removeTime(this.dateStart.value);
  }

  public onDatesChanged(): void {
    if (this.isDailyRepeat) {
      if (DateUtils.compareOnlyDate(new Date(this.repeatUntil.value), new Date(this.dateStart.value)) <= 0) {
        this.patchFormValue({
          repeatUntil: DateUtils.addDays(new Date(this.dateStart.value), 1)
        });
      }
    }
  }

  public onTimesChanged(): void {
    this.formManager.validate(() => ({}), () => ({}), () => false, ['timeStartsAt', 'timeEndsAt']);
  }

  public onSave(): void {
    const startAt: Date = this.form.get('dateStartsAt').value;
    const timeStartsAt: Date = this.form.get('timeStartsAt').value;
    if (timeStartsAt) {
      startAt.setHours(timeStartsAt.getHours(), timeStartsAt.getMinutes());
    }

    const endAt: Date = this.form.get('dateEndsAt').value;
    const timeEndsAt: Date = this.form.get('timeEndsAt').value;
    if (timeEndsAt) {
      endAt.setHours(timeEndsAt.getHours(), timeEndsAt.getMinutes());
    }

    const request: SavePersonalEventRequest = {
      id: this.selectedEvent.id,
      title: this.form.get('title').value,
      startAt: startAt,
      endAt: endAt,
      isAllDay: this.form.get('isAllDay').value,
      description: this.form.get('description').value,
      attendeeIds: this.form.get('userIds').value,
      repeatFrequency: EventRepeatFrequency.None,
      repeatUntil: null
    };

    if (this.isDailyRepeat) {
      request.repeatFrequency = EventRepeatFrequency.Daily;
      request.endAt.setDate(request.startAt.getDate());
      request.repeatUntil = this.repeatUntil.value;
    }

    if (!this.selectedEvent.id) {
      this.subscribe(this.personalCalendarApiService.createEvent(request), data => this.dialogRef.close(true));
    } else {
      this.subscribe(this.personalCalendarApiService.updateEvent(request), () => this.dialogRef.close(true));
    }
  }

  public onDelete(): void {
    this.modalService.showConfirmMessage(
      new TranslationMessage(this.moduleFacadeService.globalTranslator, 'Are you sure you want to delete the event?'),
      () => {
        this.personalCalendarApiService
          .deleteEvent(this.selectedEvent.id)
          .toPromise()
          .then(() => {
            this.dialogRef.close(true);
          });
      }
    );
  }

  public patchValues(event: IEventDetailsModel): void {
    this.form.patchValue({
      title: event.title,
      dateStartsAt: event.startAt,
      dateEndsAt: event.endAt,
      timeStartsAt: event.startAt,
      timeEndsAt: event.endAt,
      isAllDay: event.isAllDay,
      description: event.description,
      userIds: event.attendeeIds,
      dailyRepeat: event.repeatFrequency === EventRepeatFrequency.Daily,
      repeatUntil: event.repeatUntil ? event.repeatUntil : new Date()
    });
  }

  public createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      validateByGroupControlNames: [['dateStartsAt', 'dateEndsAt', 'timeStartsAt', 'timeEndsAt', 'isAllDay', 'dailyRepeat', 'repeatUntil']],
      controls: {
        title: {
          defaultValue: '',
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Title is required')
            },
            {
              validator: Validators.maxLength(256),
              validatorType: 'TitleLength',
              message: new TranslationMessage(
                this.moduleFacadeService.translator,
                'The event title cannot be more than 255 words. Please adjust the event name'
              )
            }
          ]
        },
        dateStartsAt: {
          defaultValue: new Date(),
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start date is required')
            },
            {
              /**
               * Do not allow select start date in the past.
               */
              validator: ifValidator(
                () => !this.selectedEvent.id && this.form && this.dateStart !== undefined,
                () => () => {
                  if (DateUtils.compareOnlyDate(new Date(this.form.value.dateStartsAt), new Date()) < 0) {
                    return {
                      ['dateStartsAtDisabled']: true
                    };
                  }
                  return null;
                }
              ),
              validatorType: 'dateStartsAtDisabled',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'The Start date cannot be in the past')
            },
            {
              /**
               * Do not allow select start date in the past (update).
               */
              validator: ifValidator(
                () => this.selectedEvent.id && this.form && this.dateStart !== undefined,
                () => () => {
                  if (DateUtils.compareOnlyDate(new Date(this.form.value.dateStartsAt), this.selectedEvent.startAt) < 0) {
                    return {
                      ['dateStartsAtDisabledUpdate']: true
                    };
                  }
                  return null;
                }
              ),
              validatorType: 'dateStartsAtDisabledUpdate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'New start date cannot be before the current start date')
            },
            {
              validator: ifValidator(
                () => !this.isDailyRepeat && this.dateStart !== undefined && this.dateEnd !== undefined,
                () => startEndValidator('dateStartsAt', start => start.value, () => this.dateEnd.value, true, 'dateOnly')
              ),
              validatorType: 'dateStartsAt',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start date cannot be after end date')
            },
            {
              validator: ifValidator(
                () => this.isDailyRepeat && this.dateStart !== undefined && this.repeatUntil !== undefined,
                () => startEndValidator('dateStartsAt', start => start.value, () => this.repeatUntil.value, true, 'dateOnly')
              ),
              validatorType: 'dateStartsAtUntil',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start date cannot be after Repeat Until date')
            },
            {
              validator: ifValidator(
                () => this.isDailyRepeat && this.repeatUntil !== undefined && this.dateStart !== undefined,
                () => equalValidator('dateStartEqual', start => start.value, () => this.repeatUntil.value, 'dateOnly')
              ),
              validatorType: 'dateStartEqual',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start date cannot be equal Repeat Until date')
            }
          ]
        },
        dateEndsAt: {
          defaultValue: new Date(),
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End date is required')
            },
            {
              validator: ifValidator(
                () => !this.isDailyRepeat && this.dateStart !== undefined && this.dateEnd !== undefined,
                () => startEndValidator('dateEndsAt', () => this.dateStart.value, end => end.value, true, 'dateOnly')
              ),
              validatorType: 'dateEndsAt',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End date cannot be before start date')
            }
          ]
        },
        repeatUntil: {
          defaultValue: new Date(),
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Repeat Until date is required')
            },
            {
              validator: ifValidator(
                () => this.isDailyRepeat && this.dateStart !== undefined,
                () => startEndValidator('repeatUntil', () => this.dateStart.value, end => end.value, true, 'dateOnly')
              ),
              validatorType: 'repeatUntil',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Repeat Until date cannot be earlier start date')
            },
            {
              validator: ifValidator(
                () => this.isDailyRepeat && this.dateStart !== undefined && this.repeatUntil !== undefined,
                () => equalValidator('repeatUntilEqual', () => this.dateStart.value, repeatUntil => repeatUntil.value, 'dateOnly')
              ),
              validatorType: 'repeatUntilEqual',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Repeat Until date cannot be equal start date')
            }
          ]
        },
        timeStartsAt: {
          defaultValue: DateUtils.addMinutes(new Date(), 1),
          validators: [
            {
              validator: requiredIfValidator(() => !this.isAllDay),
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start time is required')
            },
            {
              validator: ifValidator(
                () =>
                  !this.isAllDay &&
                  this.timeStart !== undefined &&
                  this.timeEnd !== undefined &&
                  (this.isDailyRepeat ||
                    DateUtils.compareOnlyDate(new Date(this.form.value.dateStartsAt), new Date(this.form.value.dateEndsAt)) === 0),
                () => startEndValidator('timeStartsAt', start => start.value, () => this.timeEnd.value, true, 'timeOnly')
              ),
              validatorType: 'timeStartsAt',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start time cannot be after end time')
            },
            {
              validator: ifValidator(
                () =>
                  !this.isAllDay &&
                  this.timeStart !== undefined &&
                  this.timeEnd !== undefined &&
                  (this.isDailyRepeat ||
                    DateUtils.compareOnlyDate(new Date(this.form.value.dateStartsAt), new Date(this.form.value.dateEndsAt)) === 0),
                () => equalValidator('timeStartsAtEqual', start => start.value, () => this.timeEnd.value, 'timeOnly')
              ),
              validatorType: 'timeStartsAtEqual',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start time cannot be equal end time')
            },
            {
              validator: ifValidator(
                () =>
                  this.form &&
                  !this.form.value.isAllDay &&
                  this.timeStart !== undefined &&
                  this.timeEnd !== undefined &&
                  !this.isDailyRepeat &&
                  DateUtils.compareOnlyDate(new Date(this.form.value.dateStartsAt), new Date()) === 0,
                () => () => {
                  if (DateUtils.compareTimeOfDate(this.timeStart.value, DateUtils.addMinutes(new Date(), 1)) < 0) {
                    return {
                      ['timeStartsAtPast']: true
                    };
                  }
                  return null;
                }
              ),
              validatorType: 'timeStartsAtPast',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start time cannot be in the past')
            }
          ]
        },
        timeEndsAt: {
          defaultValue: DateUtils.addMinutes(new Date(), 30),
          validators: [
            {
              validator: requiredIfValidator(() => !this.isAllDay),
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End time is required')
            },
            {
              validator: ifValidator(
                () =>
                  !this.isAllDay &&
                  this.timeStart !== undefined &&
                  this.timeEnd !== undefined &&
                  (this.isDailyRepeat ||
                    DateUtils.compareOnlyDate(new Date(this.form.value.dateStartsAt), new Date(this.form.value.dateEndsAt)) === 0),
                () => startEndValidator('timeEndsAt', () => this.timeStart.value, end => end.value, true, 'timeOnly')
              ),
              validatorType: 'timeEndsAt',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End time cannot be before start time')
            },
            {
              validator: ifValidator(
                () =>
                  !this.isAllDay &&
                  this.timeStart !== undefined &&
                  this.timeEnd !== undefined &&
                  (this.isDailyRepeat ||
                    DateUtils.compareOnlyDate(new Date(this.form.value.dateStartsAt), new Date(this.form.value.dateEndsAt)) === 0),
                () => equalValidator('timeEndsAtEqual', () => this.timeStart.value, end => end.value, 'timeOnly')
              ),
              validatorType: 'timeEndsAtEqual',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End time cannot be equal start time')
            }
          ]
        },
        isAllDay: {
          defaultValue: false
        },
        dailyRepeat: {
          defaultValue: false
        },
        description: {
          defaultValue: ''
        },
        userIds: {
          defaultValue: []
        }
      }
    };
  }

  private updateAttendeesMaxShowingCount(): void {
    this.attendeesMaxShowingCount = this.attendeesShowAll === true ? this.selectedAttendees.length : this.defaultAttendeesShowingCount;
  }

  private updateAttendessToggleButtonText(): void {
    this.attendeesToggleButtonName = this.attendeesShowAll
      ? this.translate('show less')
      : this.translate('##count## more', { count: this.selectedAttendees.length - this.defaultAttendeesShowingCount });
  }

  private createFetchUsersFn(): void {
    const _fetchUsersFn = UserUtils.createFetchUsersByDeptWithCheckMoreDataFn([this.currentUser.departmentId], this.userRepository, false);
    this.fetchUsersFn = (searchText: string, skipCount: number, maxResultCount: number) => {
      return _fetchUsersFn(searchText, skipCount, maxResultCount).pipe(
        map(_ => {
          this.hasMoreUsersToFetch = _.hasMoreData;
          return _.items.map(u => new BaseUserInfo(u)).filter(u => u.id !== this.currentUser.id);
        })
      );
    };
  }

  private initForm(): void {
    this.initFormData();
    this.form.patchValue({
      userIds: [this.currentUser.extId]
    });

    if (this.selectedEvent.id) {
      this.patchValues(this.selectedEvent);
      this.formManager.validate(() => ({}), () => ({}), () => false);

      if (!this.isEventOwner) {
        this.form.disable();
        this.title = 'Event Detail';
      }
    } else if (this.isSlotClick) {
      this.form.patchValue({
        dateStartsAt: new Date(this.timeSlotStart),
        dateEndsAt: new Date(this.timeSlotEnd),
        timeStartsAt: new Date(this.timeSlotStart),
        timeEndsAt: new Date(this.timeSlotEnd)
      });
    }
    if (this.isAllDaySlot) {
      this.form.patchValue({
        isAllDay: true
      });
    }

    this.onAttendeeChanged();
  }

  private createFetchUsersByIdsFn(): void {
    this.fetchUserItemsByIdsFn = ids => {
      return this.userRepository
        .loadPublicUserInfoList(
          {
            userIds: ids
          },
          false
        )
        .pipe(map(users => users.map(u => new AttendeeInfoOption(u, u.id === this.currentUser.id))));
    };
  }
}
