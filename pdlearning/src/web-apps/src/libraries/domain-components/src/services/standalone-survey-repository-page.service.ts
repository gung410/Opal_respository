import { BehaviorSubject, Observable, of } from 'rxjs';
import {
  IStandaloneSurveyModel,
  SearchSurveyResponse,
  StandaloneSurveyApiService,
  StandaloneSurveyModel,
  SurveyStatus,
  UserRepository
} from '@opal20/domain-api';
import { map, switchMap } from 'rxjs/operators';

import { Injectable } from '@angular/core';
import { Utils } from '@opal20/infrastructure';

@Injectable()
export class StandaloneSurveyRepositoryPageService {
  public get formListData$(): Observable<StandaloneSurveyRepositoryPageFormListData> {
    return this._formListDataSubject.asObservable();
  }
  private _formListDataSubject: BehaviorSubject<StandaloneSurveyRepositoryPageFormListData> = new BehaviorSubject(
    new StandaloneSurveyRepositoryPageFormListData()
  );

  constructor(private lnaFormApiService: StandaloneSurveyApiService, private userRepository: UserRepository) {}

  public loadFormListData(
    skipCount: number,
    maxResultCount: number,
    searchFormTitle: string | undefined = undefined,
    filterByStatus: SurveyStatus[] = []
  ): Observable<StandaloneSurveyRepositoryPageFormListData> {
    const obs: Observable<SearchSurveyResponse> = this.lnaFormApiService.searchSurvey(
      skipCount,
      maxResultCount,
      searchFormTitle,
      filterByStatus,
      false
    );

    return obs.pipe(
      switchMap(formSearchResult => {
        if (formSearchResult.totalCount === 0) {
          this._formListDataSubject.next(
            new StandaloneSurveyRepositoryPageFormListData({
              totalCount: 0,
              formList: []
            })
          );
          return of(null);
        }

        return this.userRepository.loadPublicUserInfoList({ userIds: Utils.uniq(formSearchResult.items.map(item => item.ownerId)) }).pipe(
          switchMap(publicUser => {
            const publicUserDic = Utils.toDictionary(publicUser, p => p.id);
            const formListData = new StandaloneSurveyRepositoryPageFormListData({
              totalCount: formSearchResult.totalCount,
              formList: formSearchResult.items.map(item => {
                item.owner = publicUserDic[item.ownerId];
                if (item.archivedBy) {
                  item.archivedByUser = publicUserDic[item.archivedBy];
                }
                return item;
              })
            });

            this._formListDataSubject.next(formListData);

            return of(formListData);
          })
        );
      })
    );
  }

  public deleteForm(formId: string): Observable<void> {
    return this.lnaFormApiService.deleteSurvey(formId).pipe(
      map(() => {
        const changedFormListData = Utils.clone(this._formListDataSubject.value, formListData => {
          formListData.formList = Utils.clone(formListData.formList, formList => {
            Utils.remove(formList, p => p.id === formId);
          });
        });
        this._formListDataSubject.next(changedFormListData);
      })
    );
  }

  public unpublishForm(formId: string): Observable<void> {
    return this.changeLnaFormStatus(formId, SurveyStatus.Unpublished);
  }

  public publishForm(formId: string): Observable<void> {
    return this.changeLnaFormStatus(formId, SurveyStatus.Published);
  }

  public renameForm(formId: string, formTitle: string): Observable<void> {
    const updateFormItemResult = this.updateFormItem(formId, (item, index) => {
      item.title = formTitle;
    });
    return this.lnaFormApiService.updateSurvey(updateFormItemResult.changedFormItem).pipe(
      map(() => {
        this._formListDataSubject.next(updateFormItemResult.changedFormListData);
      })
    );
  }

  public changeFormStatusForm(formId: string, lnaFormStatus: SurveyStatus): Observable<void> {
    const updateFormItemResult = this.updateFormItem(formId, (item, index) => {
      item.status = lnaFormStatus;
    });
    return this.lnaFormApiService.updateStatusAndData(updateFormItemResult.changedFormItem, [], [], false).pipe(
      map(() => {
        this._formListDataSubject.next(updateFormItemResult.changedFormListData);
      })
    );
  }

  public cloneForm(formId: string, newFormTitle: string): Observable<void> {
    const formIndex = this._formListDataSubject.value.formList.findIndex(p => p.id === formId);
    const clonedForm: StandaloneSurveyModel = Utils.cloneDeep(this._formListDataSubject.value.formList[formIndex]);
    clonedForm.title = newFormTitle;

    return this.lnaFormApiService.cloneSurveys(formId, newFormTitle).pipe(
      map(formWithQuestions => {
        const changedFormListData = Utils.clone(this._formListDataSubject.value, formListData => {
          formListData.formList = Utils.clone(formListData.formList, formList => {
            formList.splice(0, 0, formWithQuestions.form);
          });
          formListData.totalCount += 1;
        });
        this._formListDataSubject.next(changedFormListData);
      })
    );
  }

  public transferOwnership(formId: string, newOwnerId: string): Observable<void> {
    return this.lnaFormApiService.transferOwnerShip({
      objectId: formId,
      newOwnerId: newOwnerId
    });
  }

  public archiveForm(formId: string, archiveByUserId: string): Observable<void> {
    return this.lnaFormApiService.archiveSurvey({ objectId: formId, archiveByUserId: archiveByUserId });
  }

  private changeLnaFormStatus(formId: string, lnaFormStatus: SurveyStatus): Observable<void> {
    const updateFormItemResult = this.updateFormItem(formId, (item, index) => {
      item.status = lnaFormStatus;
    });
    return this.lnaFormApiService.updateSurvey(updateFormItemResult.changedFormItem).pipe(
      map(() => {
        this._formListDataSubject.next(updateFormItemResult.changedFormListData);
      })
    );
  }

  private updateFormItem(
    formId: string,
    updateActionFn: (item: StandaloneSurveyModel, itemIndex: number) => void
  ): { changedFormListData: StandaloneSurveyRepositoryPageFormListData; changedFormItem: StandaloneSurveyModel } {
    const formIndex: number = this._formListDataSubject.value.formList.findIndex(p => p.id === formId);
    const changedFormListData: StandaloneSurveyRepositoryPageFormListData = Utils.clone(this._formListDataSubject.value, formListData => {
      formListData.formList = Utils.clone(formListData.formList, formList => {
        formList[formIndex] = Utils.clone(formList[formIndex], formItem => {
          updateActionFn(formItem, formIndex);
          formItem.changedDate = new Date(Date.now());
        });
      });
    });
    return {
      changedFormListData: changedFormListData,
      changedFormItem: changedFormListData.formList[formIndex]
    };
  }
}

export interface IStandaloneSurveyRepositoryPageFormListData {
  totalCount: number;
  formList: IStandaloneSurveyModel[];
}

export class StandaloneSurveyRepositoryPageFormListData implements IStandaloneSurveyRepositoryPageFormListData {
  public totalCount: number = 0;
  public formList: StandaloneSurveyModel[] = [];

  constructor(data?: IStandaloneSurveyRepositoryPageFormListData) {
    if (data != null) {
      this.totalCount = data.totalCount;
      this.formList = data.formList.map(_ => new StandaloneSurveyModel(_));
    }
  }
}
