import { SweetAlertResult } from 'sweetalert2';

export interface CxInformationDialogResult extends SweetAlertResult {
}

export class CxInformationDialogParams {
  /**
   * The content of the message.
   */
  message?: string;
  /**
   * The title (header) of the dialog.
   */
  title?: string;
  /**
   * Use this to change the text on the "Confirm"-button.
   *
   * @Default 'OK'
   */
  confirmButtonText?: string;
  /**
   * Use this to change the text on the "Cancel"-button.
   *
   * @Default 'Cancel'
   */
  cancelButtonText?: string;
  /**
   * Set to true if you want to invert default buttons positions.
   */
  reverseButtons?: boolean;

  constructor(data?: Partial<CxInformationDialogParams>) {
    if (!data) {
      return;
    }
    this.message = data.message;
    this.title = data.title;
    this.confirmButtonText = data.confirmButtonText;
    this.cancelButtonText = data.cancelButtonText;
    this.reverseButtons = data.reverseButtons;
  }
}
