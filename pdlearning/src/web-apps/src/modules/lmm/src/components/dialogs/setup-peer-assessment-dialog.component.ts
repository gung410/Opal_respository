import { AssignmentRepository, RegistrationApiService, SearchRegistrationsType } from '@opal20/domain-api';
import { BaseFormComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { IOpalSelectDefaultItem, OpalDialogService } from '@opal20/common-components';
import { Subscription, combineLatest } from 'rxjs';

import { DialogRef } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'setup-peer-assessment-dialog',
  templateUrl: './setup-peer-assessment-dialog.component.html'
})
export class SetupPeerAssessmentDialogComponent extends BaseFormComponent {
  @Input() public courseId: string = '';
  @Input() public assignmentId: string = '';
  @Input() public classrunId: string = '';

  public peerAssessorOptions: IOpalSelectDefaultItem<number>[] = [{ value: 0, label: 'N/A' }];
  public selectedPeerAssessorOption: number = 0;

  private _loadDataSub: Subscription = new Subscription();

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private assignmentRepository: AssignmentRepository,
    private registrationApiService: RegistrationApiService,
    public dialogRef: DialogRef,
    private opalDialogService: OpalDialogService
  ) {
    super(moduleFacadeService);
  }

  public onCancel(): void {
    this.dialogRef.close();
  }

  public onAssign(): void {
    if (this.selectedPeerAssessorOption != null) {
      this.subscribe(
        this.assignmentRepository.setupPeerAssessment({
          assignmentId: this.assignmentId,
          classrunId: this.classrunId,
          numberAutoAssessor: this.selectedPeerAssessorOption
        }),
        () => {
          this.opalDialogService
            .openConfirmDialog({
              confirmTitle: 'Setup Peer Assessment',
              confirmMsg: 'You have successfully issued peer assessment to the user(s).',
              hideYesBtn: true,
              noBtnText: 'Close'
            })
            .subscribe(_ => {
              this.dialogRef.close();
            });
        }
      );
    } else {
      this.dialogRef.close();
    }
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();

    const participantsObs = this.registrationApiService.searchRegistration(
      this.courseId,
      this.classrunId,
      null,
      SearchRegistrationsType.Participant,
      null,
      null,
      null,
      null,
      0,
      0,
      false
    );

    const assignmentObs = this.assignmentRepository.getAssignmentById(this.assignmentId);

    this._loadDataSub = combineLatest(participantsObs, assignmentObs)
      .pipe(this.untilDestroy())
      .subscribe(([participants, assignment]) => {
        this.selectedPeerAssessorOption = assignment.assessmentConfig != null ? assignment.assessmentConfig.numberAutoAssessor : 0;
        this.peerAssessorOptions = this.getPeerAssessorOptions(participants.totalCount);
      });
  }

  protected onInit(): void {
    this.loadData();
  }

  private getPeerAssessorOptions(total: number): IOpalSelectDefaultItem<number>[] {
    if (total < 2) {
      return [{ value: 0, label: 'N/A' }];
    } else {
      const opts: IOpalSelectDefaultItem<number>[] = [];
      if (this.selectedPeerAssessorOption === 0) {
        opts.push({ value: 0, label: 'N/A' });
      }
      for (let i = 1; i < total; i++) {
        opts.push({ value: i, label: `${i}` });
      }
      return opts;
    }
  }
}
