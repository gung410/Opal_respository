import { BaseFormComponent, DateUtils, IFormBuilderDefinition, ModuleFacadeService, TranslationMessage } from '@opal20/infrastructure';
import { Component, Input, ViewChild } from '@angular/core';
import { ICommunityEventDetailsModel, ICommunityModel } from '@opal20/domain-api';
import { equalValidator, ifValidator, requiredIfValidator, startEndValidator } from '@opal20/common-components';

import { DatePickerComponent } from '@progress/kendo-angular-dateinputs';
import { Validators } from '@angular/forms';

@Component({
  selector: 'community-event-webinar-template',
  templateUrl: './community-event-webinar-template.component.html'
})
export class CommunityEventWebinarTemplateComponent extends BaseFormComponent {
  @ViewChild('eventDate', null) public eventDate: DatePickerComponent;
  @ViewChild('timeStart', null) public timeStart: DatePickerComponent;
  @ViewChild('timeEnd', null) public timeEnd: DatePickerComponent;

  @Input() public selectedEvent: ICommunityEventDetailsModel;
  @Input() public selectedEventId: string = null;
  @Input() public communityId: string = null;
  @Input() public communitySelectable: boolean = false;
  @Input() public communities: ICommunityModel[] = [];

  public scheduled: string = 'immediately';
  public allDay: boolean = false;
  public eventTitle: string;
  public communityEventPrivacy: string;
  public acceptButtonName: string;
  public declineButtonName: string;
  public canEditEvent: boolean = false;

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public ngOnInit(): void {
    this.initForm();
  }

  public onScheduledChanged(): void {
    this.patchFormValue({
      timeStartsAt: new Date(),
      timeEndsAt: DateUtils.addMinutes(new Date(), 30)
    });
    this.formManager.validate(() => ({}), () => ({}), () => false);
  }

  public onTimesChanged(): void {
    this.formManager.validate(() => ({}), () => ({}), () => false, ['timeStartsAt', 'timeEndsAt']);
  }

  public startDisabledDates(date: Date): boolean {
    if (this.selectedEvent) {
      return DateUtils.removeTime(date) < DateUtils.removeTime(this.selectedEvent.createdAt);
    }
    return DateUtils.removeTime(date) < DateUtils.removeTime(new Date());
  }

  public patchValues(event: ICommunityEventDetailsModel): void {
    this.form.patchValue({
      title: event.title,
      eventDate: event.startAt,
      timeStartsAt: event.startAt,
      timeEndsAt: event.endAt,
      isAllDay: event.isAllDay,
      communityId: this.communityId,
      communityEventPrivacy: event.communityEventPrivacy
    });
  }

  public initEventData(selectedEvent: ICommunityEventDetailsModel): void {
    this.scheduled = 'scheduled';
    this.patchValues(selectedEvent);
  }

  public createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
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
        eventDate: {
          defaultValue: new Date(),
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Event date is required')
            },
            {
              /**
               * Do not allow select start date in the past.
               */
              validator: ifValidator(
                () => this.form && this.eventDate !== undefined,
                () => () => {
                  if (this.selectedEvent) {
                    if (DateUtils.compareOnlyDate(new Date(this.form.value.eventDate), new Date(this.selectedEvent.createdAt)) < 0) {
                      return {
                        ['dateStartsAtDisabled']: true
                      };
                    }
                  } else {
                    if (DateUtils.compareOnlyDate(new Date(this.form.value.eventDate), new Date()) < 0) {
                      return {
                        ['dateStartsAtDisabled']: true
                      };
                    }
                  }
                  return null;
                }
              ),
              validatorType: 'dateStartsAtDisabled',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'The event date cannot be in the past')
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
                () => !this.isAllDay && this.timeStart !== undefined && this.timeEnd !== undefined,
                () => startEndValidator('timeStartsAt', start => start.value, () => this.timeEnd.value, true, 'timeOnly')
              ),
              validatorType: 'timeStartsAt',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start time cannot be after end time')
            },
            {
              validator: ifValidator(
                () => !this.isAllDay && this.timeStart !== undefined && this.timeEnd !== undefined,
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
                  DateUtils.compareOnlyDate(new Date(this.form.value.eventDate), new Date()) === 0,
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
                () => this.timeStart !== undefined && this.timeEnd !== undefined,
                () => startEndValidator('timeEndsAt', () => this.timeStart.value, end => end.value, true, 'timeOnly')
              ),
              validatorType: 'timeEndsAt',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End time cannot be before start time')
            },
            {
              //  Nếu tạo webinar event thì end time không thể sớm hơn current datetime.
              validator: ifValidator(
                () => this.scheduled === 'immediately' && this.timeEnd !== undefined,
                () => startEndValidator('timeEndsAt', () => new Date(), end => end.value, false, 'timeOnly')
              ),
              validatorType: 'timeEndsAt',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End time cannot be before start time')
            },
            {
              validator: ifValidator(
                () => !this.isAllDay && this.timeStart !== undefined && this.timeEnd !== undefined,
                () => equalValidator('timeEndsAtEqual', () => this.timeStart.value, end => end.value, 'timeOnly')
              ),
              validatorType: 'timeEndsAtEqual',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End time cannot be equal start time')
            }
          ]
        },
        isAllDay: {
          defaultValue: false
        }
      }
    };
  }

  private initForm(): void {
    this.initFormData();
    setTimeout(() => {
      this.canEditEvent = DateUtils.compareDate(this.form.value.timeStartsAt, new Date()) >= 0;
      if (!this.canEditEvent && this.selectedEventId) {
        this.form.disable();
      }
    });
  }

  public get isAllDay(): boolean {
    return this.form ? this.form.get('isAllDay').value : false;
  }
}
