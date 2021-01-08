import { BasePageComponent, IGridFilter, ModuleFacadeService, NotificationType, TranslationMessage } from '@opal20/infrastructure';
import {
  CAMRoutePaths,
  CAMTabConfiguration,
  ContextMenuAction,
  ContextMenuEmit,
  ECertificateTemplateDetailMode,
  ListRegistrationGridDisplayColumns,
  NavigationPageService,
  RegistrationViewModel,
  RouterPageInput
} from '@opal20/domain-components';
import { Component, HostBinding } from '@angular/core';
import { ContextMenuItem, OpalDialogService } from '@opal20/common-components';
import {
  ECertificateRepository,
  ECertificateTemplateModel,
  SearchECertificateType,
  SearchRegistrationsType,
  UserInfoModel
} from '@opal20/domain-api';

import { ECertificateManagementPageInput } from '../models/ecertificate-management-page-input.model';
import { NAVIGATORS } from '../cam.config';
import { RegistrationECertificateDialogComponent } from './dialogs/registration-ecertificate-dialog.component';
import { SelectEvent } from '@progress/kendo-angular-layout';

@Component({
  selector: 'ecertificate-management-page',
  templateUrl: './ecertificate-management-page.component.html'
})
export class ECertificateManagementPageComponent extends BasePageComponent {
  public SearchECertificateType: typeof SearchECertificateType = SearchECertificateType;
  public searchTextForECertificate: string = '';
  public searchTextForIssuanceTracking: string = '';
  public filterData: unknown = null;
  public filterForECertificate: IGridFilter = {
    search: '',
    filter: null
  };

  public filterForIssuanceTracking: IGridFilter = {
    search: '',
    filter: null
  };

  public eCertificateManagementPageInput: RouterPageInput<ECertificateManagementPageInput, CAMTabConfiguration, unknown> = NAVIGATORS[
    CAMRoutePaths.CoursePlanningPage
  ] as RouterPageInput<ECertificateManagementPageInput, CAMTabConfiguration, unknown>;
  public CAMTabConfiguration: typeof CAMTabConfiguration = CAMTabConfiguration;

  public contextMenuItemsForECertificateTemplates: ContextMenuItem[] = [
    {
      id: ContextMenuAction.Delete,
      text: this.translateCommon('Delete'),
      icon: 'delete'
    }
  ];

  public SearchRegistrationsType: typeof SearchRegistrationsType = SearchRegistrationsType;
  public displayColumnsIssuanceTracking: ListRegistrationGridDisplayColumns[] = [
    ListRegistrationGridDisplayColumns.name,
    ListRegistrationGridDisplayColumns.organisation,
    ListRegistrationGridDisplayColumns.serviceScheme,
    ListRegistrationGridDisplayColumns.teachingLevel,
    ListRegistrationGridDisplayColumns.teachingSubjectJobFamily,
    ListRegistrationGridDisplayColumns.accountType,
    ListRegistrationGridDisplayColumns.designation,
    ListRegistrationGridDisplayColumns.courseJoined,
    ListRegistrationGridDisplayColumns.eCertificateIssuanceDate
  ];

  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();
  public get selectedTab(): CAMTabConfiguration {
    return this.eCertificateManagementPageInput.activeTab != null
      ? this.eCertificateManagementPageInput.activeTab
      : CAMTabConfiguration.AllECertificateTab;
  }

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public navigationPageService: NavigationPageService,
    private eCertificateRepository: ECertificateRepository,
    private opalDialogService: OpalDialogService
  ) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public onTabSelected(event: SelectEvent): void {
    this.eCertificateManagementPageInput.activeTab = eCertificateManagementPageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.eCertificateManagementPageInput);
  }

  public onSubmitSearchForECertificate(): void {
    this.filterForECertificate = {
      ...this.filterForECertificate,
      search: this.searchTextForECertificate
    };
  }

  public onSubmitSearchForIssuanceTracking(): void {
    this.filterForIssuanceTracking = {
      ...this.filterForIssuanceTracking,
      search: this.searchTextForIssuanceTracking
    };
  }

  public resetFilterForECertificate(): void {
    this.filterForECertificate = {
      ...this.filterForECertificate,
      search: this.searchTextForECertificate
    };
  }

  public resetFilterForIssuanceTracking(): void {
    this.filterForIssuanceTracking = {
      ...this.filterForIssuanceTracking,
      search: this.searchTextForIssuanceTracking
    };
  }

  public canCreateECertificate(): boolean {
    return (
      this.currentUser &&
      ECertificateTemplateModel.canCreateOrModify(this.currentUser) &&
      this.selectedTab === CAMTabConfiguration.AllECertificateTab
    );
  }

  public onCreateECertificate(): void {
    this.navigationPageService.navigateTo(
      CAMRoutePaths.ECertificateTemplateDetailPage,
      {
        activeTab: CAMTabConfiguration.ECertificateInfoTab,
        data: {
          mode: ECertificateTemplateDetailMode.NewECertificate
        }
      },
      this.eCertificateManagementPageInput
    );
  }

  public selectedContextMenuECertificateTemplateList(contextMenuEmit: ContextMenuEmit<ECertificateTemplateModel>): void {
    switch (contextMenuEmit.event.item.id) {
      case ContextMenuAction.Delete:
        this.deleteECertificateTemplate(contextMenuEmit.dataItem);
        break;
      default:
        break;
    }
  }

  public onViewECertificateTemplate(data: ECertificateTemplateModel): void {
    this.navigationPageService.navigateTo(
      CAMRoutePaths.ECertificateTemplateDetailPage,
      {
        activeTab: CAMTabConfiguration.ECertificateInfoTab,
        data: {
          mode: ECertificateTemplateDetailMode.View,
          id: data.id
        }
      },
      this.eCertificateManagementPageInput
    );
  }

  public onViewIssuanceTracking(dataItem: RegistrationViewModel): void {
    this.opalDialogService.openDialogRef(
      RegistrationECertificateDialogComponent,
      {
        registrationId: dataItem.id
      },
      { width: '100vw', height: '100vh', maxWidth: '100vw', maxHeight: '100vh', borderRadius: '0' }
    );
  }

  protected onInit(): void {
    this.getNavigatePageData();
  }

  private deleteECertificateTemplate(eCertificateTemplate: ECertificateTemplateModel): void {
    this.modalService.showConfirmMessage(
      new TranslationMessage(this.moduleFacadeService.globalTranslator, 'Are you sure you want to delete this e-certificate template?'),
      () => {
        this.eCertificateRepository.deleteECertificateTemplate(eCertificateTemplate.id).then(() => {
          this.showNotification(`${eCertificateTemplate.title} is successfully deleted`, NotificationType.Success);
        });
      }
    );
  }

  private getNavigatePageData(): void {
    const navigateData: RouterPageInput<ECertificateManagementPageInput, CAMTabConfiguration, unknown> = this.getNavigateData();
    if (navigateData) {
      this.eCertificateManagementPageInput = navigateData;
    } else {
      this.navigationPageService.returnHome();
    }
  }
}

export const eCertificateManagementPageTabIndexMap = {
  0: CAMTabConfiguration.AllECertificateTab,
  1: CAMTabConfiguration.IssuanceTrackingTab
};
