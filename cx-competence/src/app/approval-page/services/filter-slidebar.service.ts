import { Injectable, Type } from '@angular/core';
import { IDictionary } from 'app-models/dictionary';
import { BehaviorSubject, Subject } from 'rxjs';

@Injectable()
export class FilterSlidebarService {
  /**
   * Submit filter form signal. Occur every click "Submit"/"Apply"  button.
   */
  public onSubmitFilter: Subject<IDictionary<unknown>> = new Subject<
    IDictionary<unknown>
  >();
  /**
   * Reset filter parammeters signal. Occur every click "Clear" button.
   */
  public onResetFilter: Subject<boolean> = new Subject<boolean>();

  /**
   * Init filter form singal. The value is a IFilterForm component.
   */
  public onInitFilterForm: BehaviorSubject<any> = new BehaviorSubject<any>(
    null
  );

  constructor() {}

  public initSlidebar<T>(component: Type<T>): void {
    this.onInitFilterForm.next(component);
  }
}
