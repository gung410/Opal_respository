import { Injectable } from '@angular/core';
import Swal, { SweetAlertResult, SweetAlertIcon } from 'sweetalert2';
import { CxInformationDialogResult, CxInformationDialogParams } from './cx-information-dialog.model';

@Injectable({
  providedIn: 'root'
})
export class CxInformationDialogService {
  defaultConfirmButtonText: string = 'OK';
  constructor() {
  }

  public success(params: CxInformationDialogParams): Promise<CxInformationDialogResult> {
    return this.show('success', params);
  }

  public warning(params: CxInformationDialogParams): Promise<CxInformationDialogResult> {
    return this.show('warning', params);
  }

  public error(params: CxInformationDialogParams): Promise<CxInformationDialogResult> {
    return this.show('error', params);
  }

  public info(params: CxInformationDialogParams): Promise<CxInformationDialogResult> {
    return this.show('info', params);
  }

  public question(params: CxInformationDialogParams): Promise<CxInformationDialogResult> {
    return this.show('question', params, true);
  }

  private show(
    swalType: SweetAlertIcon,
    params: CxInformationDialogParams,
    showCancelButton: boolean = false
  ): Promise<SweetAlertResult> {
    return Swal.fire({
      reverseButtons: params.reverseButtons ? params.reverseButtons : true,
      focusConfirm: true,
      allowOutsideClick: false,
      icon: swalType,
      text: params.message,
      titleText: params.title,
      confirmButtonText: params.confirmButtonText ? params.confirmButtonText : this.defaultConfirmButtonText,
      cancelButtonText: params.cancelButtonText,
      showCancelButton,
    });
  }
}
