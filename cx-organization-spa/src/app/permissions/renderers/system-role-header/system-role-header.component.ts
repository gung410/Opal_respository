import { Component, ElementRef, OnDestroy } from '@angular/core';

import { IHeaderAngularComp } from 'ag-grid-angular';

import { IAfterGuiAttachedParams, IHeaderParams } from 'ag-grid-community';

import { EditingSignal } from 'app/permissions/dtos/editing-signal.dto';

import { SystemRoleAction } from 'app/permissions/enum/permission-action.enum';

import { ColumnItemModel } from 'app/permissions/models/column-item.model';

import { GrantedAccessRightsModel } from 'app/permissions/models/granted-access-rights.model';

import { SystemRoleHeaderExtraAttributesModel } from 'app/permissions/models/system-role-header-extra-attributes.model';

import { PermissionsColumnService } from 'app/permissions/services/permissions-column.service';

import { PermissionsTableService } from 'app/permissions/services/permissions-table.service';

import { Observable, Subscription } from 'rxjs';

import { filter, map } from 'rxjs/operators';

@Component({
  selector: 'system-role-header',
  templateUrl: './system-role-header.component.html',
  styleUrls: ['./system-role-header.component.scss']
})
export class SystemRoleHeaderComponent
  implements IHeaderAngularComp, OnDestroy {
  subscription: Subscription = new Subscription();
  originColName: string = '';
  colName: string = '';
  isHidden$: Observable<boolean>;
  communitySiteModuleId: number;

  colId: number | string;
  isEditing: boolean = false;
  isShow: boolean = true;
  extraAttributes: SystemRoleHeaderExtraAttributesModel;
  permissionActionsEventEmitter: (
    systemRoleAction: SystemRoleAction,
    systemRoleId: number | string,
    systemRoleName?: string
  ) => void;
  params: any;

  constructor(
    private permissionsTableSvc: PermissionsTableService,
    public elementRef: ElementRef<HTMLElement>,
    private permissionsColumnSvc: PermissionsColumnService<
      ColumnItemModel<GrantedAccessRightsModel>
    >
  ) {
    this.communitySiteModuleId = this.permissionsTableSvc.communitySiteModuleId;
    this.createEditObserver();
    this.createActionsObserver();
    this.checkCurrentModuleToHide();
  }

  agInit(params: any): void {
    this.params = params;
    this.originColName = params.displayName as string;
    this.colName = params.displayName as string;
    this.colId = params.column.colId;
    this.permissionActionsEventEmitter = params.onClick;
    this.extraAttributes = params.extraAttributes;
  }

  refresh(params: IHeaderParams): boolean {
    return false;
  }

  afterGuiAttached?(params?: IAfterGuiAttachedParams): void {}

  onEditPermissionsClicked(): void {
    this.sendEditNotification(true);
    this.permissionsColumnSvc.activateColumn(this.colId);
    this.permissionActionsEventEmitter(SystemRoleAction.Edit, this.colId);
  }

  onCancelIconClicked(): void {
    // this.resetColState();
    this.permissionActionsEventEmitter(SystemRoleAction.Cancel, this.colId);
    // this.sendEditNotification(false);
  }

  onEditRoleClicked(): void {
    this.permissionActionsEventEmitter(SystemRoleAction.EditRole, this.colId);
  }

  onDuplicateClicked(): void {
    this.permissionActionsEventEmitter(
      SystemRoleAction.Clone,
      this.colId,
      this.colName
    );
  }

  onSaveIconClicked(): void {
    this.permissionActionsEventEmitter(
      SystemRoleAction.Save,
      this.colId,
      this.colName
    );
    // this.sendEditNotification(false);
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  private columnHighlightSwitcher(switchSate: boolean = true): void {
    const columnElements: NodeListOf<Element> = document.querySelectorAll(
      `[col-id="${this.colId}"]`
    );
    if (!switchSate) {
      columnElements.forEach((colElement) => {
        colElement.classList.remove('high-light-column');
      });

      return;
    }

    columnElements.forEach((colElement) => {
      colElement.classList.add('high-light-column');
    });
  }

  private createActionsObserver(): void {
    const actionSubscription = this.permissionsTableSvc.actionSignal$
      .pipe(filter((actionSignal) => actionSignal.colId === this.colId))
      .subscribe((actionSignal) => {
        switch (actionSignal.actionName) {
          case 'Cancel':
            this.processCancelActions();
            break;
          case 'Save':
            this.processSaveActions();
            break;
          default:
            break;
        }
      });
    this.subscription.add(actionSubscription);
  }

  private checkCurrentModuleToHide(): void {
    this.isHidden$ = this.permissionsTableSvc.moduleSelectionSignal$.pipe(
      map((moduleId) => this.communitySiteModuleId === moduleId)
    );
  }

  private createEditObserver(): void {
    const editSubscription = this.permissionsTableSvc.editingSignal$.subscribe(
      (editingSignal) => {
        if (editingSignal.systemRoleId !== this.colId) {
          this.isShow = !editingSignal.isEditing;
        } else if (editingSignal.systemRoleId === this.colId) {
          this.isEditing = editingSignal.isEditing;
          this.columnHighlightSwitcher(editingSignal.isEditing);
        }
      }
    );
    this.subscription.add(editSubscription);
  }

  private processCancelActions(): void {
    this.resetColState();
    this.sendEditNotification(false);
  }

  private processSaveActions(): void {
    this.sendEditNotification(false);
  }

  private resetColState(): void {}

  private sendEditNotification(isEditing: boolean): void {
    this.permissionsTableSvc.sendEditingSignal(
      new EditingSignal({
        isEditing,
        systemRoleId: this.colId.toString()
      })
    );
  }
}
