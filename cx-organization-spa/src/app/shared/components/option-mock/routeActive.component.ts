import { Type } from '@angular/core';
import {
  ActivatedRoute,
  ActivatedRouteSnapshot,
  Data,
  Params,
  Route,
  UrlSegment
} from '@angular/router';
import { Observable } from 'rxjs';

export class MockActivatedRoute extends ActivatedRoute {
  snapshot: ActivatedRouteSnapshot;
  url: Observable<UrlSegment[]>;
  params: Observable<Params>;
  queryParams: Observable<Params>;
  fragment: Observable<string>;
  data: Observable<Data>;
  outlet: string;
  component: Type<any> | string;
  routeConfig: Route;
  root: ActivatedRoute;
  parent: ActivatedRoute;
  firstChild: ActivatedRoute;
  children: ActivatedRoute[];
  pathFromRoot: ActivatedRoute[];
  toString(): string {
    return '';
  }
  constructor() {
    super();
    this.params = Observable.of({ searchValue: 'Test' });
  }
}
