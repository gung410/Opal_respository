import { Component, OnDestroy } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { IAfterGuiAttachedParams } from 'ag-grid-community';
import { GrantedType } from 'app/permissions/enum/granted-type.enum';
import { ColumnItemModel } from 'app/permissions/models/column-item.model';
import { GrantedAccessRightsModel } from 'app/permissions/models/granted-access-rights.model';
import { PermissionsColumnService } from 'app/permissions/services/permissions-column.service';
import { PermissionsTableService } from 'app/permissions/services/permissions-table.service';
import { BehaviorSubject, Subscription } from 'rxjs';
import { filter, tap } from 'rxjs/operators';
import { Utils } from '../../../shared/utilities/utils';

@Component({
  selector: 'checkbox-renderer',
  templateUrl: './checkbox-renderer.component.html'
})
export class CheckboxRendererComponent
  implements ICellRendererAngularComp, OnDestroy {
  subscription: Subscription = new Subscription();
  isCheckboxHidden: boolean = false;
  params: any;
  get checkState(): boolean {
    return this._checkState;
  }
  set checkState(isCheckState: boolean) {
    if (isCheckState == null) {
      return;
    }

    this._checkState = isCheckState;
  }

  isDisabled: boolean = true;

  private _systemRoleId: string; // also Column Id
  private _accessRightId: number;
  private _currentGrantedAccessRights: GrantedAccessRightsModel = new GrantedAccessRightsModel();
  private _checkState: boolean;
  private _originCheckState: boolean;
  private grantedAccessRightSubject: BehaviorSubject<
    ColumnItemModel<GrantedAccessRightsModel>
  >;

  constructor(
    private permissionsTableSvc: PermissionsTableService,
    private permissionsColumnSvc: PermissionsColumnService<
      ColumnItemModel<GrantedAccessRightsModel>
    >
  ) {
    this.creatEditObserver();
    this.createCancelObserver();
  }

  agInit(params: any): void {
    this.setupVariable(params);
    this.buildGrantedAccessRights(params.value);
    this.checkLastRowRender();

    this.grantedAccessRightSubject = new BehaviorSubject<
      ColumnItemModel<GrantedAccessRightsModel>
    >(
      new ColumnItemModel({
        item: this._currentGrantedAccessRights,
        colId: this._currentGrantedAccessRights.systemRoleId
      })
    );
    this.permissionsColumnSvc.registerObs(this.grantedAccessRightSubject);
  }

  checkedHandler(event: any): void {
    this.checkState = event.target.checked;
    this.sendCheckboxChange(event.target.checked);
    // this.params.node.setDataValue(colId, checked);
  }

  refresh(params: any): boolean {
    return false;
  }

  afterGuiAttached?(params?: IAfterGuiAttachedParams): void {
    // throw new Error('Method not implemented.');
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  private setupVariable(params: any): void {
    this.params = params;
    this.checkState = Utils.isDefined(params.value) ? params.value : false;
    this._originCheckState = this.checkState;
    this._systemRoleId = this.params.column.colId;
    this._accessRightId = this.params.data.accessRightId;
    this.isCheckboxHidden = this.params.data.isHideAccessRight;
  }

  private checkLastRowRender(): void {
    this.permissionsTableSvc.sendLastRowRenderSignal(this._accessRightId);
  }

  private creatEditObserver(): void {
    const editSubscription = this.permissionsTableSvc.editingSignal$
      .pipe(
        filter(
          (editingSignal) => editingSignal.systemRoleId === this._systemRoleId
        )
      )
      .subscribe((editingSignal) => {
        this.isDisabled = !editingSignal.isEditing;
      });
    this.subscription.add(editSubscription);
  }

  private createCancelObserver(): void {
    const cancelSubscription = this.permissionsTableSvc.actionSignal$
      .pipe(
        filter(
          (actionSignal) =>
            actionSignal.colId === this._systemRoleId &&
            actionSignal.actionName === 'Cancel'
        )
      )
      .subscribe((actionSignal) => {
        if (actionSignal.isPerformAction) {
          this.updateCell(true);

          return;
        }

        this.updateCell();
      });
    this.subscription.add(cancelSubscription);
  }

  private updateCell(isDiscardChange: boolean = false): void {
    if (isDiscardChange) {
      this.checkState = this._originCheckState;
      this.sendCheckboxChange(this.checkState);
      // Due to registered observable issue, temporary comment this line.
      // this.updateCheckBoxForNode();

      return;
    }
    // Due to registered observable issue, temporary comment this line.
    // this.updateCheckBoxForNode();
    this._originCheckState = this.checkState;
  }

  private updateCheckBoxForNode(): void {
    this.params.node.setDataValue(this._systemRoleId, this.checkState);
  }

  private buildGrantedAccessRights(isGranted: boolean): void {
    this._currentGrantedAccessRights.systemRoleId = Number(this._systemRoleId);
    this._currentGrantedAccessRights.accessRightId = this._accessRightId;
    this._currentGrantedAccessRights.grantedType = isGranted
      ? GrantedType.Allow
      : GrantedType.Deny;
  }

  private sendCheckboxChange(isGranted: boolean): void {
    this._currentGrantedAccessRights.grantedType = isGranted
      ? GrantedType.Allow
      : GrantedType.Deny;

    // tslint:disable-next-line:no-angle-bracket-type-assertion
    this.grantedAccessRightSubject.next(<
      ColumnItemModel<GrantedAccessRightsModel>
    >{
      item: this._currentGrantedAccessRights,
      colId: this._currentGrantedAccessRights.systemRoleId
    });
  }
}
