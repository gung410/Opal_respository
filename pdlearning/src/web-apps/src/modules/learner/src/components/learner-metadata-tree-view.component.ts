import { Component, Input } from '@angular/core';

import { MetadataTagModel } from '@opal20/domain-api';

@Component({
  selector: 'meta-data-tree-view',
  templateUrl: './learner-metadata-tree-view.component.html'
})
export class LearnerMetadataTreeView {
  @Input() public metadataTagsTree: MetadataTagModel[] = [];
  @Input() public isFirstLevel: boolean = true;

  public metadataTagsTreeTrackByFn(index: number, item: MetadataTagModel): string {
    return item.tagId;
  }
}
