import { Component, OnDestroy } from '@angular/core';
import { IHeaderAngularComp } from 'ag-grid-angular';
import { IAfterGuiAttachedParams, IHeaderParams } from 'ag-grid-community';
import { ModuleType } from 'app/permissions/enum/module-type.enum';
import { PermissionsApiService } from 'app/permissions/services/permissions-api.service';
import { PermissionsTableService } from 'app/permissions/services/permissions-table.service';
import { AccessRightsViewModel } from 'app/permissions/viewmodels/acessrights.viewmodel';
import { Subscription } from 'rxjs';

@Component({
  selector: 'dropdown-list-modules-header',
  templateUrl: './dropdown-list-modules-header.component.html',
  styleUrls: ['./dropdown-list-modules-header.component.scss']
})
export class DropdownListModulesHeaderComponent
  implements IHeaderAngularComp, OnDestroy {
  params: any;
  moduleSelectionEventEmitter: (event: number) => void;
  selectedModuleId: number = 0;
  batchJobsMonitoringModule: ModuleType = ModuleType.BatchJobsMonitoring;
  organizationSpaModule: ModuleType = ModuleType.OrganizationSpa;
  moduleItems: AccessRightsViewModel[] = [];
  moduleSubscription: Subscription;
  cslModuleId: number;

  constructor(
    private permissionsApiSvc: PermissionsApiService,
    private permissionsTableSvc: PermissionsTableService
  ) {
    this.getModules();
  }
  agInit(params: any): void {
    this.params = params;
    this.moduleSelectionEventEmitter = params.onClick;
  }

  refresh(params: IHeaderParams): boolean {
    // throw new Error('Method not implemented.');
    return false;
  }

  onItemChanged(moduleId: number): void {
    this.moduleSelectionEventEmitter(moduleId);
  }

  afterGuiAttached?(params?: IAfterGuiAttachedParams): void {
    // throw new Error('Method not implemented.');
  }

  ngOnDestroy(): void {
    if (this.moduleSubscription) {
      this.moduleSubscription.unsubscribe();
    }
  }

  private getModules(): void {
    if (this.permissionsTableSvc.moduleItems) {
      this.selectedModuleId = this.permissionsTableSvc.currentModuleId;
      this.moduleItems = this.permissionsTableSvc.moduleItems;
      this.cslModuleId = this.permissionsTableSvc.communitySiteModuleId;
    }
  }
}
