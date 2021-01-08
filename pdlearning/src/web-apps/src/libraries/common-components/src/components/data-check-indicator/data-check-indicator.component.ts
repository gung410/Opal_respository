import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, HostListener, Input } from '@angular/core';
import { Observable, Subscription } from 'rxjs';

@Component({
  selector: 'data-check-indicator',
  templateUrl: './data-check-indicator.component.html'
})
export class DataCheckIndicatorComponent extends BaseComponent {
  @Input() public checkHasDataFn?: () => Observable<boolean>;
  @Input() public hasData: boolean = false;
  @Input('hasDataChange') public hasDataChangeEvent: EventEmitter<boolean> = new EventEmitter();

  private _checkHasDataSub: Subscription = new Subscription();
  constructor(moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  @HostListener('click', ['$event'])
  public onClick(e: unknown): void {
    this.checkHasData();
  }

  public checkHasData(): void {
    this._checkHasDataSub.unsubscribe();
    if (this.checkHasDataFn == null) {
      return;
    }

    this._checkHasDataSub = this.checkHasDataFn()
      .pipe(this.untilDestroy())
      .subscribe(data => {
        if (this.hasData !== data) {
          this.hasData = data;
          this.hasDataChangeEvent.emit(data);
        }
      });
  }

  protected onInit(): void {
    this.checkHasData();
  }
}
