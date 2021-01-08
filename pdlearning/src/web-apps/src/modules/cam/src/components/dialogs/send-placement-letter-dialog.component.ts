import { AnnouncementApiService, AnnouncementRepository, AnnouncementType } from '@opal20/domain-api';
import { BaseComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { DialogAction, OpalDialogService } from '@opal20/common-components';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { MailTag, RegistrationViewModel } from '@opal20/domain-components';
import { Subscription, combineLatest } from 'rxjs';

import { Component } from '@angular/core';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { SendPlacementLetterViewModel } from '../../view-models/send-placement-letter-view.model';

@Component({
  selector: 'send-placement-letter-dialog',
  templateUrl: './send-placement-letter-dialog.component.html'
})
export class SendPlacementLetterDialogComponent extends BaseComponent {
  public registrationVms: RegistrationViewModel[] = [];
  public classRunId: string;
  public mailTags: MailTag[] = [];
  public sendPlacementLetterVm: SendPlacementLetterViewModel = new SendPlacementLetterViewModel();
  public loadingData: boolean = false;
  public dataLoaded: boolean = false;
  public isPreview: boolean = false;
  public previewMessage: SafeHtml;

  private _loadDataSub: Subscription = new Subscription();

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public dialogRef: DialogRef,
    private announcementRepository: AnnouncementRepository,
    private opalDialogService: OpalDialogService,
    private announcementApiService: AnnouncementApiService,
    private sanitizer: DomSanitizer
  ) {
    super(moduleFacadeService);
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();
    this.loadingData = true;
    const emailTemplateObs = this.announcementRepository.getSendAnnouncementDefaultTemplate(AnnouncementType.PlacementLetter);
    this._loadDataSub = combineLatest(emailTemplateObs).subscribe(
      ([emailTemplate]) => {
        this.sendPlacementLetterVm = new SendPlacementLetterViewModel(emailTemplate);
        this.mailTags = this.buildMailTags();
        this.loadingData = false;
        this.dataLoaded = true;
      },
      () => {
        this.loadingData = false;
      }
    );
  }

  public onCancel(): void {
    this.dialogRef.close();
  }

  public onReset(): void {
    this.sendPlacementLetterVm = this.sendPlacementLetterVm.reset();
  }

  public onPreview(): void {
    this.isPreview = true;
    this.announcementApiService
      .previewAnnouncementTemplate({
        announcementType: AnnouncementType.PlacementLetter,
        classRunId: this.classRunId,
        base64Message: Utils.encodeBase64(this.sendPlacementLetterVm.mailContent),
        userNameTag: this.sendPlacementLetterVm.defaultTemplate.userNameTagValue,
        courseTitleTag: this.sendPlacementLetterVm.defaultTemplate.courseTitleTagValue,
        courseCodeTag: this.sendPlacementLetterVm.defaultTemplate.courseCodeTagValue,
        courseAdminNameTag: this.sendPlacementLetterVm.defaultTemplate.courseAdminNameTagValue,
        courseAdminEmailTag: this.sendPlacementLetterVm.defaultTemplate.courseAdminEmailTagValue,
        listSessionTag: this.sendPlacementLetterVm.defaultTemplate.listSessionTagValue,
        detailUrlTag: this.sendPlacementLetterVm.defaultTemplate.detailUrlTagValue
      })
      .then(data => {
        if (data != null) {
          this.previewMessage = this.sanitizer.bypassSecurityTrustHtml(data.message);
        }
      });
  }

  public onBack(): void {
    this.isPreview = false;
  }

  public onSend(): void {
    this.opalDialogService
      .openConfirmDialog({
        confirmTitle: 'Confirmation',
        confirmMsg: 'You are about to send the placement letter(s) for selected learner(s). Do you want to proceed?',
        yesBtnText: 'Yes',
        noBtnText: 'Cancel'
      })
      .subscribe(action => {
        if (action === DialogAction.OK) {
          this.announcementApiService
            .sendPlacementLetter({
              ids: this.registrationVms.map(item => item.id),
              base64Message: Utils.encodeBase64(this.sendPlacementLetterVm.mailContent),
              userNameTag: this.sendPlacementLetterVm.defaultTemplate.userNameTagValue,
              courseTitleTag: this.sendPlacementLetterVm.defaultTemplate.courseTitleTagValue,
              courseCodeTag: this.sendPlacementLetterVm.defaultTemplate.courseCodeTagValue,
              courseAdminNameTag: this.sendPlacementLetterVm.defaultTemplate.courseAdminNameTagValue,
              courseAdminEmailTag: this.sendPlacementLetterVm.defaultTemplate.courseAdminEmailTagValue,
              listSessionTag: this.sendPlacementLetterVm.defaultTemplate.listSessionTagValue,
              detailUrlTag: this.sendPlacementLetterVm.defaultTemplate.detailUrlTagValue
            })
            .then(() => {
              this.showNotification('Placement letter sent successfully');
              this.dialogRef.close(DialogAction.OK);
            });
        }
      });
  }

  protected onInit(): void {
    this.loadData();
  }

  private buildMailTags(): MailTag[] {
    return [
      {
        text: 'User Name',
        value: this.sendPlacementLetterVm.defaultTemplate.userNameTagValue
      },
      {
        text: 'Course Title',
        value: this.sendPlacementLetterVm.defaultTemplate.courseTitleTagValue
      },
      {
        text: 'Course Code',
        value: this.sendPlacementLetterVm.defaultTemplate.courseCodeTagValue
      },
      {
        text: 'Course Admin Name',
        value: this.sendPlacementLetterVm.defaultTemplate.courseAdminNameTagValue
      },
      {
        text: 'Course Admin Email',
        value: this.sendPlacementLetterVm.defaultTemplate.courseAdminEmailTagValue
      },
      {
        text: 'List Sessions',
        value: this.sendPlacementLetterVm.defaultTemplate.listSessionTagValue
      },
      {
        text: 'Detail URL',
        value: this.sendPlacementLetterVm.defaultTemplate.detailUrlTagValue
      }
    ];
  }
}
