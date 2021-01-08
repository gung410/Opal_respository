import {
  BaseFormComponent,
  IFormBuilderDefinition,
  IntervalScheduler,
  ModuleFacadeService,
  NotificationType,
  TranslationMessage,
  Utils
} from '@opal20/infrastructure';
import {
  BreadcrumbItem,
  BreadcrumbService,
  CAMRoutePaths,
  CAMTabConfiguration,
  ContextMenuAction,
  ECertificateTemplateDetailMode,
  ECertificateTemplateDetailViewModel,
  NavigationData,
  NavigationPageService,
  RouterPageInput
} from '@opal20/domain-components';
import {
  ButtonAction,
  ButtonGroupButton,
  ContextMenuItem,
  OpalDialogService,
  SPACING_CONTENT,
  requiredAndNoWhitespaceValidator
} from '@opal20/common-components';
import { Component, HostBinding, ViewChild } from '@angular/core';
import {
  ECertificateRepository,
  ECertificateTemplateModel,
  ECertificateTemplateStatus,
  ISaveECertificateTemplateRequest,
  SearchRegistrationsType,
  UserInfoModel
} from '@opal20/domain-api';
import { Observable, Subscription, combineLatest, from, of } from 'rxjs';
import { SelectEvent, TabStripComponent } from '@progress/kendo-angular-layout';

import { CAM_ROUTE_BREADCUMB_MAPPING_FN } from '../helpers/cam-route-breadcumb-mapping-fn';
import { ECertificateTemplateDetailPageInput } from '../models/ecertificate-template-detail-input.model';
import { NAVIGATORS } from '../cam.config';
import { Validators } from '@angular/forms';

@Component({
  selector: 'ecertificate-template-detail-page',
  templateUrl: './ecertificate-template-detail-page.component.html'
})
export class ECertificateTemplateDetailPageComponent extends BaseFormComponent {
  @ViewChild(TabStripComponent, { static: false }) public tabStrip: TabStripComponent;
  public breadCrumbItems: BreadcrumbItem[] = [];
  public actionBtnGroup: ButtonAction<unknown>[] = [];
  public stickySpacing: number = SPACING_CONTENT;

  public get title(): string {
    return this.eCertificateTemplate.title;
  }

  public get detailPageInput(): RouterPageInput<ECertificateTemplateDetailPageInput, CAMTabConfiguration, unknown> | undefined {
    return this._detailPageInput;
  }

  public set detailPageInput(v: RouterPageInput<ECertificateTemplateDetailPageInput, CAMTabConfiguration, unknown> | undefined) {
    if (Utils.isDifferent(this._detailPageInput, v) && v) {
      this._detailPageInput = v;
      if (this.initiated) {
        this.loadECertificateInfo();
      }
    }
  }

  public get eCertificateTemplate(): ECertificateTemplateDetailViewModel {
    return this._eCertificateTemplate;
  }
  public set eCertificateTemplate(v: ECertificateTemplateDetailViewModel) {
    this._eCertificateTemplate = v;
  }

  public get getContextMenuByECertificateTemplate(): ContextMenuItem[] {
    return this.contextMenuItemsForECertificate.filter(
      item =>
        this.eCertificateTemplate.eCertificateTemplateData.canDeleteECertificateTemplate(this.currentUser) &&
        item.id === ContextMenuAction.Delete &&
        this.selectedTab === CAMTabConfiguration.CourseInfoTab
    );
  }

  public contextMenuItemsForECertificate: ContextMenuItem[] = [
    {
      id: ContextMenuAction.Delete,
      text: this.translateCommon('Delete'),
      icon: 'delete'
    }
  ];

  public buttonGroup: Partial<ButtonGroupButton>[] = [
    {
      displayText: 'Edit',
      onClickFn: () => this.onEditECertificate(),
      shownIfFn: () => this.showEditECertificate()
    },
    {
      displayText: 'Save',
      onClickFn: () => this.onSaveClicked(),
      shownIfFn: () => this.showSubmitECertificate(),
      isDisabledFn: () => !this.dataHasChanged()
    },
    {
      displayText: 'Submit',
      onClickFn: () => this.onSubmitClicked()
    },
    {
      id: ContextMenuAction.Delete,
      icon: 'delete',
      displayText: 'Delete',
      shownInMoreFn: () => true,
      onClickFn: () => {
        this.modalService.showConfirmMessage(
          new TranslationMessage(this.moduleFacadeService.globalTranslator, 'Are you sure you want to delete this e-certificate template?'),
          () => {
            this.eCertificateRepository.deleteECertificateTemplate(this.detailPageInput.data.id).then(() => {
              this.showNotification(`${this.title} is successfully deleted`, NotificationType.Success);
              this.navigationPageService.navigateBack();
            });
          }
        );
      },
      shownIfFn: () =>
        this.eCertificateTemplate.eCertificateTemplateData.canDeleteECertificateTemplate(this.currentUser) &&
        this.selectedTab === CAMTabConfiguration.ECertificateInfoTab
    }
  ];

  public navigationData: NavigationData;
  public loadingData: boolean = false;
  public registrationStatusChanged: boolean = false;
  public SearchRegistrationsType: typeof SearchRegistrationsType = SearchRegistrationsType;
  public CAMTabConfiguration: typeof CAMTabConfiguration = CAMTabConfiguration;
  private _detailPageInput: RouterPageInput<ECertificateTemplateDetailPageInput, CAMTabConfiguration, unknown> = NAVIGATORS[
    CAMRoutePaths.ECertificateTemplateDetailPage
  ] as RouterPageInput<ECertificateTemplateDetailPageInput, CAMTabConfiguration, unknown>;
  private _loadECertificateTemplateInfoSub: Subscription = new Subscription();
  private _eCertificateTemplate: ECertificateTemplateDetailViewModel = new ECertificateTemplateDetailViewModel();
  private currentUser = UserInfoModel.getMyUserInfo();
  // Auto save after 30 minutes
  private scheduler: IntervalScheduler = new IntervalScheduler(600000, () => {
    if (this.dataHasChanged()) {
      this.saveDraftECertificateTemplate();
    }
  });

  public get selectedTab(): CAMTabConfiguration {
    return this.detailPageInput.activeTab != null ? this.detailPageInput.activeTab : CAMTabConfiguration.ECertificateInfoTab;
  }

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private eCertificateRepository: ECertificateRepository,
    private navigationPageService: NavigationPageService,
    private breadcrumbService: BreadcrumbService,
    private opalDialogService: OpalDialogService
  ) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public onTabSelected(event: SelectEvent): void {
    this.detailPageInput.activeTab = eCertificateDetailPageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.detailPageInput);
  }

  public loadECertificateInfo(): void {
    this._loadECertificateTemplateInfoSub.unsubscribe();
    const eCertificateTemplateObs =
      this.detailPageInput.data.id != null
        ? this.eCertificateRepository.getECertificateTemplateById(this.detailPageInput.data.id)
        : of(null);
    const eCertificateLayoutObs = this.eCertificateRepository.getECertificateLayouts();
    this.loadingData = true;
    this._loadECertificateTemplateInfoSub = combineLatest(eCertificateTemplateObs, eCertificateLayoutObs)
      .pipe(this.untilDestroy())
      .subscribe(
        ([eCertificateTemplate, eCertificateLayouts]) => {
          this.eCertificateTemplate = new ECertificateTemplateDetailViewModel(eCertificateTemplate, eCertificateLayouts);
          this.loadBreadcrumb();
          this.loadingData = false;
        },
        () => {
          this.loadingData = false;
        }
      );
  }

  public onClickBack(): void {
    this.navigationPageService.navigateBack(
      () => this.dataHasChanged(),
      () => this.validateAndSaveECertificate(this.eCertificateTemplate.status)
    );
  }

  public showSubmitECertificate(): boolean {
    return (
      this.detailPageInput.data.mode === ECertificateTemplateDetailMode.Edit ||
      (this.detailPageInput.data.mode === ECertificateTemplateDetailMode.NewECertificate &&
        this.selectedTab === CAMTabConfiguration.ECertificateInfoTab)
    );
  }

  public showEditECertificate(): boolean {
    return (
      this.detailPageInput.data.mode === ECertificateTemplateDetailMode.View && this.selectedTab === CAMTabConfiguration.ECertificateInfoTab
    );
  }

  public onEditECertificate(): void {
    this.detailPageInput.data.mode = ECertificateTemplateDetailMode.Edit;
    this.tabStrip.selectTab(0);
  }

  public onSaveClicked(): void {
    this.subscribe(this.validateAndSaveECertificate(ECertificateTemplateStatus.Draft), _ => {
      this.navigationPageService.navigateBack();
    });
  }

  public onSubmitClicked(): void {
    this.validateAndSaveECertificate(ECertificateTemplateStatus.Active).subscribe(() => this.navigationPageService.navigateBack());
  }

  public validateAndSaveECertificate(status: ECertificateTemplateStatus): Observable<void> {
    return from(
      new Promise<void>((resolve, reject) => {
        this.validate(status === ECertificateTemplateStatus.Draft ? ['title'] : null).then(valid => {
          if (valid) {
            this.saveECertificate(status).then(_ => {
              this.showNotification();
              resolve();
            }, reject);
          } else {
            reject();
          }
        });
      })
    );
  }

  public saveDraftECertificateTemplate(): Observable<void> {
    return from(
      this.saveECertificate(ECertificateTemplateStatus.Draft).then(_ => {
        this.showNotification();
      })
    );
  }

  public dataHasChanged(): boolean {
    return this.eCertificateTemplate && this.eCertificateTemplate.dataHasChanged();
  }

  protected onInit(): void {
    this.getNavigatePageData();
    this.loadECertificateInfo();
    this.scheduler.init();
  }

  protected onDestroy(): void {
    this.scheduler.destroy();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'ecertificate-detail',
      controls: {
        eCertificateLayoutId: {
          defaultValue: null,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        },
        title: {
          defaultValue: null,
          validators: [
            {
              validator: requiredAndNoWhitespaceValidator(),
              validatorType: 'required'
            }
          ]
        },
        background: {
          defaultValue: null,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        }
      }
    };
  }

  private getNavigatePageData(): void {
    const navigateData: RouterPageInput<ECertificateTemplateDetailPageInput, CAMTabConfiguration, unknown> = this.getNavigateData();
    if (navigateData) {
      this.detailPageInput = navigateData;
    } else {
      this.navigationPageService.returnHome();
    }
  }

  private saveECertificate(status: ECertificateTemplateStatus): Promise<ECertificateTemplateModel> {
    return new Promise((resolve, reject) => {
      const templateParamDic = Utils.toDictionarySelect(this.eCertificateTemplate.params, x => x.key, x => x);

      const params = [];
      this.eCertificateTemplate.selectedLayout.params.forEach(param => {
        if (param.isAutoPopulated === false) {
          params.push(templateParamDic[param.key]);
        }
      });

      const request: ISaveECertificateTemplateRequest = {
        data: Utils.clone(this.eCertificateTemplate.eCertificateTemplateData, cloneData => {
          cloneData.status = status;
          cloneData.params = params;
        })
      };
      this.eCertificateRepository
        .saveECertificateTemplate(request)
        .pipe(this.untilDestroy())
        .subscribe(session => {
          resolve(session);
        }, reject);
    });
  }

  private loadBreadcrumb(): void {
    this.breadCrumbItems = this.breadcrumbService.loadBreadcrumbTab(
      this.detailPageInput,
      CAM_ROUTE_BREADCUMB_MAPPING_FN(
        this.detailPageInput,
        p =>
          this.navigationPageService.navigateByRouter(
            p,
            () => this.dataHasChanged(),
            () => this.validateAndSaveECertificate(this.eCertificateTemplate.status)
          ),
        {
          [CAMRoutePaths.ECertificateTemplateDetailPage]: { textFn: () => this.eCertificateTemplate.title }
        }
      )
    );
  }
}

export const eCertificateDetailPageTabIndexMap = {
  0: CAMTabConfiguration.ECertificateInfoTab
};
