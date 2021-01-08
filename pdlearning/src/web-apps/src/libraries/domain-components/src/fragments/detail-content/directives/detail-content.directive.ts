import { Directive, OnDestroy, OnInit } from '@angular/core';

import { DetailContentFragment } from '../detail-content.fragment';
import { DetailContentService } from './../detail-content.service';

@Directive({ selector: 'detail-content' })
export class DetailContentDirective implements OnInit, OnDestroy {
  private _fragment: DetailContentFragment | null;
  constructor(private detailContentService: DetailContentService) {}

  public ngOnInit(): void {
    // Implement ngOnInit
    this._fragment = this.detailContentService.currentFragment();
  }

  public ngOnDestroy(): void {
    this._fragment.detachViews();
  }
}
