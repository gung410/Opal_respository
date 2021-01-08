import { BaseComponent, ClipboardUtil, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, HostBinding, Input } from '@angular/core';
import { LearningPathModel, UserInfoModel } from '@opal20/domain-api';

import { WebAppLinkBuilder } from './../../helpers/webapp-link-builder.helper';

@Component({
  selector: 'copy-learning-path-hyperlink',
  templateUrl: './copy-learning-path-hyperlink.component.html'
})
export class CopyLearningPathHyperlinkComponent extends BaseComponent {
  @Input() public learningPath: LearningPathModel;

  @HostBinding('class.hidden') get isHidden(): boolean {
    return !this.canShowCopyHyperLinkButton();
  }

  private currentUser = UserInfoModel.getMyUserInfo();
  constructor(moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onCopyShareableLearningPathLinkBtnClicked(e: MouseEvent, item: LearningPathModel): void {
    e.stopImmediatePropagation();
    this.copyShareableLearningPathLink(item);
  }

  public copyShareableLearningPathLink(item: LearningPathModel): void {
    ClipboardUtil.copyTextToClipboard(WebAppLinkBuilder.buildLearningPathDetailUrl(item.id, true));
    this.showNotification('Copy learning path hyperlink successfully.');
  }

  public canShowCopyHyperLinkButton(): boolean {
    return this.learningPath.isPublished() && this.learningPath.hasViewCopyHyperLinkLearningPathButtonPermission(this.currentUser);
  }
}
