import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, TranslationMessage, Utils } from '@opal20/infrastructure';
import { DepartmentInfoModel, OrganizationRepository, TaggingRepository } from '@opal20/domain-api';
import { Observable, Subscription, combineLatest } from 'rxjs';

import { BlockoutDateFilterModel } from './../../models/blockout-date-filter.model';
import { BlockoutDateFilterViewModel } from './../../view-models/blockout-date-filter-view.model';
import { CommonFilterAction } from './../../models/common-filter-action.model';
import { Component } from '@angular/core';
import { PopupRef } from '@progress/kendo-angular-popup';
import { startEndValidator } from '@opal20/common-components';
import { switchMap } from 'rxjs/operators';

interface IBlockoutDateFilterInput {
  data: BlockoutDateFilterModel;
}

@Component({
  selector: 'blockout-date-filter',
  templateUrl: './blockout-date-filter.component.html'
})
export class BlockoutDateFilterComponent extends BaseFormComponent {
  public popupRef: PopupRef;
  public actionFn: CommonFilterAction;
  public fetchDepartmentFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<DepartmentInfoModel[]> = null;
  public blockoutDateFilterVm: BlockoutDateFilterViewModel = new BlockoutDateFilterViewModel([]);

  public get inputs(): IBlockoutDateFilterInput {
    return this._inputs;
  }

  public set inputs(v: IBlockoutDateFilterInput) {
    if (v != null) {
      this._inputs = v;
    }
  }

  public get hasDataChange(): boolean {
    return this.blockoutDateFilterVm.dataHasChanged();
  }

  private _loadDataSub: Subscription = new Subscription();
  private _inputs: IBlockoutDateFilterInput = {
    data: new BlockoutDateFilterModel()
  };

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private taggingRepository: TaggingRepository,
    private organizationRepository: OrganizationRepository
  ) {
    super(moduleFacadeService);
    this.fetchDepartmentFn = this._createFetchDepartmentFn();
  }

  public onApply(): void {
    this.validate().then(valid => {
      if (valid) {
        this.popupRef.close();
        this.actionFn.applyFn(this.blockoutDateFilterVm.data);
      }
    });
  }

  public onClear(): void {
    Utils.setValueAllProperties(this.blockoutDateFilterVm.data, undefined);
  }

  protected onInit(): void {
    this.loadData();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'blockout-date-filter',
      validateByGroupControlNames: [['createdDateFrom', 'createdDateTo'], ['changedDateFrom', 'changedDateTo']],
      controls: {
        createdDateFrom: {
          defaultValue: null,
          validators: [
            {
              validator: startEndValidator(
                'createdDateFrom',
                p => p.value,
                p => this.blockoutDateFilterVm.data.createdDateTo,
                true,
                'dateOnly'
              ),
              validatorType: 'createdDateFrom',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'From Date cannot be greater than To Date')
            }
          ]
        },
        createdDateTo: {
          defaultValue: null,
          validators: [
            {
              validator: startEndValidator(
                'createdDateTo',
                p => this.blockoutDateFilterVm.data.createdDateFrom,
                p => p.value,
                true,
                'dateOnly'
              ),
              validatorType: 'createdDateTo',
              message: new TranslationMessage(this.moduleFacadeService.translator, 'To Date cannot be less than From Date')
            }
          ]
        },
        serviceSchemes: {
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
          BlockoutDateFilterViewModel.create(metadatas, this.inputs.data ? this.inputs.data : new BlockoutDateFilterModel())
        ),
        this.untilDestroy()
      )
      .subscribe(vm => {
        this.blockoutDateFilterVm = vm;
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
