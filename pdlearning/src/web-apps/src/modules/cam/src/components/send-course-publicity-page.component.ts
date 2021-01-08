import {
  AnnouncementApiService,
  AnnouncementRepository,
  AnnouncementType,
  BaseUserInfo,
  ISendPublicityRequest,
  SystemRoleEnum,
  TaggingRepository,
  UserRepository,
  UserUtils
} from '@opal20/domain-api';
import { BaseFormComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { Observable, Subscription, combineLatest, from } from 'rxjs';

import { MailTag } from '@opal20/domain-components';
import { OpalDialogService } from '@opal20/common-components';
import { SendCoursePublicityOption } from '../models/send-course-publicity-option.model';
import { SendCoursePublicityViewModel } from '../view-models/send-course-publicity-view.model';
import { map } from 'rxjs/operators';

@Component({
  selector: 'send-course-publicity-page',
  templateUrl: './send-course-publicity-page.component.html'
})
export class SendCoursePublicityPageComponent extends BaseFormComponent {
  @Input() public courseId: string;
  @Input() public stickyDependElement: HTMLElement;
  @Input() public stickySpacing: number;

  public mailTags: MailTag[] = [];
  public fetchUserItemsFn: (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]>;
  public sendCoursePublicityVm: SendCoursePublicityViewModel = new SendCoursePublicityViewModel();
  public SendCoursePublicityOption: typeof SendCoursePublicityOption = SendCoursePublicityOption;
  public loadingData: boolean = false;

  private _loadDataSub: Subscription = new Subscription();
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private taggingRepository: TaggingRepository,
    private userRepository: UserRepository,
    private announcementRepository: AnnouncementRepository,
    private announcementApiService: AnnouncementApiService,
    private opalDialogService: OpalDialogService
  ) {
    super(moduleFacadeService);
    this.fetchUserItemsFn = this._createFetchUserSelectItemFn([SystemRoleEnum.Learner]);
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();
    this.loadingData = true;
    const taggingObs = this.taggingRepository.loadAllMetaDataTags();
    const emailTemplateObs = this.announcementRepository.getSendAnnouncementDefaultTemplate(AnnouncementType.CoursePublicity);
    this._loadDataSub = combineLatest(emailTemplateObs, taggingObs).subscribe(
      ([emailTemplate, taggings]) => {
        this.sendCoursePublicityVm = new SendCoursePublicityViewModel(emailTemplate, taggings);
        this.mailTags = this.buildMailTags();
        this.loadingData = false;
      },
      () => {
        this.loadingData = false;
      }
    );
  }

  public onReset(): void {
    this.sendCoursePublicityVm = this.sendCoursePublicityVm.reset();
  }

  public onSend(): void {
    if (this.sendCoursePublicityVm.checkReceiveValidation() === false) {
      this.opalDialogService.openConfirmDialog({
        confirmTitle: 'Warning',
        confirmMsg: 'Please select who will get the message.',
        hideNoBtn: true,
        yesBtnText: 'Close'
      });
    } else if (Utils.isEmpty(this.sendCoursePublicityVm.mailContent)) {
      this.opalDialogService.openConfirmDialog({
        confirmTitle: 'Warning',
        confirmMsg: 'Please input the message',
        hideNoBtn: true,
        yesBtnText: 'Close'
      });
    } else {
      const specificTargetAudience = this.sendCoursePublicityVm.option === SendCoursePublicityOption.SpecificTargetAudience;
      const request: ISendPublicityRequest = {
        courseId: this.courseId,
        specificTargetAudience: specificTargetAudience,
        userIds: specificTargetAudience === false ? [] : this.sendCoursePublicityVm.users,
        teachingSubjectIds: specificTargetAudience === false ? [] : this.sendCoursePublicityVm.teachingSubjectIds,
        teachingLevels: specificTargetAudience === false ? [] : this.sendCoursePublicityVm.teachingLevels,
        base64Message: Utils.encodeBase64_2(this.sendCoursePublicityVm.mailContent),
        userNameTag: this.sendCoursePublicityVm.defaultTemplate.userNameTagValue,
        courseTitleTag: this.sendCoursePublicityVm.defaultTemplate.courseTitleTagValue
      };
      this.subscribe(from(this.announcementApiService.sendCoursePublicity(request)), () => {
        this.showNotification(this.translate('Course Publicity sent successfully.'));
      });
    }
  }

  protected onInit(): void {
    this.loadData();
  }

  private _createFetchUserSelectItemFn(
    inRoles?: SystemRoleEnum[],
    mapFn?: (data: BaseUserInfo[]) => BaseUserInfo[]
  ): (searchText: string, skipCount: number, maxResultCount: number) => Observable<BaseUserInfo[]> {
    const createFetchUsersFn = UserUtils.createFetchUsersFn(inRoles, this.userRepository);
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

  private buildMailTags(): MailTag[] {
    return [
      {
        text: 'User Name',
        value: this.sendCoursePublicityVm.defaultTemplate.userNameTagValue
      },
      {
        text: 'Course Title',
        value: this.sendCoursePublicityVm.defaultTemplate.courseTitleTagValue
      }
    ];
  }
}
