import {
  AfterViewInit,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  Input,
  OnDestroy,
  OnInit,
  TemplateRef,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';

import { ButtonDirective } from '@progress/kendo-angular-buttons';
import { DialogAction } from '../../../models/dialog-action.model';
import { OpalDialogSettings } from '../dialog-configs';

// @dynamic
@Component({
  selector: 'opal-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class OpalConfirmDialogComponent extends BaseComponent implements OnDestroy, OnInit, AfterViewInit {
  public static get dialogConfigs(): OpalDialogSettings {
    return {};
  }

  @ViewChild('yesBtn', { static: true }) public yesBtn: ButtonDirective;

  @Input() public onConfirm?: (result: DialogAction, skipConfirmRequest?: boolean) => void;
  @Input() public onClose?: () => void;
  @Input() public confirmTitle: string = 'Confirmation';
  @Input() public confirmTitleParams?: object;
  @Input() public confirmMsg: string | undefined = 'Are you sure?';
  @Input() public confirmMsgHtml: string | undefined;
  @Input() public confirmMsgParams?: object;
  @Input() public yesBtnPrimary: boolean = true;
  @Input() public noBtnPrimary: boolean = false;
  @Input() public noBtnText: string = 'No';
  @Input() public yesBtnText: string = 'Yes';
  @Input() public bodyTemplate: TemplateRef<unknown> | undefined;
  @Input() public hideNoBtn?: boolean;
  @Input() public hideYesBtn?: boolean;
  @Input() public closeIconClass?: string = 'k-icon k-i-close';
  @Input() public showCloseBtn?: boolean = true;
  @Input() public disableYesBtnFn?: () => boolean;
  @Input() public confirmRequest?: () => Promise<[string[], undefined] | [undefined, unknown]>;
  @Input() public confirmRequestSuccessMsg?: string = 'Your request has been processed successfully';
  @Input() public confirmRequestSuccessMsgParams?: object;
  @Input() public confirmRequestResultTemplate: TemplateRef<unknown> | undefined;
  @Input() public confirmRequestNotShowFailedMsg: boolean = false;
  @Input() public confirmRequestNotShowSuccessMsg: boolean = false;

  public confirmRequestSucceeded: boolean = false;
  public confirmRequestData: unknown | undefined;
  private _errors: string[] | undefined;
  public get errors(): string[] | undefined {
    return this._errors;
  }
  public set errors(value: string[] | undefined) {
    if (!Utils.isDifferent(this._errors, value)) {
      return;
    }
    this._errors = value;
    if (this.initiated) {
      this.changeDetectorRef.detectChanges();
    }
  }
  private _connectingApi: boolean = false;
  public get connectingApi(): boolean {
    return this._connectingApi;
  }
  public set connectingApi(value: boolean) {
    if (this._connectingApi === value) {
      return;
    }
    this._connectingApi = value;
    if (this.initiated) {
      this.changeDetectorRef.detectChanges();
    }
  }

  constructor(moduleFacadeService: ModuleFacadeService, public changeDetectorRef: ChangeDetectorRef) {
    super(moduleFacadeService);
  }

  public ngAfterViewInit(): void {
    super.ngAfterViewInit();
    this.yesBtn.focus();
  }

  public async onYesBtnClicked(e: MouseEvent): Promise<void> {
    if (this.onConfirm != null) {
      this.onConfirm(DialogAction.OK);
    }
    if (this.confirmRequest != null) {
      this.confirmRequestSucceeded = false;
      this.errors = undefined;
      this.confirmRequestData = undefined;
      this.connectingApi = true;
      const [error, data] = await this.confirmRequest();
      this.connectingApi = false;
      this.errors = error;
      this.confirmRequestData = data;
      this.confirmRequestSucceeded = this.errors == null;
      if (this.confirmRequestNotShowSuccessMsg && this.confirmRequestSucceeded && this.onClose != null) {
        this.onConfirm(DialogAction.OK, true);
      }
      this.changeDetectorRef.detectChanges();
    }
  }

  public onNoBtnClicked(e: MouseEvent): void {
    if (this.onConfirm != null) {
      this.onConfirm(DialogAction.Cancel);
    }
  }

  public onCloseBtnClicked(e: MouseEvent): void {
    if (this.onClose != null) {
      this.onClose();
    }
  }

  public getErrorsAsHtml(...errorss: (string[] | undefined)[]): string | undefined {
    let result: string = '';
    for (let i = 0; i < errorss.length; i++) {
      const errors = errorss[i];
      if (errors != null && errors.length) {
        result += errors.join('<br>');
      }
    }
    return result === '' ? undefined : result;
  }

  public isYesBtnDisabled(): boolean {
    return this.connectingApi || (this.disableYesBtnFn && this.disableYesBtnFn());
  }

  public isNoBtnDisabled(): boolean {
    return this.connectingApi;
  }
}
