import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, TranslationMessage, Utils } from '@opal20/infrastructure';
import { DepartmentInfoModel, OrganizationRepository, TaggingRepository } from '@opal20/domain-api';
import { Observable, Subscription, combineLatest } from 'rxjs';
import { mustBeInThePastValidator, startEndValidator, validateMustBePastDateType } from '@opal20/common-components';

import { CommonFilterAction } from './../../models/common-filter-action.model';
import { Component } from '@angular/core';
import { CourseFilterModel } from './../../models/course-filter.model';
import { CourseFilterViewModel } from './../../view-models/course-filter-view.model';
import { ICourseFilterSetting } from './../../models/course-filter-setting.model';
import { PopupRef } from '@progress/kendo-angular-popup';
import { switchMap } from 'rxjs/operators';

interface ICourseFilterInput {
  data: CourseFilterModel;
  settings: ICourseFilterSetting;
}

@Component({
  selector: 'course-filter',
  templateUrl: './course-filter.component.html'
})
export class CourseFilterComponent extends BaseFormComponent {
  public popupRef: PopupRef;
  public courseFilterVm: CourseFilterViewModel = new CourseFilterViewModel([], []);
  public fetchDepartmentFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> = null;
  public showMore: boolean = false;
  public actionFn: CommonFilterAction;

  public get inputs(): ICourseFilterInput {
    return this._inputs;
  }

  public set inputs(v: ICourseFilterInput) {
    if (v != null) {
      this._inputs = v;
    }
  }

  public get hasDataChange(): boolean {
    return this.courseFilterVm.dataHasChanged();
  }

  private _loadDataSub: Subscription = new Subscription();
  private _inputs: ICourseFilterInput = {
    data: new CourseFilterModel(),
    settings: null
  };

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private taggingRepository: TaggingRepository,
    private organizationRepository: OrganizationRepository
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
        this.actionFn.applyFn(this.courseFilterVm.data);
      }
    });
  }

  public onClear(): void {
    Utils.setValueAllProperties(this.courseFilterVm.data, undefined);
  }

  protected onInit(): void {
    this.loadData();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'course-filter',
      validateByGroupControlNames: [['createdDateFrom', 'createdDateTo'], ['changedDateFrom', 'changedDateTo']],
      controls: {
        ownBy: {
          defaultValue: null,
          validators: null
        },
        fromOrganisation: {
          defaultValue: null,
          validators: null
        },
        createdDateFrom: {
          defaultValue: null,
          validators: [
            {
              validator: startEndValidator('createdDateFrom', p => p.value, p => this.courseFilterVm.data.createdDateTo, true, 'dateOnly'),
              validatorType: 'createdDateFrom',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'From Date cannot be greater than To Date')
            },
            {
              validator: mustBeInThePastValidator(),
              validatorType: validateMustBePastDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'From Date cannot be in the future')
            }
          ]
        },
        createdDateTo: {
          defaultValue: null,
          validators: [
            {
              validator: startEndValidator('createdDateTo', p => this.courseFilterVm.data.createdDateFrom, p => p.value, true, 'dateOnly'),
              validatorType: 'createdDateTo',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'To Date cannot be less than From Date')
            },
            {
              validator: mustBeInThePastValidator(),
              validatorType: validateMustBePastDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'To Date cannot be in the future')
            }
          ]
        },
        changedDateFrom: {
          defaultValue: null,
          validators: [
            {
              validator: startEndValidator('createdDateFrom', p => p.value, p => this.courseFilterVm.data.changedDateTo, true, 'dateOnly'),
              validatorType: 'createdDateFrom',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'From Date cannot be greater than To Date')
            },
            {
              validator: mustBeInThePastValidator(),
              validatorType: validateMustBePastDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'From Date cannot be in the future')
            }
          ]
        },
        changedDateTo: {
          defaultValue: null,
          validators: [
            {
              validator: startEndValidator('createdDateTo', p => this.courseFilterVm.data.changedDateFrom, p => p.value, true, 'dateOnly'),
              validatorType: 'createdDateTo',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'To Date cannot be less than From Date')
            },
            {
              validator: mustBeInThePastValidator(),
              validatorType: validateMustBePastDateType,
              message: new TranslationMessage(this.moduleFacadeService.translator, 'To Date cannot be in the future')
            }
          ]
        },
        status: {
          defaultValue: null,
          validators: null
        },
        contentStatus: {
          defaultValue: null,
          validators: null
        },
        pdType: {
          defaultValue: null,
          validators: null
        },
        category: {
          defaultValue: null,
          validators: null
        },
        serviceScheme: {
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
        courseLevel: {
          defaultValue: null,
          validators: null
        },
        subject: {
          defaultValue: null,
          validators: null
        },
        learningFrameworks: {
          defaultValue: null,
          validators: null
        },
        learningDimension: {
          defaultValue: null,
          validators: null
        },
        learningArea: {
          defaultValue: null,
          validators: null
        },
        learningSubArea: {
          defaultValue: null,
          validators: null
        }
      }
    };
  }

  private loadData(): void {
    this._loadDataSub.unsubscribe();
    const taggingObs = this.taggingRepository.loadAllMetaDataTags();
    this._loadDataSub = combineLatest(taggingObs)
      .pipe(
        switchMap(([metadatas]) =>
          CourseFilterViewModel.create(
            ids => this.organizationRepository.loadOrganizationalUnitsByIds(ids),
            metadatas,
            this.inputs.data ? this.inputs.data : new CourseFilterModel(),
            this.inputs.settings
          )
        ),
        this.untilDestroy()
      )
      .subscribe(vm => {
        this.courseFilterVm = vm;
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
