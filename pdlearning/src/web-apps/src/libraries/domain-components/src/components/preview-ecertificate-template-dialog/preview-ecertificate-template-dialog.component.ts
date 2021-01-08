import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, HostBinding, Input } from '@angular/core';
import { ECertificateApiService, IGetPreviewECertificateTemplateRequest, PreviewECertificateTemplateModel } from '@opal20/domain-api';

import { DialogAction } from '@opal20/common-components';
import { DialogRef } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'preview-ecertificate-template-dialog',
  templateUrl: './preview-ecertificate-template-dialog.component.html'
})
export class PreviewEcertificateTemplateDialogComponent extends BaseComponent {
  @Input() public getPreviewECertificateTemplate: IGetPreviewECertificateTemplateRequest;

  @HostBinding('class.preview-content')
  public getPreviewContentClassName(): boolean {
    return true;
  }

  public previewECertificateTemplateModel: PreviewECertificateTemplateModel = new PreviewECertificateTemplateModel();
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public dialogRef: DialogRef,
    public eCertificateApiService: ECertificateApiService
  ) {
    super(moduleFacadeService);
  }

  public closePreview(): void {
    this.dialogRef.close({ action: DialogAction.Close });
  }

  protected onInit(): void {
    this.loadData();
  }

  private loadData(): void {
    this.eCertificateApiService.getPreviewECertificateTemplate(this.getPreviewECertificateTemplate).then(data => {
      this.previewECertificateTemplateModel = data;
    });
  }
}
