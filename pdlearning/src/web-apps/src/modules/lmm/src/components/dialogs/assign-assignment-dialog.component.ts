import {
  AssignAssignmentPaticipant,
  ClassRun,
  ClassRunRepository,
  PublicUserInfo,
  Registration,
  SearchRegistrationsType
} from '@opal20/domain-api';
import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, TranslationMessage } from '@opal20/infrastructure';
import { Component, Input, ViewEncapsulation } from '@angular/core';
import { requiredForListValidator, startEndValidator } from '@opal20/common-components';

import { Constant } from '@opal20/authentication';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { IAssignAssignmentDialogEvent } from '../../models/assign-assignment-dialog-event.model';
import { ListRegistrationGridComponentService } from '@opal20/domain-components';
import { Subscription } from 'rxjs';
import { Validators } from '@angular/forms';

@Component({
  selector: 'assign-assignment-dialog',
  templateUrl: './assign-assignment-dialog.component.html',
  encapsulation: ViewEncapsulation.None
})
export class AssignAssignmentDialogComponent extends BaseFormComponent {
  @Input() public courseId: string = '';
  @Input() public assignmentId: string = '';
  @Input() public classRun: ClassRun = new ClassRun();

  public gridData: GridDataResult;
  public selectedParticipants: string[] = [];
  public startDate: Date = null;
  public endDate: Date = null;
  public participantsInfo: PublicUserInfo[] = [];
  public classRunParticipants: Registration[] = [];

  private _loadDataSub: Subscription = new Subscription();

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    public listRegistrationGridComponentService: ListRegistrationGridComponentService,
    public classRunRepository: ClassRunRepository,
    public dialogRef: DialogRef
  ) {
    super(moduleFacadeService);
  }

  public onAssign(): void {
    this.validate().then(valid => {
      if (valid) {
        const selectedClassRunParticipants = this.classRunParticipants
          .filter(p => this.selectedParticipants.includes(p.userId))
          .map(x => {
            return new AssignAssignmentPaticipant(x);
          });

        this.dialogRef.close(<IAssignAssignmentDialogEvent>{
          registrations: selectedClassRunParticipants,
          startDate: this.startDate,
          endDate: this.endDate
        });
      }
    });
  }

  public onCancel(): void {
    this.dialogRef.close();
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();

    this._loadDataSub = this.listRegistrationGridComponentService
      .loadRegistration(
        this.courseId,
        this.classRun.id,
        SearchRegistrationsType.Participant,
        '',
        false,
        null,
        null,
        0,
        Constant.MAX_ITEMS_PER_REQUEST,
        false,
        null,
        null,
        null,
        this.assignmentId
      )
      .pipe(this.untilDestroy())
      .subscribe(registrationVm => {
        if (registrationVm.data) {
          this.classRunParticipants = registrationVm.data;
          this.participantsInfo = registrationVm.data.map(p => p.register);
        }
      });
  }

  protected onInit(): void {
    this.loadData();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      validateByGroupControlNames: [['startDate', 'endDate']],
      controls: {
        participants: {
          defaultValue: '',
          validators: [
            {
              validator: requiredForListValidator(),
              validatorType: 'required'
            }
          ]
        },

        startDate: {
          defaultValue: null,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            },

            {
              validator: startEndValidator('startDate', p => p.value, p => this.endDate, true, 'dateOnly'),
              validatorType: 'startDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start Date cannot be greater than End Date')
            },

            {
              validator: startEndValidator('startDateWithClassStartDate', p => this.classRun.startDateTime, p => p.value, true, 'dateOnly'),
              validatorType: 'startDateWithClassStartDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start date must be in the time frame of the classrun')
            },

            {
              validator: startEndValidator('startDateWithClassEndDate', p => p.value, p => this.classRun.endDateTime, true, 'dateOnly'),
              validatorType: 'startDateWithClassEndDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'Start date must be in the time frame of the classrun')
            }
          ]
        },

        endDate: {
          defaultValue: null,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            },

            {
              validator: startEndValidator('endDate', p => this.startDate, p => p.value, true, 'dateOnly'),
              validatorType: 'endDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End Date cannot be less than Start Date')
            },

            {
              validator: startEndValidator('endDateWithClassStartDate', p => this.classRun.startDateTime, p => p.value, true, 'dateOnly'),
              validatorType: 'endDateWithClassStartDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End date must be in the time frame of the classrun')
            },

            {
              validator: startEndValidator('endDateWithClassEndDate', p => p.value, p => this.classRun.endDateTime, true, 'dateOnly'),
              validatorType: 'endDateWithClassEndDate',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'End date must be in the time frame of the classrun')
            }
          ]
        }
      }
    };
  }
}
