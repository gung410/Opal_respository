import { BasePageComponent, ModuleFacadeService, TranslationMessage } from '@opal20/infrastructure';

import { BehaviorSubject } from 'rxjs';
import { Component } from '@angular/core';

@Component({
  selector: 'modal-service',
  template: `
    <h3>Modal Service</h3>
    Message: {{ message$ | async }}
    <br />
    <button (click)="showConfirmation()">Show Confirmation</button>
    <button (click)="showInformation()">Show Information</button>
    <button (click)="showSuccessMessage()">Show Success</button>
    <button (click)="showErrorMessage()">Show Error</button>
    <button (click)="showWarningMessage()">Show Warning</button>
    <button (click)="showCustomMessage()">Show Custom Message</button>
  `
})
export class POCModalComponent extends BasePageComponent {
  public message$: BehaviorSubject<string> = new BehaviorSubject('...');

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public showConfirmation(): void {
    this.moduleFacadeService.modalService.showConfirmMessage(
      'Can you confirm this ...?',
      this.onYesCallback.bind(this),
      this.onNoCallback.bind(this),
      this.onCloseCallback.bind(this)
    );
  }

  public showInformation(): void {
    this.moduleFacadeService.modalService.showInformationMessage(
      'This is an information message ...',
      this.onOkCallback.bind(this),
      this.onCloseCallback.bind(this)
    );
  }

  public showSuccessMessage(): void {
    this.moduleFacadeService.modalService.showSuccessMessage(
      'This is an success message ...',
      this.onOkCallback.bind(this),
      this.onCloseCallback.bind(this)
    );
  }

  public showErrorMessage(): void {
    this.moduleFacadeService.modalService.showErrorMessage(
      'This is an error message ...',
      this.onOkCallback.bind(this),
      this.onCloseCallback.bind(this)
    );
  }

  public showWarningMessage(): void {
    this.moduleFacadeService.modalService.showWarningMessage(
      'This is an warning message ...',
      this.onOkCallback.bind(this),
      this.onCloseCallback.bind(this)
    );
  }

  public showCustomMessage(): void {
    this.moduleFacadeService.modalService.showMessage(
      new TranslationMessage(this.moduleFacadeService.translator, 'poc.modal.title'),
      new TranslationMessage(this.moduleFacadeService.translator, 'poc.modal.content'),
      [
        {
          text: new TranslationMessage(this.moduleFacadeService.translator.globalTranslator, 'Yes'),
          callback: this.onYesCallback.bind(this)
        },
        {
          text: new TranslationMessage(this.moduleFacadeService.translator.globalTranslator, 'No'),
          callback: this.onNoCallback.bind(this)
        },
        {
          text: new TranslationMessage(this.moduleFacadeService.translator.globalTranslator, 'OK'),
          callback: this.onOkCallback.bind(this)
        },
        {
          text: new TranslationMessage(this.moduleFacadeService.translator.globalTranslator, 'Cancel'),
          callback: this.onCancelCallback.bind(this)
        }
      ],
      this.onCloseCallback.bind(this)
    );
  }

  private onYesCallback(): void {
    this.message$.next('You choose YES.');
  }

  private onNoCallback(): void {
    this.message$.next('You choose NO.');
  }

  private onOkCallback(): void {
    this.message$.next('You choose OK.');
  }

  private onCancelCallback(): void {
    this.message$.next('You choose Cancel.');
  }

  private onCloseCallback(): void {
    this.message$.next('You closed the modal.');
  }
}
