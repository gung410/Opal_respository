import { Component, OnDestroy } from '@angular/core';
import { IHeaderAngularComp } from 'ag-grid-angular';
import { IAfterGuiAttachedParams, IHeaderParams } from 'ag-grid-community';
import { AccessRightsRequest } from 'app/permissions/dtos/request-dtos/access-rights-request';
import { ObjectType } from 'app/permissions/enum/object-type.enum';
import { AccessRightsModel } from 'app/permissions/models/access-rights.model';
import { PermissionsApiService } from 'app/permissions/services/permissions-api.service';
import { AccessRightsViewModel } from 'app/permissions/viewmodels/acessrights.viewmodel';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'dropdown-list-modules-header',
  templateUrl: './dropdown-list-modules-header.component.html',
  styleUrls: ['./dropdown-list-modules-header.component.scss']
})
export class DropdownListModulesHeaderComponent
  implements IHeaderAngularComp, OnDestroy {
  params: any;
  moduleSelectionEventEmitter: (event: number) => void;
  selectedModuleId: number = 5; // Id of Module SAM
  moduleItems: AccessRightsViewModel[] = [];

  constructor(private permissionsApiSvc: PermissionsApiService) {
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
    // throw new Error('Method not implemented.');
  }

  getModules(): void {
    this.permissionsApiSvc
      .getAccessRights(
        new AccessRightsRequest({
          objectTypes: [ObjectType.Module],
          includeLocalizedData: true
        })
      )
      .pipe(
        map((modules) => {
          return modules.map((module) => new AccessRightsViewModel(module));
        })
      )
      .subscribe((modules) => {
        this.moduleItems.push(...modules);
      });
  }
}
