import { ComponentType, ZINDEX_LEVEL_4 } from '@opal20/infrastructure';
import { DialogRef, DialogService } from '@progress/kendo-angular-dialog';
import { Injectable, NgZone } from '@angular/core';

import { DialogAction } from '../../models/dialog-action.model';
import { Observable } from 'rxjs';
import { OpalConfirmDialogComponent } from './confirm-dialog/confirm-dialog.component';
import { OpalDialogSettings } from './dialog-configs';
import { map } from 'rxjs/operators';

// @dynamic
@Injectable()
export class OpalDialogService {
  public static get defaultDialogConfigs(): OpalDialogSettings {
    return {
      maxWidth: '95vw',
      maxHeight: '95vh',
      padding: '20px'
    };
  }

  public currentDialogsRef: DialogRef[] = [];

  constructor(private _kendoDialogSvc: DialogService, private _ngZone: NgZone) {}

  public openDialog<T, InputT>(component: ComponentType<T>, inputs?: InputT, configs?: OpalDialogSettings): Observable<unknown> {
    const dialogRef = this.openDialogRef(component, inputs, configs);
    return dialogRef.result;
  }

  public openDialogRef<T>(component: ComponentType<T>, inputs?: Partial<T>, configs?: OpalDialogSettings): DialogRef {
    return this._ngZone.run(() => {
      const finalConfigs = {
        ...OpalDialogService.defaultDialogConfigs,
        ...configs
      };

      const dialogRef = this._kendoDialogSvc.open({
        ...finalConfigs,
        content: component
      });
      this.currentDialogsRef.push(dialogRef);

      if (inputs != null) {
        this._assignInputsData(dialogRef, inputs);
      }

      const dialogContainerElement = this.getDialogContainerElement(dialogRef);
      dialogContainerElement.style.display = 'flex';
      dialogContainerElement.style.flexDirection = 'column';
      dialogContainerElement.style.overflow = 'visible';
      dialogContainerElement.style.maxWidth = finalConfigs.maxWidth != null ? finalConfigs.maxWidth : '100vw';
      dialogContainerElement.style.maxHeight = finalConfigs.maxHeight != null ? finalConfigs.maxHeight : '100vh';
      dialogContainerElement.style.borderRadius = finalConfigs.borderRadius != null ? finalConfigs.borderRadius : '5px';

      const dialogWrapperElement = this.getDialogWrapperElement(dialogRef);
      dialogWrapperElement.style.zIndex = (finalConfigs.zIndex != null ? finalConfigs.zIndex : ZINDEX_LEVEL_4).toString();

      const dialogContentElement = this.getDialogContentElement(dialogRef);
      dialogContentElement.style.padding = finalConfigs.padding != null ? finalConfigs.padding : '0px';
      return dialogRef;
    });
  }

  public openConfirmDialogRef(inputs?: Partial<OpalConfirmDialogComponent>, configs?: OpalDialogSettings): DialogRef {
    return this._ngZone.run(() => {
      if (inputs == null) {
        inputs = {};
      }

      const finalConfigs = {
        ...OpalConfirmDialogComponent.dialogConfigs,
        ...configs
      };
      const dialogRef = this.openDialogRef(OpalConfirmDialogComponent, inputs, finalConfigs);

      const userOnConfirm = inputs.onConfirm;
      const userConfirmRequest = inputs.confirmRequest;
      let confirmedResult: DialogAction | null = null;

      inputs.onConfirm = ((result: DialogAction, skipConfirmRequest: boolean = false) => {
        confirmedResult = result;

        if (userOnConfirm != null) {
          userOnConfirm(result);
        }

        if (userConfirmRequest == null || result === DialogAction.Cancel || skipConfirmRequest) {
          this.closeDialog(dialogRef, result);
        }
      }).bind(this);

      const userOnClose = inputs.onClose;

      inputs.onClose = (() => {
        confirmedResult = DialogAction.Close;

        if (userOnClose != null) {
          userOnClose();
        }

        this.closeDialog(dialogRef, DialogAction.Close);
      }).bind(this);
      this._assignInputsData(dialogRef, inputs);

      dialogRef.result = dialogRef.result.pipe(
        map(_ => {
          return confirmedResult;
        })
      );

      return dialogRef;
    });
  }

  public openConfirmDialog(inputs?: Partial<OpalConfirmDialogComponent>, configs?: OpalDialogSettings): Observable<DialogAction> {
    return <Observable<DialogAction>>this.openConfirmDialogRef(inputs, configs).result;
  }

  public closeLastCurrentDialog(dialogResult?: unknown): void {
    return this._ngZone.run(() => {
      const lastCurrentDialogRef = this.currentDialogsRef.pop();

      if (lastCurrentDialogRef == null) {
        return;
      }

      lastCurrentDialogRef.close(dialogResult);
    });
  }

  public closeAllDialogs(): void {
    return this._ngZone.run(() => {
      this.currentDialogsRef.forEach(p => {
        p.close();
      });
      this.currentDialogsRef = [];
    });
  }

  public closeDialog(dialogRef: DialogRef, dialogResult?: unknown): void {
    return this._ngZone.run(() => {
      const newCurrentDialogsRef: DialogRef[] = [];

      this.currentDialogsRef.forEach(p => {
        if (p === dialogRef) {
          p.close(dialogResult);
        } else {
          newCurrentDialogsRef.push(p);
        }
      });
      this.currentDialogsRef = newCurrentDialogsRef;
    });
  }

  public getDialogContainerElement(dialogRef: DialogRef): HTMLElement {
    // tslint:disable-next-line:no-any
    return (<any>dialogRef).dialog.instance._elRef.nativeElement.querySelector('[role="dialog"]');
  }

  public getDialogContentElement(dialogRef: DialogRef): HTMLElement {
    // tslint:disable-next-line:no-any
    return (<any>dialogRef).dialog.instance._elRef.nativeElement.querySelector('.k-content.k-window-content.k-dialog-content');
  }

  public getDialogWrapperElement(dialogRef: DialogRef): HTMLElement {
    // tslint:disable-next-line:no-any
    return (<any>dialogRef).dialog.location.nativeElement;
  }

  private _assignInputsData(dialogRef: DialogRef, inputs: Object): void {
    const sourceProperties = Object.getOwnPropertyNames(inputs);

    sourceProperties.forEach(prop => {
      // tslint:disable-next-line:no-any
      const propValue = (<any>inputs)[prop];

      if (propValue !== null) {
        dialogRef.content.instance[prop] = propValue;
      }
    });
  }
}
