import { BaseFormComponent, IFilter, IFormBuilderDefinition, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { ClassRun, ClassRunRepository, ClassRunStatus, RegistrationRepository, SearchClassRunType } from '@opal20/domain-api';
import { Component, Input } from '@angular/core';
import { DialogAction, OpalDialogSettings } from '@opal20/common-components';
import { Observable, Subscription } from 'rxjs';

import { ChangeRegistrationClassDialogViewModel } from '../../view-models/change-registration-class-dialog-view.model';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { Validators } from '@angular/forms';
import { map } from 'rxjs/operators';

@Component({
  selector: 'change-registration-class-dialog',
  templateUrl: './change-registration-class-dialog.component.html'
})
export class ChangeRegistrationClassDialogComponent extends BaseFormComponent {
  public static get defaultDialogSettings(): OpalDialogSettings {
    return { minWidth: '400px', width: '600px', height: '500px' };
  }

  public _courseId: string = '';
  public get courseId(): string {
    return this._courseId;
  }
  @Input() public set courseId(v: string) {
    this._courseId = v;
    if (this.initiated) {
      this.loadData();
    }
  }

  public _currentClassId: string = '';
  public get currentClassId(): string {
    return this._currentClassId;
  }
  @Input() public set currentClassId(v: string) {
    this._currentClassId = v;
    if (this.initiated) {
      this.loadData();
    }
  }

  public _registrationIds: string[] = [];
  public get registrationIds(): string[] {
    return this._registrationIds;
  }
  @Input() public set registrationIds(v: string[]) {
    this._registrationIds = v;
    if (this.initiated) {
      this.vm.registrationIds = Utils.clone(v);
    }
  }

  public vm: ChangeRegistrationClassDialogViewModel = new ChangeRegistrationClassDialogViewModel();
  public loading: boolean = false;
  public dataInitiated: boolean = false;

  private _loadDataSub: Subscription = new Subscription();

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public dialogRef: DialogRef,
    public classRunRepository: ClassRunRepository,
    public registrationRepository: RegistrationRepository
  ) {
    super(moduleFacadeService);
  }

  public fetchDestinationClassRunSelectItemFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<ClassRun[]> = (
    searchText,
    skipCount,
    maxResultCount
  ) => {
    const filterClassRun: IFilter = {
      containFilters: [
        {
          field: 'Status',
          values: [ClassRunStatus.Published],
          notContain: false
        }
      ],
      fromToFilters: []
    };

    return this.classRunRepository
      .loadClassRunsByCourseId(
        this.courseId,
        SearchClassRunType.Owner,
        searchText,
        filterClassRun,
        true,
        false,
        skipCount,
        maxResultCount,
        null,
        false
      )
      .pipe(map(_ => _.items));
  };

  public ignoreDestinationClassItemFn: (item: ClassRun) => boolean = p => p.id === this.currentClassId;

  public onCancel(): void {
    this.dialogRef.close(DialogAction.Close);
  }

  public onProceed(): void {
    this.validate().then(valid => {
      if (valid) {
        this.registrationRepository
          .massChangeClassRun({
            registrationIds: this.vm.registrationIds,
            classRunChangeId: this.vm.changeToClassId,
            comment: 'Changed By CA'
          })
          .then(p => {
            this.dialogRef.close(DialogAction.OK);
          });
      }
    });
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();

    this.loading = true;
    this._loadDataSub = this.classRunRepository
      .loadClassRunById(this.currentClassId)
      .pipe(this.untilDestroy())
      .subscribe(
        data => {
          this.vm = new ChangeRegistrationClassDialogViewModel({
            courseId: this.courseId,
            currentClass: data,
            registrationIds: this.registrationIds
          });
          this.dataInitiated = true;
          this.loading = false;
        },
        err => {
          this.loading = false;
        },
        () => {
          this.loading = false;
        }
      );
  }

  protected onInit(): void {
    this.loadData();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        changeToClassId: {
          defaultValue: null,
          validators: [{ validator: Validators.required, validatorType: 'required' }]
        }
      }
    };
  }
}
