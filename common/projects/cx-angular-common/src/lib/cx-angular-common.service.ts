import { Injectable } from '@angular/core';

/* You can put the necessary thing for your function in here
  This file will hold the settings for the whole life cycle in common */
@Injectable({
  providedIn: 'root'
})
export class CxCommonService {
  // tslint:disable-next-line:variable-name
  private _textMandatoryNeedToShow: string;
  public get textMandatoryNeedToShow(): string {
    return this._textMandatoryNeedToShow;
  }
  public set textMandatoryNeedToShow(value: string) {
    this._textMandatoryNeedToShow = value;
  }
  constructor() { }

}
