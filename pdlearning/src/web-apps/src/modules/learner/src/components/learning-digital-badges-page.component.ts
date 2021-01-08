import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'learning-digital-badges-page',
  templateUrl: './learning-digital-badges-page.component.html'
})
export class LearningDigitalBadgesPageComponent extends BaseComponent implements OnInit {
  @Output() public onDigitalBadgesBackClick: EventEmitter<void> = new EventEmitter<void>();

  public totalCount: number = 0;

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onGetDigitalBadges(totalCountCallBack: number = 0): void {
    this.totalCount = totalCountCallBack;
  }

  public onBackClicked(): void {
    this.onDigitalBadgesBackClick.emit();
  }
}
