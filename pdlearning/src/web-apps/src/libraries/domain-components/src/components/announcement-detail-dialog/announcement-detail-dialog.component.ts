import { Announcement, PublicUserInfo, RegistrationRepository, UserRepository } from '@opal20/domain-api';
import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

import { ANNOUNCEMENT_STATUS_COLOR_MAP } from './../../models/announcement-status-color-map.model';
import { Component } from '@angular/core';
import { DialogAction } from '@opal20/common-components';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'announcement-detail-dialog',
  templateUrl: './announcement-detail-dialog.component.html'
})
export class AnnouncementDetailDialogComponent extends BaseComponent {
  public announcement: Announcement = new Announcement();
  public statusColorMap = ANNOUNCEMENT_STATUS_COLOR_MAP;
  public preview: SafeHtml;
  public users: PublicUserInfo[] = [];
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public dialogRef: DialogRef,
    private sanitizer: DomSanitizer,
    private userRepository: UserRepository,
    private registrationRepository: RegistrationRepository
  ) {
    super(moduleFacadeService);
  }

  public onClose(): void {
    this.dialogRef.close({ action: DialogAction.Close });
  }

  protected onInit(): void {
    this.preview = this.sanitizer.bypassSecurityTrustHtml(this.announcement.message);
    this.registrationRepository
      .getRegistrationByIds(this.announcement.participants)
      .pipe(
        switchMap(registrations => {
          return this.userRepository.loadPublicUserInfoList({ userIds: registrations.map(x => x.userId) });
        }),
        this.untilDestroy()
      )
      .subscribe(users => {
        this.users = users;
      });
  }
}
