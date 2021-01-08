import {
  AnnouncementApiService,
  AnnouncementRepository,
  AnnouncementType,
  ISendNominationRequest,
  OrganizationRepository
} from '@opal20/domain-api';
import { BaseFormComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { Subscription, combineLatest, from } from 'rxjs';

import { MailTag } from '@opal20/domain-components';
import { OpalDialogService } from '@opal20/common-components';
import { SendCourseNominationAnnoucementOption } from '../models/send-course-nomination-option.model';
import { SendCourseNominationAnnoucementViewModel } from './../view-models/send-course-nomination-view.model';

@Component({
  selector: 'send-course-nomination-announcement-page',
  templateUrl: './send-course-nomination-announcement-page.component.html'
})
export class SendCourseNominationAnnoucementPageComponent extends BaseFormComponent {
  @Input() public courseId: string;
  @Input() public stickyDependElement: HTMLElement;
  @Input() public stickySpacing: number;

  public mailTags: MailTag[] = [];
  public sendCourseNominationAnnoucementVm: SendCourseNominationAnnoucementViewModel = new SendCourseNominationAnnoucementViewModel();
  public SendCourseNominationAnnoucementOption: typeof SendCourseNominationAnnoucementOption = SendCourseNominationAnnoucementOption;
  public loadingData: boolean = false;

  private _loadDataSub: Subscription = new Subscription();
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private organizationRepository: OrganizationRepository,
    private announcementRepository: AnnouncementRepository,
    private announcementApiService: AnnouncementApiService,
    private opalDialogService: OpalDialogService
  ) {
    super(moduleFacadeService);
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();
    this.loadingData = true;
    const departmentObs = this.organizationRepository.loadDepartmentInfoList({
      departmentId: 1,
      includeChildren: true,
      includeDepartmentType: true
    });
    const emailTemplateObs = this.announcementRepository.getSendAnnouncementDefaultTemplate(AnnouncementType.CourseNomination);
    this._loadDataSub = combineLatest(emailTemplateObs, departmentObs).subscribe(
      ([emailTemplate, departments]) => {
        this.sendCourseNominationAnnoucementVm = new SendCourseNominationAnnoucementViewModel(emailTemplate, departments);
        this.mailTags = this.buildMailTags();
        this.loadingData = false;
      },
      () => {
        this.loadingData = false;
      }
    );
  }

  public onReset(): void {
    this.sendCourseNominationAnnoucementVm = this.sendCourseNominationAnnoucementVm.reset();
  }

  public onSend(): void {
    if (this.sendCourseNominationAnnoucementVm.checkReceiveValidation() === false) {
      this.opalDialogService.openConfirmDialog({
        confirmTitle: 'Warning',
        confirmMsg: 'Please select who will get the message.',
        hideNoBtn: true,
        yesBtnText: 'Close'
      });
    } else if (Utils.isEmpty(this.sendCourseNominationAnnoucementVm.mailContent)) {
      this.opalDialogService.openConfirmDialog({
        confirmTitle: 'Warning',
        confirmMsg: 'Please input the message',
        hideNoBtn: true,
        yesBtnText: 'Close'
      });
    } else {
      const specificOrganisation =
        this.sendCourseNominationAnnoucementVm.option === SendCourseNominationAnnoucementOption.SpecificOrganisation;
      const request: ISendNominationRequest = {
        courseId: this.courseId,
        specificOrganisation: specificOrganisation,
        organisations: specificOrganisation === false ? [] : this.sendCourseNominationAnnoucementVm.organisationIds,
        base64Message: Utils.encodeBase64(this.sendCourseNominationAnnoucementVm.mailContent),
        userNameTag: this.sendCourseNominationAnnoucementVm.defaultTemplate.userNameTagValue,
        courseTitleTag: this.sendCourseNominationAnnoucementVm.defaultTemplate.courseTitleTagValue
      };
      this.subscribe(from(this.announcementApiService.sendCourseNominationAnnoucement(request)), () => {
        this.showNotification(this.translate('Course Nomination sent successfully.'));
      });
    }
  }

  protected onInit(): void {
    this.loadData();
  }

  private buildMailTags(): MailTag[] {
    return [
      {
        text: 'User Name',
        value: this.sendCourseNominationAnnoucementVm.defaultTemplate.userNameTagValue
      },
      {
        text: 'Course Title',
        value: this.sendCourseNominationAnnoucementVm.defaultTemplate.courseTitleTagValue
      }
    ];
  }
}
