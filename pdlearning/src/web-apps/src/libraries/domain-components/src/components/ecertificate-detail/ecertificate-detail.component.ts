import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { ECertificateTemplateDetailMode } from './../../models/ecertificate-template-detail-mode.model';
import { ECertificateTemplateDetailViewModel } from './../../view-models/ecertificate-template-detail-view.model';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'ecertificate-detail',
  templateUrl: './ecertificate-detail.component.html'
})
export class ECertificateDetailComponent extends BaseComponent {
  @Input() public form: FormGroup;
  @Input() public mode: ECertificateTemplateDetailMode;

  public get eCertificateTemplate(): ECertificateTemplateDetailViewModel {
    return this._eCertificateTemplate;
  }

  @Input()
  public set eCertificateTemplate(v: ECertificateTemplateDetailViewModel) {
    this._eCertificateTemplate = v;
  }

  public loadingData: boolean = false;
  private _eCertificateTemplate: ECertificateTemplateDetailViewModel;

  public static asViewMode(mode: ECertificateTemplateDetailMode): boolean {
    return mode === ECertificateTemplateDetailMode.View;
  }

  constructor(public moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }
}
