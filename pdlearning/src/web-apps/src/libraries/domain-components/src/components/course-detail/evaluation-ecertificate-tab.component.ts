import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import {
  ECertificateRepository,
  ECertificateSupportedField,
  ECertificateTemplateModel,
  ECertificateTemplateParam,
  IECertificateTemplateParam,
  IGetPreviewECertificateTemplateRequest,
  MetadataId,
  SearchECertificateType,
  UserInfoModel
} from '@opal20/domain-api';
import { Observable, Subscription } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { CourseDetailComponent } from './course-detail.component';
import { CourseDetailMode } from '../../models/course-detail-mode.model';
import { CourseDetailViewModel } from '../../view-models/course-detail-view.model';
import { FormGroup } from '@angular/forms';
import { OpalDialogService } from '@opal20/common-components';
import { PreviewEcertificateTemplateDialogComponent } from '../preview-ecertificate-template-dialog/preview-ecertificate-template-dialog.component';
import { PreviewFormDialogComponent } from '../preview-form-dialog/preview-form-dialog.component';
import { formatDate } from '@angular/common';

@Component({
  selector: 'evaluation-ecertificate-tab',
  templateUrl: './evaluation-ecertificate-tab.component.html'
})
export class EvaluationEcertificateTabComponent extends BaseComponent {
  @Input() public form: FormGroup;
  @Input() public course: CourseDetailViewModel;
  @Input() public mode: CourseDetailMode | undefined;
  public fetchEcertificateTemplatesFn: (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<ECertificateTemplateModel[]> = null;
  public showPreviewEcertificate: boolean = false;
  public showPreviewPostForm: boolean = false;

  public MetadataId: typeof MetadataId = MetadataId;
  public CourseDetailMode: typeof CourseDetailMode = CourseDetailMode;
  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();
  private eCertificateLayoutSubs: Subscription = new Subscription();
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private opalDialogService: OpalDialogService,
    private eCertificateRepository: ECertificateRepository
  ) {
    super(moduleFacadeService);
    this.fetchEcertificateTemplatesFn = this._createFetchEcertificateTemplatesFn();
  }

  public asViewMode(): boolean {
    return CourseDetailComponent.asViewMode(this.mode);
  }

  public isPlanningVerificationRequired(): boolean {
    return CourseDetailComponent.isPlanningVerificationRequired(this.course);
  }

  public onPostCourseFormSelectItemPreviewBtnClicked(e: Event, formId: string): void {
    e.stopImmediatePropagation();
    this.showPreviewPostForm = true;
    const dialogRef = this.opalDialogService.openDialogRef(
      PreviewFormDialogComponent,
      {
        formId: formId
      },
      {
        maxWidth: '100vw',
        maxHeight: '100vh',
        width: '100vw',
        height: '100vh',
        borderRadius: '0'
      }
    );

    this.subscribe(dialogRef.result, () => {
      this.showPreviewPostForm = false;
    });
  }

  public onEcertificateSelectItemPreviewBtnClicked(e: Event, ecertificateTemplateId: string): void {
    e.stopImmediatePropagation();
    const ecertificateTemplate = this.course.eCertificateTemplateDic[ecertificateTemplateId];
    this.showPreviewEcertificate = true;
    this.eCertificateLayoutSubs.unsubscribe();
    this.eCertificateLayoutSubs = this.eCertificateRepository
      .getECertificateLayoutById(ecertificateTemplate.eCertificateLayoutId)
      .pipe(
        this.untilDestroy(),
        switchMap(data => {
          const ecertificateLayout = data;
          const ecertificateLayoutParams = ecertificateLayout.params;
          const templateParamDict = ecertificateTemplate.paramDict;
          const previewTemplateParamDict: Dictionary<IECertificateTemplateParam> = {};
          ecertificateLayoutParams.forEach(layoutParam => {
            const key = layoutParam.key;
            previewTemplateParamDict[key] = new ECertificateTemplateParam(<IECertificateTemplateParam>{
              key: key,
              value: templateParamDict[key] ? templateParamDict[key].value : ''
            });
          });
          previewTemplateParamDict[ECertificateSupportedField.FullName].value = this.currentUser.fullName;
          previewTemplateParamDict[ECertificateSupportedField.CourseName].value = this.course.courseNameInECertificate
            ? this.course.courseNameInECertificate
            : this.course.courseName.substring(0, 100);
          previewTemplateParamDict[ECertificateSupportedField.CompletedDate].value = formatDate(new Date(), 'dd/MM/yyyy', 'en-sg');
          previewTemplateParamDict[ECertificateSupportedField.Principal].value =
            previewTemplateParamDict[ECertificateSupportedField.Principal].value !== ''
              ? previewTemplateParamDict[ECertificateSupportedField.Principal].value
              : this.course.courseFacilitatorId
              ? this.course.courseFacilitatorItems.find(p => p.id === this.course.courseFacilitatorId).fullName
              : '';

          const dialogRef = this.opalDialogService.openDialogRef(
            PreviewEcertificateTemplateDialogComponent,
            {
              getPreviewECertificateTemplate: <IGetPreviewECertificateTemplateRequest>{
                eCertificateLayoutId: ecertificateTemplate.eCertificateLayoutId,
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
          return dialogRef.result;
        })
      )
      .subscribe(
        () => {
          this.showPreviewEcertificate = false;
        },
        () => {
          this.showPreviewEcertificate = false;
        }
      );
  }

  public asViewModeForCompletingCourseForPlanning(): boolean {
    return CourseDetailComponent.asViewModeForCompletingCourseForPlanning(this.course);
  }

  public displayPreviewEcertificate(): boolean {
    return (
      this.course.eCertificateTemplateId &&
      this.course.eCertificateTemplateDic[this.course.eCertificateTemplateId] != null &&
      this.asViewMode()
    );
  }

  public displayPreviewForm(): boolean {
    return (
      this.course.postCourseEvaluationFormId &&
      this.course.postCourseFormsDic[this.course.postCourseEvaluationFormId] != null &&
      this.asViewMode() &&
      this.course.courseData.hasViewPostCourseEvaluationPermission(this.currentUser)
    );
  }

  private _createFetchEcertificateTemplatesFn(): (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<ECertificateTemplateModel[]> {
    return (searchText: string, skipCount: number, maxResultCount: number) =>
      this.eCertificateRepository
        .searchECertificateTemplates(searchText, SearchECertificateType.CourseSelection, skipCount, maxResultCount)
        .pipe(map(data => data.items));
  }
}
