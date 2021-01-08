import { Observable, of } from 'rxjs';

import { CanDeactivate } from '@angular/router';
import { Injectable } from '@angular/core';
import { switchMap } from 'rxjs/operators';

export interface ICanDeactivateComponent {
  canDeactivate: () => Observable<boolean>;
}

@Injectable()
export class FormGuard implements CanDeactivate<ICanDeactivateComponent> {
  /**
   * Handling unsaved changes
   * @param screen Current active component
   */
  public canDeactivate(screen: ICanDeactivateComponent): Observable<boolean> | Promise<boolean> | boolean {
    const canDeactivate$: Observable<boolean> = screen.canDeactivate ? screen.canDeactivate() : of(true);

    return canDeactivate$.pipe(
      switchMap(status => {
        if (status) {
          return of(true);
        } else {
          return of(false);
        }
      })
    );
  }
}
