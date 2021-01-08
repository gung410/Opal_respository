import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { Subscription, from } from 'rxjs';

import { DialogAction } from '@opal20/common-components';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { RegistrationApiService } from '@opal20/domain-api';
import { RegistrationECertificateViewModel } from '../../models/registration-ecertificate-view.model';

@Component({
  selector: 'registration-ecertificate-dialog',
  templateUrl: './registration-ecertificate-dialog.component.html'
})
export class RegistrationECertificateDialogComponent extends BaseComponent {
  public _registrationId: string;
  public get registrationId(): string {
    return this._registrationId;
  }
  @Input()
  public set registrationId(v: string) {
    this._registrationId = v;
    if (this.initiated) {
      this.loadData();
    }
  }

  public loading: boolean = false;
  public vm: RegistrationECertificateViewModel = new RegistrationECertificateViewModel();

  private _loadDataSub: Subscription = new Subscription();

  constructor(moduleFacadeService: ModuleFacadeService, private registrationApiSvc: RegistrationApiService, private dialogRef: DialogRef) {
    super(moduleFacadeService);
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();

    this.loading = true;
    this._loadDataSub = from(this.registrationApiSvc.getRegistrationCertificateById(this.registrationId))
      .pipe(this.untilDestroy())
      .subscribe(
        data => {
          this.vm = new RegistrationECertificateViewModel({ registrationECertificate: data });
          this.loading = false;
        },
        error => {
          this.loading = false;
        },
        () => {
          this.loading = false;
        }
      );
  }

  public close(): void {
    this.dialogRef.close({ action: DialogAction.Close });
  }

  public downloadCertificate(): void {
    Utils.downloadPdfFromBase64(this.vm.registrationECertificate.base64Pdf, this.vm.registrationECertificate.pdfFileName);
  }

  protected onInit(): void {
    this.loadData();
  }
}
