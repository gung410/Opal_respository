import { DialogCloseResult, DialogRef, DialogService, DialogSettings } from '@progress/kendo-angular-dialog';

import { GlobalTranslatorService } from '../translation/global-translator.service';
import { Injectable } from '@angular/core';
import { TranslationMessage } from '../translation/translation.models';

export interface IModalAction {
  text: string | TranslationMessage;
  primary?: boolean;
  data?: unknown;
  callback?: () => void | boolean;
}

@Injectable()
export class ModalService {
  constructor(private dialogService: DialogService, private globalTranslator: GlobalTranslatorService) {}

  public showConfirmMessage(
    content: string | TranslationMessage,
    yesCallback?: () => void | boolean,
    noCallback?: () => void | boolean,
    closeCallback?: () => void,
    width?: number
  ): DialogRef {
    const title: TranslationMessage = new TranslationMessage(this.globalTranslator, 'Confirmation');
    const actions: IModalAction[] = [
      { text: new TranslationMessage(this.globalTranslator, 'Yes'), callback: yesCallback },
      { text: new TranslationMessage(this.globalTranslator, 'No'), callback: noCallback }
    ];
    if (width) {
      const newSettings: DialogSettings = new DialogSettings();
      newSettings.title = title && title.toString();
      newSettings.content = content.toString();
      newSettings.actions = actions;
      newSettings.width = width;
      return this.showMessage(title, content, actions, closeCallback, newSettings);
    }
    return this.showMessage(title, content, actions, closeCallback);
  }

  public showInformationMessage(content: string | TranslationMessage, okCallback?: () => void, closeCallback?: () => void): DialogRef {
    const title: TranslationMessage = new TranslationMessage(this.globalTranslator, 'Information');
    const actions: IModalAction[] = [{ text: new TranslationMessage(this.globalTranslator, 'OK'), primary: true, callback: okCallback }];

    return this.showMessage(title, content, actions, closeCallback);
  }

  public showSuccessMessage(content: string | TranslationMessage, okCallback?: () => void, closeCallback?: () => void): DialogRef {
    const title: TranslationMessage = new TranslationMessage(this.globalTranslator, 'Success');
    const actions: IModalAction[] = [{ text: new TranslationMessage(this.globalTranslator, 'OK'), primary: true, callback: okCallback }];

    return this.showMessage(title, content, actions, closeCallback);
  }

  public showErrorMessage(content: string | TranslationMessage, okCallback?: () => void, closeCallback?: () => void): DialogRef {
    const title: TranslationMessage = new TranslationMessage(this.globalTranslator, 'Error');
    const actions: IModalAction[] = [{ text: new TranslationMessage(this.globalTranslator, 'OK'), primary: true, callback: okCallback }];

    return this.showMessage(title, content, actions, closeCallback);
  }

  public showWarningMessage(content: string | TranslationMessage, okCallback?: () => void, closeCallback?: () => void): DialogRef {
    const title: TranslationMessage = new TranslationMessage(this.globalTranslator, 'Warning');
    const actions: IModalAction[] = [{ text: new TranslationMessage(this.globalTranslator, 'OK'), primary: true, callback: okCallback }];

    return this.showMessage(title, content, actions, closeCallback);
  }

  public showMessage(
    title: string | TranslationMessage | null,
    content: string | TranslationMessage,
    actions: IModalAction[],
    closeCallback?: () => void,
    settings: DialogSettings = new DialogSettings()
  ): DialogRef {
    settings.title = title && title.toString();
    settings.content = content.toString();
    settings.actions = actions;
    settings.minWidth = 250;

    const ref: DialogRef = this.dialogService.open(settings);

    ref.result.subscribe((result: IModalAction) => {
      if (result.callback) {
        result.callback();
      }

      if (result instanceof DialogCloseResult && closeCallback) {
        closeCallback();
      }
    });

    return ref;
  }
}
