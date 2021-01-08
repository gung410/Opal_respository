import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { IActionItemResponse } from './dtos/action-item-response.dto';
import { ICreateActionItemResultRequest } from './dtos/create-action-item-result-request';
import { Injectable } from '@angular/core';

@Injectable()
export class IdpRepositoryContext extends BaseRepositoryContext {
  public actionItemResultSubject: BehaviorSubject<Dictionary<ICreateActionItemResultRequest>> = new BehaviorSubject({});
  public actionItemResponseSubject: BehaviorSubject<Dictionary<IActionItemResponse>> = new BehaviorSubject({});
}
