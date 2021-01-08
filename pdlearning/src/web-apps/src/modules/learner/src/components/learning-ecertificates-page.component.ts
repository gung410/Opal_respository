import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'learning-ecertificates-page',
  templateUrl: './learning-ecertificates-page.component.html'
})
export class LearningECertificatesPageComponent extends BaseComponent implements OnInit {
  @Output() public onECertificatesBackClick: EventEmitter<void> = new EventEmitter<void>();

  public totalCount: number = 0;

  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onGetECertificates(totalCountCallBack: number = 0): void {
    this.totalCount = totalCountCallBack;
  }

  public onBackClicked(): void {
    this.onECertificatesBackClick.emit();
  }
}
