import {
  AnnouncementApiService,
  AnnouncementRepository,
  AnnouncementType,
  BaseUserInfo,
  Course,
  CourseStatus,
  ISendOrderRefreshmentRequest,
  SystemRoleEnum,
  UserRepository,
  UserUtils
} from '@opal20/domain-api';
import { BaseFormComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { Observable, Subscription, combineLatest, from, of } from 'rxjs';

import { CourseDetailViewModel } from '@opal20/domain-components';
import { OpalDialogService } from '@opal20/common-components';
import { SendOrderRefreshmentViewModel } from '../view-models/send-order-refreshment-view.model';
import { map } from 'rxjs/operators';

@Component({
  selector: 'send-order-refreshment-page',
  templateUrl: './send-order-refreshment-page.component.html'
})
export class SendOrderRefreshmentComponent extends BaseFormComponent {
  @Input() public courseId: string;
  @Input() public stickyDependElement: HTMLElement;
  @Input() public stickySpacing: number;
  @Input() public set course(v: Course) {
    if (Utils.isDifferent(this._course, v)) {
      this._course = v;
      this.loadTemplateData();
    }
  }

  public get course(): Course {
    return this._course;
  }

  public fetchCourseAdministratorItemsFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]>;
  public sendOrderRefreshmentVm: SendOrderRefreshmentViewModel = new SendOrderRefreshmentViewModel();

  private _loadDataSub: Subscription = new Subscription();
  private _course: Course = new Course();

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private announcementRepository: AnnouncementRepository,
    private announcementApiService: AnnouncementApiService,
    private userRepository: UserRepository,
    private opalDialogService: OpalDialogService
  ) {
    super(moduleFacadeService);
    this.fetchCourseAdministratorItemsFn = this._createFetchUserSelectItemFn(CourseDetailViewModel.courseAdministratorItemsRoles);
  }

  public onReset(): void {
    this.sendOrderRefreshmentVm = this.sendOrderRefreshmentVm.reset();
  }

  public onSend(): void {
    if (this.sendOrderRefreshmentVm.checkReceiveValidation() === false) {
      this.opalDialogService.openConfirmDialog({
        confirmTitle: 'Warning',
        confirmMsg: 'Please select who will get the message.',
        hideNoBtn: true,
        yesBtnText: 'Close'
      });
    } else if (Utils.isEmpty(this.sendOrderRefreshmentVm.emailContent)) {
      this.opalDialogService.openConfirmDialog({
        confirmTitle: 'Warning',
        confirmMsg: 'Please input the message',
        hideNoBtn: true,
        yesBtnText: 'Close'
      });
    } else {
      const request: ISendOrderRefreshmentRequest = {
        sendToEmails: this.sendOrderRefreshmentVm.sendToEmails,
        emailCC: this.sendOrderRefreshmentVm.emailCC,
        subject: this.sendOrderRefreshmentVm.emailSubject,
        base64Message: Utils.encodeBase64_2(this.sendOrderRefreshmentVm.emailContent)
      };
      this.subscribe(from(this.announcementApiService.sendOrderRefreshment(request)), () => {
        this.showNotification(this.translate('Order Refreshment sent successfully.'));
      });
    }
  }

  private loadTemplateData(): void {
    this._loadDataSub.unsubscribe();
    const announcementObs = this.announcementRepository.getSendAnnouncementDefaultTemplate(
      AnnouncementType.OrderRefreshment,
      this.courseId,
      true
    );
    const userObs: Observable<BaseUserInfo[]> =
      this.course.status === CourseStatus.Published
        ? this.userRepository.loadBaseUserInfoList({ extIds: this.course.getAdminstratorIds(), pageSize: 0, pageIndex: 0 })
        : of([]);
    this._loadDataSub = combineLatest(announcementObs, userObs).subscribe(
      ([emailTemplate, users]) =>
        (this.sendOrderRefreshmentVm = new SendOrderRefreshmentViewModel(emailTemplate, this.course.courseName, users))
    );
  }

  private _createFetchUserSelectItemFn(
    inRoles?: SystemRoleEnum[],
    mapFn?: (data: BaseUserInfo[]) => BaseUserInfo[]
  ): (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]> {
    const createFetchUsersFn = UserUtils.createFetchUsersFn(inRoles, this.userRepository, false);
    return (searchText: string, skipCount: number, maxResultCount: number) =>
      createFetchUsersFn(searchText, skipCount, maxResultCount).pipe(
        map(users => {
          if (mapFn) {
            return mapFn(users);
          }
          return users;
        })
      );
  }
}
