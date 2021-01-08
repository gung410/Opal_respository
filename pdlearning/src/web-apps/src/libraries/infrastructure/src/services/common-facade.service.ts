import { AppInfoService } from '../app-info/app-info.service';
import { GlobalSpinnerService } from '../spinner/global-spinner.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ModalService } from './modal.service';

@Injectable()
export class CommonFacadeService {
  constructor(
    public globalSpinner: GlobalSpinnerService,
    public globalModalService: ModalService,
    public appInfoService: AppInfoService,
    public http: HttpClient
  ) {}
}
