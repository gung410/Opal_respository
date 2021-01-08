import { Injectable, OnDestroy } from '@angular/core';
import { combineLatest, Observable, Subscription } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { Utils } from '../../shared/utilities/utils';
import { ColumnItemModel } from '../models/column-item.model';

@Injectable()
export class PermissionsColumnService<T extends ColumnItemModel<unknown>>
  implements OnDestroy {
  columnDataChange$: Observable<Array<unknown>> = new Observable<
    Array<unknown>
  >();
  numberOfRegisteredObject: number = 0;
  originColumnData: Array<unknown>;

  private _originColumnDataSubscription: Subscription;
  private _activeColumn: number | string | null = null;
  private registeredObsArray: Array<Observable<T>> = [];

  registerObs(registerObs: Observable<T>): void {
    if (this.IsObsHasRegistered(registerObs)) {
      return;
    }
    this.numberOfRegisteredObject++;
    this.registeredObsArray.push(registerObs);
    // tslint:disable-next-line: deprecation
    this.columnDataChange$ = combineLatest(...this.registeredObsArray).pipe(
      map((columns: Array<ColumnItemModel<unknown>>) => {
        if (!this._activeColumn) {
          return [];
        }

        return columns
          .filter((colItem) => colItem.colId === Number(this._activeColumn))
          .map((colItem) => colItem.item);
      })
    );
  }

  activateColumn(colId: number | string | null): void {
    if (!colId) {
      return;
    }

    if (this._originColumnDataSubscription) {
      this._originColumnDataSubscription.unsubscribe();
    }

    this._activeColumn = colId;
    this._originColumnDataSubscription = this.columnDataChange$
      .pipe(take(1))
      .subscribe((originColumnData) => {
        this.originColumnData = Utils.cloneDeep(originColumnData);
      });
  }

  isDifferentWithOriginColumnData(columnData: Array<unknown>): boolean {
    return Utils.isDifferent(this.originColumnData, columnData);
  }

  ngOnDestroy(): void {}

  refreshColumn(): void {
    this.columnDataChange$ = new Observable<T[]>();
    this.registeredObsArray = [];
    this.numberOfRegisteredObject = 0;
  }

  private IsObsHasRegistered(newObs: Observable<T>): boolean {
    return this.registeredObsArray.some((registeredObs) =>
      Utils.isEqual(newObs, registeredObs)
    );
  }
}
