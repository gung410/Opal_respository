import { BehaviorSubject, Observable, combineLatest, of } from 'rxjs';
import { GlobalSpinnerService, Utils } from '@opal20/infrastructure';
import {
  IUpdateSurveyAnswerRequest,
  LearningCatalogRepository,
  PublicUserInfo,
  StandaloneSurveyAnswerApiService,
  StandaloneSurveyApiService,
  SurveyAnswerModel,
  SurveyWithQuestionsModel,
  UserInfoModel,
  UserRepository
} from '@opal20/domain-api';

import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

@Injectable()
export class StandaloneSurveyQuizPlayerPageService {
  public get formData$(): Observable<SurveyWithQuestionsModel | undefined> {
    return this._formDataSubject.asObservable();
  }
  public get formAnswersData$(): Observable<SurveyAnswerModel[] | undefined> {
    return this._formAnswersDataSubject.asObservable();
  }

  public readonly populatedFields = [
    {
      shortCode: '[name]',
      tempShortCode: '[$name]',
      disabledShortCode: '/[name]/'
    },
    {
      shortCode: '[email]',
      tempShortCode: '[$email]',
      disabledShortCode: '/[email]/'
    },
    {
      shortCode: '[placeofwork]',
      tempShortCode: '[$placeofwork]',
      disabledShortCode: '/[placeofwork]/'
    },
    {
      shortCode: '[designation]',
      tempShortCode: '[$designation]',
      disabledShortCode: '/[designation]/'
    },
    {
      shortCode: '[teachinglevel]',
      tempShortCode: '[$teachinglevel]',
      disabledShortCode: '/[teachinglevel]/'
    },
    {
      shortCode: '[teachingsubject_or_jobfamily]',
      tempShortCode: '[$teachingsubject_or_jobfamily]',
      disabledShortCode: '/[teachingsubject_or_jobfamily]/'
    },
    {
      shortCode: '[accounttype]',
      tempShortCode: '[$accounttype]',
      disabledShortCode: '/[accounttype]/'
    }
  ];

  private _formDataSubject: BehaviorSubject<SurveyWithQuestionsModel | undefined> = new BehaviorSubject(undefined);
  private _formAnswersDataSubject: BehaviorSubject<SurveyAnswerModel[] | undefined> = new BehaviorSubject(undefined);
  private _publicUserInfor: PublicUserInfo;

  constructor(
    private userRepository: UserRepository,
    private learningCatalogRepository: LearningCatalogRepository,
    private formApiService: StandaloneSurveyApiService,
    private formAnswerApiService: StandaloneSurveyAnswerApiService,
    private globalSpinnerService: GlobalSpinnerService
  ) {}

  public loadData(formId: string, resourceId: string | null): Observable<[SurveyWithQuestionsModel, SurveyAnswerModel[]]> {
    return combineLatest([
      this.formApiService.getSurveyWithQuestionsById(formId),
      this.formAnswerApiService.getByFormId(formId, resourceId)
    ]).pipe(
      map(formAnswer => {
        this._formDataSubject.next(formAnswer[0]);
        this._formAnswersDataSubject.next(formAnswer[1]);
        this.globalSpinnerService.hide(true);
        return formAnswer;
      })
    );
  }

  public startNewFormAnswer(
    formId: string,
    resourceId?: string,
    initalFormAnswer: SurveyAnswerModel = null
  ): Observable<SurveyAnswerModel> {
    if (initalFormAnswer) {
      return of(this.proceedInitialPreviewData(initalFormAnswer));
    }
    return this.formAnswerApiService
      .saveFormAnswer({
        formId: formId,
        resourceId: resourceId
      })
      .pipe(
        map(formAnswer => {
          return this.proceedInitialPreviewData(formAnswer);
        })
      );
  }

  public updateFormAnswer(dto: IUpdateSurveyAnswerRequest, updatedFormAnswer: SurveyAnswerModel): Observable<SurveyAnswerModel> {
    if (updatedFormAnswer) {
      return of(this.proceedUpdatePreviewData(updatedFormAnswer));
    }
    return this.formAnswerApiService.updateFormAnswer(dto).pipe(
      map(formAnswer => {
        return this.proceedUpdatePreviewData(formAnswer);
      })
    );
  }

  public applyToPreparedPopulate(questionTitle: string): Promise<string> {
    if (this.populatedFields.some(items => questionTitle.includes(items.disabledShortCode))) {
      return new Promise(populateResolve => {
        this.populatedFields.forEach(element => {
          questionTitle = questionTitle.split(element.disabledShortCode).join(element.tempShortCode);
        });
        return populateResolve(questionTitle);
      });
    } else {
      return Promise.resolve(questionTitle);
    }
  }

  public applyToDisabledPopuplatedFields(questionTitle: string): Promise<string> {
    if (this.populatedFields.some(items => questionTitle.includes(items.tempShortCode))) {
      return new Promise(populateResolve => {
        this.populatedFields.forEach(element => {
          questionTitle = questionTitle.split(element.tempShortCode).join(element.shortCode);
        });
        return populateResolve(questionTitle);
      });
    } else {
      return Promise.resolve(questionTitle);
    }
  }

  public applyPopulatedFields(questionTitle: string): Promise<string> {
    if (this.populatedFields.some(items => questionTitle.includes(items.shortCode))) {
      return new Promise(userInforResolve => {
        if (!this._publicUserInfor) {
          this.userRepository.loadPublicUserInfoList({ userIds: [UserInfoModel.getMyUserInfo().id] }).subscribe(publicUserInfor => {
            this._publicUserInfor = publicUserInfor[0];
            return this.mapPopulatedFields(questionTitle).then(newQuestionTitle => {
              return userInforResolve(newQuestionTitle);
            });
          });
        } else {
          return this.mapPopulatedFields(questionTitle).then(newQuestionTitle => {
            return userInforResolve(newQuestionTitle);
          });
        }
      });
    } else {
      return Promise.resolve(questionTitle);
    }
  }

  public mapPopulatedFields(questionTitle: string): Promise<string> {
    questionTitle = questionTitle.split('[name]').join(this._publicUserInfor.fullName);
    questionTitle = questionTitle.split('[email]').join(this._publicUserInfor.emailAddress);
    questionTitle = questionTitle.split('[placeofwork]').join(this._publicUserInfor.departmentName);
    questionTitle = questionTitle.split('[accounttype]').join(this._publicUserInfor.getAccountTypeDisplayText());

    const teachingLevelPromise = new Promise<void>(teachingLevelPromiseResolve => {
      if (questionTitle.includes('[teachinglevel]')) {
        this.learningCatalogRepository.loadUserTeachingLevels().subscribe(recievedTeachingLevels => {
          questionTitle = questionTitle
            .split('[teachinglevel]')
            .join(
              recievedTeachingLevels && recievedTeachingLevels.length
                ? this._publicUserInfor.getTeachingLevelDisplayText(Utils.toDictionary(recievedTeachingLevels, p => p.id)).join(', ')
                : ''
            );
          teachingLevelPromiseResolve();
        });
      } else {
        teachingLevelPromiseResolve();
      }
    });

    const teachingSubjectPromise = new Promise<void>(teachingSubjectPromiseResolve => {
      if (questionTitle.includes('[teachingsubject_or_jobfamily]')) {
        combineLatest(
          this.learningCatalogRepository.loadUserTeachingSubjects(),
          this.learningCatalogRepository.loadUserJobFamiles()
        ).subscribe(combineResult => {
          const teachingSubjectResult =
            combineResult[0] && combineResult[0].length
              ? this._publicUserInfor.getTeachingSubjectDisplayText(Utils.toDictionary(combineResult[0], p => p.id)).join(', ')
              : '';

          const jobFamiliResult =
            combineResult[1] && combineResult[1].length
              ? this._publicUserInfor.getJobFalimyDisplayText(Utils.toDictionary(combineResult[1], p => p.id)).join(', ')
              : '';

          const combine = teachingSubjectResult.concat(jobFamiliResult);

          questionTitle = questionTitle.split('[teachingsubject_or_jobfamily]').join(combine);
          teachingSubjectPromiseResolve();
        });
      } else {
        teachingSubjectPromiseResolve();
      }
    });

    const designationPromise = new Promise<void>(designationPromiseResolve => {
      if (questionTitle.includes('[designation]')) {
        this.learningCatalogRepository.loadUserDesignationList().subscribe(recievedDesignations => {
          questionTitle = questionTitle
            .split('[designation]')
            .join(
              recievedDesignations && recievedDesignations.length
                ? this._publicUserInfor.getDesignationDisplayText(Utils.toDictionary(recievedDesignations, p => p.id))
                : ''
            );
          designationPromiseResolve();
        });
      } else {
        designationPromiseResolve();
      }
    });

    return Promise.all([teachingLevelPromise, teachingSubjectPromise, designationPromise]).then(() => {
      return Promise.resolve(questionTitle);
    });
  }

  private proceedInitialPreviewData(initialFormAnswerModel: SurveyAnswerModel): SurveyAnswerModel {
    const currentFormAnswersData = this._formAnswersDataSubject.value !== undefined ? Utils.clone(this._formAnswersDataSubject.value) : [];
    const newFormAnswersData = [initialFormAnswerModel].concat(currentFormAnswersData);
    this._formAnswersDataSubject.next(newFormAnswersData);
    this.globalSpinnerService.hide(true);
    return initialFormAnswerModel;
  }

  private proceedUpdatePreviewData(updatedFormAnswer: SurveyAnswerModel): SurveyAnswerModel {
    this._formAnswersDataSubject.next(
      Utils.replaceOne(this._formAnswersDataSubject.value, updatedFormAnswer, item => item.id === updatedFormAnswer.id)
    );
    this.globalSpinnerService.hide(true);
    return updatedFormAnswer;
  }
}
