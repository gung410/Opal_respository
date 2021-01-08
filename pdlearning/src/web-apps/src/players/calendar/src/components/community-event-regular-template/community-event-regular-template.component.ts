import { BaseFormComponent, DateUtils, IFormBuilderDefinition, ModuleFacadeService, TranslationMessage } from '@opal20/infrastructure';
import { Component, Input, ViewChild } from '@angular/core';
import { EventRepeatFrequency, ICommunityEventDetailsModel, ICommunityModel } from '@opal20/domain-api';
import { equalValidator, ifValidator, requiredIfValidator, startEndValidator } from '@opal20/common-components';

import { DatePickerComponent } from '@progress/kendo-angular-dateinputs';
import { Validators } from '@angular/forms';

@Component({
  selector: 'community-event-regular-template',
  templateUrl: './community-event-regular-template.component.html'
})
export class CommunityEventRegularTemplateComponent extends BaseFormComponent {
  @ViewChild('dateStart', null) public dateStart: DatePickerComponent;
  @ViewChild('dateEnd', null) public dateEnd: DatePickerComponent;
  @ViewChild('repeatUntil', null) public repeatUntil: DatePickerComponent;
  @ViewChild('timeStart', null) public timeStart: DatePickerComponent;
  @ViewChild('timeEnd', null) public timeEnd: DatePickerComponent;

  @Input() public selectedEvent: ICommunityEventDetailsModel;
  @Input() public selectedEventId: string = null;
  @Input() public communityId: string = null;
  @Input() public communitySelectable: boolean = false;
  @Input() public communities: ICommunityModel[] = [];

  public allDay: boolean = false;
  public eventTitle: string;
  public acceptButtonName: string;
  public declineButtonName: string;
  public isEventOwner: boolean = false;

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public ngOnInit(): void {
    this.initForm();
  }

  public get isDailyRepeat(): boolean {
    return this.form ? this.form.get('dailyRepeat').value : false;
  }

  public startDisabledDates(date: Date): boolean {
    if (this.selectedEvent) {
      return DateUtils.removeTime(date) < DateUtils.removeTime(this.selectedEvent.createdAt);
    }
    return DateUtils.removeTime(date) < DateUtils.removeTime(new Date());
  }

  public endDisabledDates(date: Date): boolean {
    if (this.isDailyRepeat) {
      return false;
    }
    return DateUtils.removeTime(date) < DateUtils.removeTime(this.dateStart.value);
  }

  public repeatUntilDisabledDates(date: Date): boolean {
    if (!this.isDailyRepeat) {
      return false;
    }
    return DateUtils.removeTime(date) <= DateUtils.removeTime(this.dateStart.value);
  }

  public onDatesChanged(): void {
    this.formManager.validate(() => ({}), () => ({}), () => false, ['dateStartsAt', 'dateEndsAt']);
  }

  public onTimesChanged(): void {
    this.formManager.validate(() => ({}), () => ({}), () => false, ['timeStartsAt', 'timeEndsAt']);
  }

  public onIsAllDayChanged(checked: boolean): void {
    if (checked) {
      this.timeStart.value = DateUtils.setTimeToStartOfTheDay(this.timeEnd.value);
      this.timeEnd.value = DateUtils.setTimeToEndInDay(this.timeEnd.value);
    } else {
      this.patchFormValue({
        timeStartsAt: new Date(),
        timeEndsAt: DateUtils.addMinutes(new Date(), 30)
      });
    }
    this.formManager.validate(() => ({}), () => ({}), () => false);
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
    this.formManager.validate(() => ({}), () => ({}), () => false, ['dateStartsAt']);
  }

  public patchValues(event: ICommunityEventDetailsModel): void {
    this.form.patchValue({
      title: event.title,
      dateStartsAt: event.startAt,
      dateEndsAt: event.endAt,
      timeStartsAt: event.startAt,
      timeEndsAt: event.endAt,
      isAllDay: event.isAllDay,
      description: event.description,
      communityId: this.communityId,
      dailyRepeat: event.repeatFrequency === EventRepeatFrequency.Daily,
      repeatUntil: event.repeatUntil ? event.repeatUntil : new Date()
    });
  }

  public initEventData(selectedEvent: ICommunityEventDetailsModel): void {
    this.patchValues(selectedEvent);
    this.formManager.validate(() => ({}), () => ({}), () => false);
  }

  public createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      validateByGroupControlNames: [['dateStartsAt', 'dateEndsAt', 'timeStartsAt', 'timeEndsAt', 'isAllDay', 'dailyRepeat', 'repeatUntil']],
      controls: {
        communityId: {
          defaultValue: this.selectedEvent ? this.selectedEvent.communityId : this.communityId,
          validators: [
            {
              validator: requiredIfValidator(() => this.communitySelectable),
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Community is required')
            }
          ]
        },
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
                () => this.dateStart !== undefined && (this.dateEnd !== undefined || this.repeatUntil !== undefined),
                () => () => {
                  if (this.selectedEvent) {
                    if (DateUtils.compareOnlyDate(new Date(this.form.value.dateStartsAt), new Date(this.selectedEvent.createdAt)) < 0) {
                      return {
                        ['dateStartsAtDisabled']: true
                      };
                    }
                  } else {
                    if (DateUtils.compareOnlyDate(new Date(this.form.value.dateStartsAt), new Date()) < 0) {
                      return {
                        ['dateStartsAtDisabled']: true
                      };
                    }
                  }
                  return null;
                }
              ),
              validatorType: 'dateStartsAtDisabled',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'The Start date cannot be in the past')
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
                () => this.isDailyRepeat && this.repeatUntil !== undefined && this.dateStart !== undefined,
                () => equalValidator('dateStartEqual', start => start.value, () => this.repeatUntil.value, 'dateOnly')
              ),
              validatorType: 'dateStartEqual',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start date cannot be equal repeat until date')
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
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Repeat until date is required')
            },
            {
              validator: ifValidator(
                () => this.isDailyRepeat && this.dateStart !== undefined && this.repeatUntil !== undefined,
                () => startEndValidator('repeatUntil', () => this.dateStart.value, end => end.value, true, 'dateOnly')
              ),
              validatorType: 'repeatUntil',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End date cannot be before start date')
            },
            {
              validator: ifValidator(
                () => this.isDailyRepeat && this.dateStart !== undefined && this.repeatUntil !== undefined,
                () => equalValidator('repeatUntilEqual', () => this.dateStart.value, repeatUntil => repeatUntil.value, 'dateOnly')
              ),
              validatorType: 'repeatUntilEqual',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Repeat until day cannot be equal start date')
            }
          ]
        },
        timeStartsAt: {
          defaultValue: DateUtils.addMinutes(new Date(), 1),
          validators: [
            {
              validator: requiredIfValidator(() => this.form && !this.form.value.isAllDay),
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start time is required')
            },
            {
              validator: ifValidator(
                () =>
                  this.form &&
                  !this.form.value.isAllDay &&
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
                  this.form &&
                  !this.form.value.isAllDay &&
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
                  !this.selectedEventId &&
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
              validator: requiredIfValidator(() => this.form && !this.form.value.isAllDay),
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End time is required')
            },
            {
              validator: ifValidator(
                () =>
                  this.form &&
                  !this.form.value.isAllDay &&
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
                  this.form &&
                  !this.form.value.isAllDay &&
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
        }
      }
    };
  }

  private initForm(): void {
    this.initFormData();
  }
}
