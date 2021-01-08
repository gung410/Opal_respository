import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { ECertificateApiService } from '@opal20/domain-api';
import { ECertificateDetailComponent } from './ecertificate-detail.component';
import { ECertificateTemplateDetailMode } from './../../models/ecertificate-template-detail-mode.model';
import { ECertificateTemplateDetailViewModel } from './../../view-models/ecertificate-template-detail-view.model';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'ecertificate-layout-selection',
  templateUrl: './ecertificate-layout-selection.component.html'
})
export class ECertificateLayoutSelectionComponent extends BaseComponent {
  @Input() public form: FormGroup;
  @Input() public eCertificateTemplate: ECertificateTemplateDetailViewModel;
  @Input() public mode: ECertificateTemplateDetailMode | undefined;

  public base64PreviewImage: string;
  constructor(public moduleFacadeService: ModuleFacadeService, public eCertificateApiService: ECertificateApiService) {
    super(moduleFacadeService);
  }

  public asViewMode(): boolean {
    return ECertificateDetailComponent.asViewMode(this.mode);
  }

  public onSelectECertificate(eCertificateLayoutId: string): void {
    if (eCertificateLayoutId) {
      this.eCertificateTemplate.eCertificateLayoutId = eCertificateLayoutId;
      this.base64PreviewImage = this.eCertificateTemplate.eCertificateLayoutDic[eCertificateLayoutId].base64PreviewImage;
    }
  }
}
