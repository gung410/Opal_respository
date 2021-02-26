import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  HostListener,
  Input,
  OnInit
} from '@angular/core';
import { OnDestroy } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { Utils } from '../../utilities/utils';

@Component({
  selector: 'data-check-indicator',
  templateUrl: './data-check-indicator.component.html',
  styleUrls: ['./data-check-indicator.component.scss']
})
export class DataCheckIndicatorComponent implements OnInit, OnDestroy {
  @Input() checkHasDataFn?: () => Observable<number>;

  @Input() set data(data: unknown) {
    if (!data) {
      return;
    }
    if (Utils.isDifferent(data, this._data)) {
      this.checkHasData();
    }
    this._data = data;
  }

  @Input() set dependencyData(dependencyData: unknown[]) {
    if (!dependencyData) {
      return;
    }
    if (Utils.isDifferent(dependencyData, this._dependencyData)) {
      this.checkHasData();
    }
    this._dependencyData = dependencyData;
  }

  hasDataChangeEvent: EventEmitter<boolean> = new EventEmitter();
  numberOfRemainingData: number = 0;

  private _checkHasDataSub: Subscription = new Subscription();
  private _data: unknown = null;
  private _dependencyData: unknown[] = null;

  constructor(protected changeDetectorRef: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.checkHasData();
  }

  // tslint:disable-next-line: no-unsafe-any
  @HostListener('click', ['$event'])
  onClick(e: unknown): void {
    this.checkHasData();
  }

  checkHasData(): void {
    this._checkHasDataSub.unsubscribe();
    if (this.checkHasDataFn == null) {
      return;
    }
    this._checkHasDataSub = this.checkHasDataFn().subscribe((data) => {
      this.numberOfRemainingData = data;
      this.changeDetectorRef.detectChanges();
    });
  }

  ngOnDestroy(): void {
    this._checkHasDataSub.unsubscribe();
  }
}
