import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  ElementRef,
  HostListener,
  OnInit,
  ViewChild,
  ViewEncapsulation,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  CxGlobalLoaderService,
  CxNode,
  CxNodeComponent,
  CxTreeButtonCondition,
  CxTreeIcon,
} from '@conexus/cx-angular-common';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'app-auth/auth.service';
import { AssessmentStatusInfo } from 'app-models/assessment.model';
import { User } from 'app-models/auth.model';
import { CxBreadCrumbItem } from 'app-models/breadcrumb.model';
import {
  ILearningPlanPermission,
  LearningPlanPermission,
} from 'app-models/common/permission/learning-plan-permission';
import { PDPlanConfig, PDPlanDto } from 'app-models/pdplan.model';
import { BreadcrumbSettingService } from 'app-services/bread-crumb-setting.service';
import { CxSurveyjsExtendedService } from 'app-services/cx-surveyjs-extended.service';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { RouteConstant } from 'app/shared/app.constant';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import { SurveyjsMode } from 'app/shared/constants/surveyjs-mode.constant';
import { uniqueId } from 'lodash';
import { ToastrService } from 'ngx-toastr';
import {
  NodeStatusType,
  OdpActivity,
  OdpActivityName,
  OdpStatusCode,
} from '../learning-plan-detail/odp.constant';
import { LearningPlanContent } from '../models/learning-plan-content.model';
import { OdpService } from '../odp.service';

@Component({
  selector: 'overall-organizational-development',
  templateUrl: './overall-organizational-development.component.html',
  styleUrls: ['./overall-organizational-development.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OverallOrganizationalDevelopmentComponent
  extends BaseScreenComponent
  implements OnInit, ILearningPlanPermission {
  get canEdit(): boolean {
    // tslint:disable-next-line: no-unsafe-any
    const isDisplayMode =
      this.planNode.dataObject.formJSON &&
      this.planNode.dataObject.formJSON.mode === SurveyjsMode.display;
    const isSubmitting = this.checkPlanStatus(
      this.planNode.dataObject.pdplanDto,
      OdpStatusCode.PendingForApproval
    );

    return (
      isDisplayMode &&
      this.haveEditRight &&
      !isSubmitting &&
      !this.isPlanOfPreviousYears(this.planNode.dataObject.pdplanDto)
    );
  }
  get currentActivePlanTypeName(): string {
    if (!this.activePdplan || !this.activePdplan.pdplanDto) {
      return '';
    }

    return OdpActivityName[this.activePdplan.pdplanDto.pdPlanActivity];
  }
  @ViewChild('viewLeftSide') viewLeftSide: ElementRef;
  @ViewChild('viewDivider') viewDivider: ElementRef;
  @ViewChild('viewToggler') viewToggler: ElementRef;
  @ViewChild('viewRightSide') viewRightSide: ElementRef;
  @ViewChild('cxSplitView') cxSplitView: ElementRef;

  activePdplan: LearningPlanContent;
  activeParentPdplanDTO: PDPlanDto;
  planNode: CxNode;
  planFormJSON: any;
  directionFormJSON: any;
  programmeFormJSON: any;
  haveEditRight: boolean;
  odpContents: LearningPlanContent[];
  currentUserRoles: any = [];

  odpActivityName: any = OdpActivityName;
  odpButtonCondition: CxTreeButtonCondition<LearningPlanContent>;
  odpTreeIcon: CxTreeIcon = new CxTreeIcon({
    add: 'material-icons add',
    collapse: 'material-icons arrow-drop-down',
    expand: 'material-icons arrow-right',
  });
  itemTrackByFn: (index: number, item: any) => {};

  planCxNode: CxNode;
  selectedNodeComponent: CxNodeComponent;
  currentBreadcrumb: CxBreadCrumbItem[] = []; // Breadcrumb data

  // Permission
  learningPlanPermission: LearningPlanPermission;

  // Split view
  private hasMouseDownDivider: boolean = false;
  private flipClass: string = 'cx-flip';
  private transitionClass: string = 'cx-light-transition';
  private disableSelectText: string = 'disable-select-text';
  private currentNodeId: string;

  constructor(
    changeDetectorRef: ChangeDetectorRef,
    authService: AuthService,
    private toastrService: ToastrService,
    private odpService: OdpService,
    private route: ActivatedRoute,
    private router: Router,
    private breadcrumbSettingService: BreadcrumbSettingService,
    private globalLoader: CxGlobalLoaderService,
    private cxSurveyjsExtendedService: CxSurveyjsExtendedService,
    private translateService: TranslateService
  ) {
    super(changeDetectorRef, authService);
  }

  initLearningPlanPermissionn(loginUser: User): void {
    this.learningPlanPermission = new LearningPlanPermission(loginUser);
  }

  ngOnInit(): void {
    this.initLearningPlanPermissionn(this.currentUser);
    if (!this.learningPlanPermission.allowViewOverall) {
      this.router.navigate([RouteConstant.ERROR_ACCESS_DENIED]);
    } else {
      this.initData();

      this.currentNodeId = this.route.snapshot.params.extId;
      this.getLearningPlanFormAPI(this.currentNodeId);
      this.subscription.add(
        this.breadcrumbSettingService.changeBreadcrumbEvent.subscribe(
          this.breadcrumDataChangedHandler
        )
      );
      this.currentBreadcrumb = this.breadcrumbSettingService.currentBreadcrumb;
    }
  }

  initData(): void {
    this.haveEditRight = this.learningPlanPermission.allowEdit;
  }

  async getLearningPlanFormAPI(extID: string): Promise<void> {
    this.globalLoader.showLoader();
    // Get ODP config
    const odpConfigs: PDPlanConfig[] = await this.getODPConfig();
    if (!odpConfigs) {
      this.globalLoader.hideLoader();
      this.toastrService.error(`Can't get Organisational PD Journey configs!`);
      this.changeDetectorRef.detectChanges();

      return;
    }

    this.planFormJSON = odpConfigs.find(
      (config) => config.pdPlanActivity === OdpActivity.Plan
    ).configuration;
    this.directionFormJSON = odpConfigs.find(
      (config) => config.pdPlanActivity === OdpActivity.Direction
    ).configuration;
    this.programmeFormJSON = this.programmeFormJSON = odpConfigs.find(
      (config) => config.pdPlanActivity === OdpActivity.Programme
    ).configuration;

    this.setSurveyjsDisplayMode();

    // Get learning Plan result tree
    const planTreeArray: PDPlanDto[] = await this.getLearningPlanResultTree(
      extID
    );
    if (!planTreeArray || planTreeArray.length === 0) {
      this.globalLoader.hideLoader();
      this.toastrService.error(
        this.translateService.instant('Odp.LearningPlan.FailedToRetrieveData')
      );
      this.router.navigate([`/odp`]);
      this.changeDetectorRef.detectChanges();

      return;
    }

    const planTree = planTreeArray[0];
    this.convertDatetimeToSurveyFormat(planTree);
    this.planCxNode = this.toCxNode(planTree);
    this.changeDetectorRef.detectChanges();
  }

  onMouseDown(event: any): void {
    if (!this.cxSplitView) {
      return;
    }
    const DIVIDER_ID = 'divider';
    if (event.target && event.target.id === DIVIDER_ID) {
      if (
        this.cxSplitView &&
        !this.cxSplitView.nativeElement.classList.contains(
          this.disableSelectText
        )
      ) {
        this.cxSplitView.nativeElement.classList.add(this.disableSelectText);
      }
      this.hasMouseDownDivider = true;
    }
  }

  @HostListener('mouseup', ['$event'])
  onMouseUp(): void {
    if (!this.cxSplitView) {
      return;
    }
    this.hasMouseDownDivider = false;
    this.cxSplitView.nativeElement.classList.remove(this.disableSelectText);
  }

  @HostListener('mousemove', ['$event'])
  onMouseMove(event: any): void {
    if (!this.viewToggler || !this.viewLeftSide || !this.viewDivider) {
      return;
    }

    const viewToggler = this.viewToggler.nativeElement;
    const leftSide = this.viewLeftSide.nativeElement;

    if (this.hasMouseDownDivider && this.cxSplitView) {
      if (viewToggler.classList.contains(this.flipClass)) {
        return;
      }

      const leftSideWidth = this.calculateLeftSideWidth(event);
      leftSide.classList.remove(this.transitionClass);
      leftSide.style.width = leftSideWidth + 'px';
    }
  }
  onClickEdit(): void {
    this.router.navigate([
      `/odp/plan-detail/${this.planNode.dataObject.pdplanDto.resultIdentity.extId}`,
    ]);
  }

  onClickBreadcrumbItem(route: any): void {
    if (route && this.currentUser) {
      if (route && route.value) {
        this.router.navigate([route.value]);
      } else {
        this.router.navigate([route]);
      }
      const itemBreadcrumbs = this.breadcrumbSettingService.mapRouteToBreadcrumb(
        this.currentUser.headerData.menus,
        route
      );
      this.addItemToBreadcrumb(itemBreadcrumbs);
    }
  }

  onCxNodeReady(rootComponent: CxNodeComponent): void {
    if (!rootComponent) {
      return;
    }
    this.planNode = rootComponent.node;
    this.cxSurveyjsExtendedService.setCurrentObjectVariables(
      this.planNode.dataObject.pdplanDto
    );

    this.globalLoader.hideLoader();
    this.changeDetectorRef.detectChanges();
  }

  calculateLeftSideWidth(eventData: any): number {
    if (!this.cxSplitView || !this.viewLeftSide) {
      return;
    }

    const splitViewWidth = this.cxSplitView.nativeElement.offsetWidth;
    const splitViewPadding = 200;
    const minWidth = 300;
    const maxWidth = splitViewWidth - splitViewPadding;
    const leftSide = this.viewLeftSide.nativeElement;
    const boxRectangle = leftSide.getBoundingClientRect();
    let width = eventData.pageX - boxRectangle.left;

    width = width < minWidth ? minWidth : width;
    width = width > maxWidth ? maxWidth : width;

    return width;
  }

  /**
   * Check whether it is the plan of previous years or not.
   * @param plan The PD Plan dto.
   */
  private isPlanOfPreviousYears(plan: PDPlanDto): boolean {
    return (
      plan &&
      plan.surveyInfo &&
      plan.surveyInfo.endDate < new Date().toISOString()
    );
  }
  // Check learning plan, learning direction is have this status code
  private checkPlanStatus(plan: PDPlanDto, statusCode: string): boolean {
    if (
      plan.assessmentStatusInfo &&
      plan.assessmentStatusInfo.assessmentStatusCode === statusCode
    ) {
      return true;
    }
  }

  private convertDatetimeToSurveyFormat(pdplan: PDPlanDto): void {
    if (pdplan.answer.startDate) {
      pdplan.answer.startDate = DateTimeUtil.toSurveyFormat(
        pdplan.answer.startDate
      );
    }
    if (pdplan.answer.endDate) {
      pdplan.answer.endDate = DateTimeUtil.toSurveyFormat(
        pdplan.answer.endDate
      );
    }
  }

  private toCxNode(planNode: PDPlanDto, parent?: CxNode): CxNode {
    const cxNode: CxNode = this.convertNodeInfo(planNode, parent);
    const parentNode =
      planNode.pdPlanActivity === OdpActivity.Direction ? cxNode : undefined;
    if (planNode.children && planNode.children.length > 0) {
      cxNode.children = [];
      planNode.children.forEach((node) => {
        cxNode.children.push(this.toCxNode(node, parentNode));
      });
    }

    return cxNode;
  }

  private convertNodeInfo(planNode: PDPlanDto, parent?: CxNode): CxNode {
    const activity = planNode.pdPlanActivity;
    const nodeId = planNode.resultIdentity.extId;
    const nodeName =
      planNode.answer && planNode.answer.Title ? planNode.answer.Title : '';
    const nodeDataObject = new LearningPlanContent({
      pdplanDto: planNode,
      id: uniqueId(),
    });

    const cxNode: CxNode = {
      id: nodeId,
      name: nodeName,
      dataObject: nodeDataObject,
      hideChildren: false,
    };

    if (activity) {
      const statusCode =
        planNode.assessmentStatusInfo &&
        planNode.assessmentStatusInfo.assessmentStatusCode
          ? planNode.assessmentStatusInfo.assessmentStatusCode
          : undefined;
      const cxNodeStatus = this.convertAssessmentStatusToNodeStatus(statusCode);
      const cxNodeStatusText = this.buildNodeStatusText(
        planNode.assessmentStatusInfo
      );
      switch (activity) {
        case OdpActivity.Plan:
          cxNode.status = {
            shortName: 'LP',
            type: cxNodeStatus,
            text: cxNodeStatusText,
          };
          cxNode.iconClass = 'icon-opj-lp';
          break;
        case OdpActivity.Direction:
          cxNode.status = {
            shortName: 'LD',
            type: cxNodeStatus,
            text: cxNodeStatusText,
          };
          cxNode.iconClass = 'icon-opj-ld';

          break;
        case OdpActivity.Programme:
          cxNode.parent = parent ? parent : undefined;
          cxNode.iconClass = 'icon-opj-klp';
          cxNode.subName =
            planNode.answer && planNode.answer.targetAudienceSummary
              ? planNode.answer.targetAudienceSummary
              : undefined;
          break;
        default:
          break;
      }
    }

    return cxNode;
  }

  private convertAssessmentStatusToNodeStatus(
    assessmentStatusCode: any
  ): NodeStatusType {
    switch (assessmentStatusCode) {
      case OdpStatusCode.Started:
        return NodeStatusType.Draft;
      case OdpStatusCode.PendingForApproval:
        return NodeStatusType.Pending;
      case OdpStatusCode.Approved:
        return NodeStatusType.Approved;
      case OdpStatusCode.Rejected:
        return NodeStatusType.Rejected;
      default:
        return;
    }
  }

  private buildNodeStatusText(
    assessmentStatusInfo: AssessmentStatusInfo
  ): string {
    if (assessmentStatusInfo && assessmentStatusInfo.assessmentStatusName) {
      return `${assessmentStatusInfo.assessmentStatusName}`;
    }

    return '';
  }

  // Breadcrumb events handler
  private breadcrumDataChangedHandler = (result: any) => {
    if (result && this.currentUser) {
      const itemBreadcrumbs = this.breadcrumbSettingService.mapRouteToBreadcrumb(
        this.currentUser.headerData.menus,
        result.route,
        result.param
      );
      this.addItemToBreadcrumb(itemBreadcrumbs);
    }
  };

  private addItemToBreadcrumb(items: any): void {
    this.currentBreadcrumb = items;
    this.changeDetectorRef.detectChanges();
  }

  private async getODPConfig(): Promise<PDPlanConfig[]> {
    try {
      return await this.odpService.getOdpConfigs().toPromise();
    } catch (error) {
      return;
    }
  }

  private async getLearningPlanResultTree(extID: string): Promise<PDPlanDto[]> {
    try {
      return await this.odpService
        .getLearningPlanResultByExtID(extID)
        .toPromise();
    } catch (error) {
      return;
    }
  }

  private setSurveyjsDisplayMode(): void {
    this.planFormJSON.mode = SurveyjsMode.display;
    this.directionFormJSON.mode = SurveyjsMode.display;
    this.programmeFormJSON.mode = SurveyjsMode.display;
  }
}
