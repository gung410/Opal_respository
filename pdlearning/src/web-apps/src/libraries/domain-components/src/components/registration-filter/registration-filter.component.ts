import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { DepartmentInfoModel, LearningCatalogRepository, OrganizationRepository, TaggingRepository } from '@opal20/domain-api';
import { Observable, Subscription, combineLatest } from 'rxjs';

import { CommonFilterAction } from './../../models/common-filter-action.model';
import { Component } from '@angular/core';
import { PopupRef } from '@progress/kendo-angular-popup';
import { RegistrationFilterModel } from './../../models/registration-filter.model';
import { RegistrationFilterViewModel } from './../../view-models/registration-filter-view.model';
import { switchMap } from 'rxjs/operators';

interface IRegistrationFilterInput {
  data: RegistrationFilterModel;
  settings: unknown;
}

@Component({
  selector: 'registration-filter',
  templateUrl: './registration-filter.component.html'
})
export class RegistrationFilterComponent extends BaseFormComponent {
  public popupRef: PopupRef;
  public registrationFilterVm: RegistrationFilterViewModel = new RegistrationFilterViewModel([], [], []);
  public fetchDepartmentFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> = null;
  public showMore: boolean = false;
  public actionFn: CommonFilterAction;

  public get inputs(): IRegistrationFilterInput {
    return this._inputs;
  }

  public set inputs(v: IRegistrationFilterInput) {
    if (v != null) {
      this._inputs = v;
    }
  }

  public get hasDataChange(): boolean {
    return this.registrationFilterVm.dataHasChanged();
  }

  private _loadDataSub: Subscription = new Subscription();
  private _inputs: IRegistrationFilterInput = {
    data: new RegistrationFilterModel(),
    settings: null
  };

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private taggingRepository: TaggingRepository,
    private organizationRepository: OrganizationRepository,
    private learningCatalogRepository: LearningCatalogRepository
  ) {
    super(moduleFacadeService);
    this.fetchDepartmentFn = this._createFetchDepartmentFn();
  }

  public onChangeShow(): void {
    this.showMore = !this.showMore;
  }

  public onApply(): void {
    this.validate().then(valid => {
      if (valid) {
        this.popupRef.close();
        this.actionFn.applyFn(this.registrationFilterVm.data);
      }
    });
  }

  public onClear(): void {
    Utils.setValueAllProperties(this.registrationFilterVm.data, undefined);
  }

  protected onInit(): void {
    this.loadData();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'registration-filter',
      controls: {
        department: {
          defaultValue: null,
          validators: null
        },
        serviceScheme: {
          defaultValue: null,
          validators: null
        },
        designation: {
          defaultValue: null,
          validators: null
        },
        developmentalRole: {
          defaultValue: null,
          validators: null
        },
        teachingLevel: {
          defaultValue: null,
          validators: null
        },
        teachingCourseOfStudy: {
          defaultValue: null,
          validators: null
        },
        teachingSubject: {
          defaultValue: null,
          validators: null
        },
        learningFrameworks: {
          defaultValue: null,
          validators: null
        }
      }
    };
  }

  private loadData(): void {
    this._loadDataSub.unsubscribe();
    const taggingObs = this.taggingRepository.loadAllMetaDataTags();
    const designationObs = this.learningCatalogRepository.loadUserDesignationList();
    this._loadDataSub = combineLatest(taggingObs, designationObs)
      .pipe(
        switchMap(([metadatas, designations]) =>
          RegistrationFilterViewModel.create(
            ids => this.organizationRepository.loadOrganizationalUnitsByIds(ids),
            metadatas,
            designations,
            this.inputs.data ? this.inputs.data : new RegistrationFilterModel()
          )
        ),
        this.untilDestroy()
      )
      .subscribe(vm => {
        this.registrationFilterVm = vm;
      });
  }

  private _createFetchDepartmentFn(): (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> {
    return (searchText, skipCount, maxResultCount) =>
      this.organizationRepository.loadOrganizationalUnits(
        searchText,
        null,
        null,
        true,
        null,
        null,
        true,
        maxResultCount === 0 ? 1 : skipCount / maxResultCount + 1,
        maxResultCount,
        false
      );
  }
}
