import { BehaviorSubject, Observable, of } from 'rxjs';
import {
  FormApiService,
  FormModel,
  FormStatus,
  FormSurveyType,
  FormType,
  FormWithQuestionsModel,
  GetPendingApprovalFormsResponseResponse,
  IFormModel,
  SearchFormResponse,
  UserRepository
} from '@opal20/domain-api';
import { map, switchMap } from 'rxjs/operators';

import { Injectable } from '@angular/core';
import { Utils } from '@opal20/infrastructure';

@Injectable()
export class FormRepositoryPageService {
  public get formListData$(): Observable<FormRepositoryPageFormListData> {
    return this._formListDataSubject.asObservable();
  }
  private _formListDataSubject: BehaviorSubject<FormRepositoryPageFormListData> = new BehaviorSubject(new FormRepositoryPageFormListData());

  constructor(private formApiService: FormApiService, private userRepository: UserRepository) {}

  public loadFormListData(
    skipCount: number,
    maxResultCount: number,
    searchFormTitle: string | undefined = undefined,
    filterByStatus: FormStatus[] = [],
    includeFormForImportToCourse?: boolean,
    filterByType?: FormType,
    filterBySurveyTypes: FormSurveyType[] = [],
    isSurveyTemplate?: boolean,
    excludeBySurveyTypes?: FormSurveyType[]
  ): Observable<FormRepositoryPageFormListData> {
    const obs: Observable<SearchFormResponse> | Observable<GetPendingApprovalFormsResponseResponse> = this.formApiService.searchForm(
      skipCount,
      maxResultCount,
      searchFormTitle,
      filterByStatus,
      includeFormForImportToCourse,
      filterByType,
      filterBySurveyTypes,
      true,
      isSurveyTemplate,
      excludeBySurveyTypes
    );

    return obs.pipe(
      switchMap(formSearchResult => {
        if (formSearchResult.totalCount === 0) {
          this._formListDataSubject.next(
            new FormRepositoryPageFormListData({
              totalCount: 0,
              formList: []
            })
          );
          return of(null);
        }

        return this.userRepository.loadPublicUserInfoList({ userIds: Utils.uniq(formSearchResult.items.map(item => item.ownerId)) }).pipe(
          switchMap(publicUser => {
            const publicUserDic = Utils.toDictionary(publicUser, p => p.id);
            const formListData = new FormRepositoryPageFormListData({
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
    return this.formApiService.deleteForm(formId).pipe(
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
    return this.changeFormStatus(formId, FormStatus.Unpublished);
  }

  public publishForm(formId: string): Observable<void> {
    return this.changeFormStatus(formId, FormStatus.Published);
  }

  public renameForm(formId: string, formTitle: string): Observable<void> {
    const updateFormItemResult = this.updateFormItem(formId, (item, index) => {
      item.title = formTitle;
    });
    return this.formApiService.updateForm(updateFormItemResult.changedFormItem).pipe(
      map(() => {
        this._formListDataSubject.next(updateFormItemResult.changedFormListData);
      })
    );
  }

  public changeFormStatusForm(formId: string, formStatus: FormStatus): Observable<void> {
    const updateFormItemResult = this.updateFormItem(formId, (item, index) => {
      item.status = formStatus;
    });
    return this.formApiService.updateStatusAndData(updateFormItemResult.changedFormItem, [], [], false).pipe(
      map(() => {
        this._formListDataSubject.next(updateFormItemResult.changedFormListData);
      })
    );
  }

  public cloneForm(formId: string, newFormTitle: string): Observable<FormWithQuestionsModel> {
    const formIndex = this._formListDataSubject.value.formList.findIndex(p => p.id === formId);
    const clonedForm: FormModel = Utils.cloneDeep(this._formListDataSubject.value.formList[formIndex]);
    clonedForm.title = newFormTitle;

    return this.formApiService.cloneForm(formId, newFormTitle).pipe(
      map(formWithQuestions => {
        const changedFormListData = Utils.clone(this._formListDataSubject.value, formListData => {
          formListData.formList = Utils.clone(formListData.formList, formList => {
            formList.splice(0, 0, formWithQuestions.form);
          });
          formListData.totalCount += 1;
        });
        this._formListDataSubject.next(changedFormListData);
        return formWithQuestions;
      })
    );
  }

  public transferOwnership(formId: string, newOwnerId: string): Observable<void> {
    return this.formApiService.transferOwnerShip({
      objectId: formId,
      newOwnerId: newOwnerId
    });
  }

  public archiveForm(formId: string, archiveByUserId: string): Observable<void> {
    return this.formApiService.archiveForm({ objectId: formId, archiveByUserId: archiveByUserId });
  }

  private changeFormStatus(formId: string, formStatus: FormStatus): Observable<void> {
    const updateFormItemResult = this.updateFormItem(formId, (item, index) => {
      item.status = formStatus;
    });
    return this.formApiService.updateForm(updateFormItemResult.changedFormItem).pipe(
      map(() => {
        this._formListDataSubject.next(updateFormItemResult.changedFormListData);
      })
    );
  }

  private updateFormItem(
    formId: string,
    updateActionFn: (item: FormModel, itemIndex: number) => void
  ): { changedFormListData: FormRepositoryPageFormListData; changedFormItem: FormModel } {
    const formIndex: number = this._formListDataSubject.value.formList.findIndex(p => p.id === formId);
    const changedFormListData: FormRepositoryPageFormListData = Utils.clone(this._formListDataSubject.value, formListData => {
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

export interface IFormRepositoryPageFormListData {
  totalCount: number;
  formList: IFormModel[];
}

export class FormRepositoryPageFormListData implements IFormRepositoryPageFormListData {
  public totalCount: number = 0;
  public formList: FormModel[] = [];

  constructor(data?: IFormRepositoryPageFormListData) {
    if (data != null) {
      this.totalCount = data.totalCount;
      this.formList = data.formList.map(_ => new FormModel(_));
    }
  }
}
