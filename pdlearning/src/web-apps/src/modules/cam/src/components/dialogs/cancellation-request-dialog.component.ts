import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService } from '@opal20/infrastructure';
import { ClassRun, ClassRunRepository, Course, CourseRepository, UserRepository } from '@opal20/domain-api';
import { Observable, Subscription, combineLatest, of } from 'rxjs';

import { ClassRunDetailViewModel } from '@opal20/domain-components';
import { Component } from '@angular/core';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { Validators } from '@angular/forms';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'cancellation-request-dialog',
  templateUrl: './cancellation-request-dialog.component.html'
})
export class CancellationRequestDialogComponent extends BaseFormComponent {
  public title: string = '';
  public classRunId: string = '';
  public loadingData: boolean = true;
  public classRun: ClassRunDetailViewModel = new ClassRunDetailViewModel();
  private _loadClassRunInfoSub: Subscription = new Subscription();
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public dialogRef: DialogRef,
    private classRunRepository: ClassRunRepository,
    private courseRepository: CourseRepository,
    private userRepository: UserRepository
  ) {
    super(moduleFacadeService);
  }

  public onCancel(): void {
    this.dialogRef.close();
  }

  public onProceed(): void {
    this.validate().then(valid => {
      if (valid) {
        this.dialogRef.close({ comment: this.classRun.comment });
      }
    });
  }

  public loadClassRunInfo(): void {
    this._loadClassRunInfoSub.unsubscribe();
    const classRunObs: Observable<ClassRun | null> =
      this.classRunId != null ? this.classRunRepository.loadClassRunById(this.classRunId) : of(null);
    this.loadingData = true;
    this._loadClassRunInfoSub = combineLatest(classRunObs)
      .pipe(
        this.untilDestroy(),
        switchMap(([classRun]) => {
          const courseObs: Observable<Course> = classRun != null ? this.courseRepository.loadCourse(classRun.courseId) : of(null);
          return combineLatest(of(classRun), courseObs);
        }),
        switchMap(([classRun, course]) => {
          return ClassRunDetailViewModel.create(
            ids =>
              this.userRepository.loadUserInfoList(
                {
                  userIds: ids,
                  pageIndex: 0,
                  pageSize: 0
                },
                null,
                ['All']
              ),
            course,
            classRun
          );
        })
      )
      .subscribe(
        classRunVm => {
          this.classRun = classRunVm;

          this.loadingData = false;
        },
        () => {
          this.loadingData = false;
        }
      );
  }

  protected onInit(): void {
    this.loadClassRunInfo();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        comment: {
          defaultValue: null,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        }
      }
    };
  }
}
