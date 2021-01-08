import { BaseComponent, FileUploaderSetting, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import {
  ECertificateParamType,
  ECertificateRepository,
  ECertificateSupportedField,
  ECertificateTemplateParam,
  IECertificateTemplateParam,
  IGetPreviewECertificateTemplateRequest,
  UserInfoModel
} from '@opal20/domain-api';

import { ECertificateDetailComponent } from './ecertificate-detail.component';
import { ECertificateTemplateDetailMode } from './../../models/ecertificate-template-detail-mode.model';
import { ECertificateTemplateDetailViewModel } from './../../view-models/ecertificate-template-detail-view.model';
import { FormGroup } from '@angular/forms';
import { OpalDialogService } from '@opal20/common-components';
import { PreviewEcertificateTemplateDialogComponent } from '../preview-ecertificate-template-dialog/preview-ecertificate-template-dialog.component';
import { formatDate } from '@angular/common';

@Component({
  selector: 'ecertificate-template-customise',
  templateUrl: './ecertificate-template-customise.component.html'
})
export class ECertificateTemplateCustomiseComponent extends BaseComponent {
  @Input() public form: FormGroup;
  public get eCertificateTemplate(): ECertificateTemplateDetailViewModel {
    return this._eCertificateTemplate;
  }
  @Input()
  public set eCertificateTemplate(v: ECertificateTemplateDetailViewModel) {
    this._eCertificateTemplate = v;
    this.fileUploaderSetting.extensions = this._eCertificateTemplate.allowedThumbnailExtensions;
  }
  @Input() public mode: ECertificateTemplateDetailMode | undefined;
  public fileUploaderSetting: FileUploaderSetting;
  public ECertificateParamType: typeof ECertificateParamType = ECertificateParamType;
  public ECertificateSupportedField: typeof ECertificateSupportedField = ECertificateSupportedField;

  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();
  private _eCertificateTemplate: ECertificateTemplateDetailViewModel;
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private opalDialogService: OpalDialogService,
    private eCertificateRepository: ECertificateRepository
  ) {
    super(moduleFacadeService);
    this.fileUploaderSetting = new FileUploaderSetting();
  }

  public asViewMode(): boolean {
    return ECertificateDetailComponent.asViewMode(this.mode);
  }

  public onPreview(): void {
    if (this.eCertificateTemplate.getTemplateParam(ECertificateSupportedField.Background)) {
      const fullName = this.currentUser.fullName;
      this.eCertificateRepository
        .getECertificateLayoutById(this.eCertificateTemplate.selectedLayout.id)
        .pipe(this.untilDestroy())
        .subscribe(data => {
          const ecertificateLayout = data;
          const ecertificateLayoutParams = ecertificateLayout.params;
          const templateParamDict = this.eCertificateTemplate.eCertificateTemplateData.paramDict;
          const previewTemplateParamDict: Dictionary<IECertificateTemplateParam> = {};
          ecertificateLayoutParams.forEach(layoutParam => {
            const key = layoutParam.key;
            previewTemplateParamDict[key] = new ECertificateTemplateParam(<IECertificateTemplateParam>{
              key: key,
              value: templateParamDict[key] ? templateParamDict[key].value : ''
            });
          });
          previewTemplateParamDict[ECertificateSupportedField.FullName].value = fullName;
          previewTemplateParamDict[ECertificateSupportedField.CourseName].value = 'Course Name';
          previewTemplateParamDict[ECertificateSupportedField.CompletedDate].value = formatDate(new Date(), 'dd/MM/yyyy', 'en-sg');

          this.opalDialogService.openDialogRef(
            PreviewEcertificateTemplateDialogComponent,
            {
              getPreviewECertificateTemplate: <IGetPreviewECertificateTemplateRequest>{
                eCertificateLayoutId: this.eCertificateTemplate.selectedLayout.id,
                params: Object.keys(previewTemplateParamDict).map(p => previewTemplateParamDict[p])
              }
            },
            {
              maxWidth: '100vw',
              maxHeight: '100vh',
              width: '100vw',
              height: '100vh',
              borderRadius: '0'
            }
          );
        });
    } else {
      this.opalDialogService.openConfirmDialog({
        confirmTitle: 'Warning',
        confirmMsg: 'You must select a background.',
        yesBtnText: 'OK',
        hideNoBtn: true
      });
    }
  }
}
