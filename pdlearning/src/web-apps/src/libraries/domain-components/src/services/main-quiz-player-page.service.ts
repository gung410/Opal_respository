import { BehaviorSubject, Observable, combineLatest, of } from 'rxjs';
import {
  FormAnswerApiService,
  FormAnswerModel,
  FormApiService,
  FormWithQuestionsModel,
  IUpdateFormAnswerRequest,
  LearningCatalogRepository,
  PublicUserInfo,
  UserInfoModel,
  UserRepository
} from '@opal20/domain-api';
import { GlobalSpinnerService, Utils } from '@opal20/infrastructure';

import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

@Injectable()
export class MainQuizPlayerPageService {
  public get formData$(): Observable<FormWithQuestionsModel | undefined> {
    return this._formDataSubject.asObservable();
  }
  public get formAnswersData$(): Observable<FormAnswerModel[] | undefined> {
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

  private _formDataSubject: BehaviorSubject<FormWithQuestionsModel | undefined> = new BehaviorSubject(undefined);
  private _formAnswersDataSubject: BehaviorSubject<FormAnswerModel[] | undefined> = new BehaviorSubject(undefined);
  private _publicUserInfor: PublicUserInfo;

  constructor(
    private userRepository: UserRepository,
    private learningCatalogRepository: LearningCatalogRepository,
    private formApiService: FormApiService,
    private formAnswerApiService: FormAnswerApiService,
    private globalSpinnerService: GlobalSpinnerService
  ) {}

  public loadData(
    formId: string,
    resourceId: string | null,
    myCourseId: string | null,
    classRunId: string | null,
    assignmentId: string | null
  ): Observable<[FormWithQuestionsModel, FormAnswerModel[]]> {
    return combineLatest([
      this.formApiService.getFormWithQuestionsById(formId),
      this.formAnswerApiService.getByFormId(formId, resourceId, myCourseId, classRunId, assignmentId)
    ]).pipe(
      map(([formQuestion, formAnswers]) => {
        this._formDataSubject.next(formQuestion);
        this._formAnswersDataSubject.next(formAnswers);
        this.globalSpinnerService.hide(true);
        return [formQuestion, formAnswers];
      })
    );
  }

  public startNewFormAnswer(
    formId: string,
    courseId?: string,
    myCourseId?: string,
    classRunId?: string,
    assignmentId?: string,
    initalFormAnswer: FormAnswerModel = null
  ): Observable<FormAnswerModel> {
    if (initalFormAnswer) {
      return of(this.proceedInitialPreviewData(initalFormAnswer));
    }
    return this.formAnswerApiService
      .saveFormAnswer({
        formId: formId,
        courseId: courseId,
        myCourseId: myCourseId,
        classRunId: classRunId,
        assignmentId: assignmentId
      })
      .pipe(
        map(formAnswer => {
          return this.proceedInitialPreviewData(formAnswer);
        })
      );
  }

  public updateFormAnswer(dto: IUpdateFormAnswerRequest, updatedFormAnswer: FormAnswerModel): Observable<FormAnswerModel> {
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

  private proceedInitialPreviewData(initialFormAnswerModel: FormAnswerModel): FormAnswerModel {
    const currentFormAnswersData = this._formAnswersDataSubject.value !== undefined ? Utils.clone(this._formAnswersDataSubject.value) : [];
    const newFormAnswersData = [initialFormAnswerModel].concat(currentFormAnswersData);
    this._formAnswersDataSubject.next(newFormAnswersData);
    this.globalSpinnerService.hide(true);
    return initialFormAnswerModel;
  }

  private proceedUpdatePreviewData(updatedFormAnswer: FormAnswerModel): FormAnswerModel {
    this._formAnswersDataSubject.next(
      Utils.replaceOne(this._formAnswersDataSubject.value, updatedFormAnswer, item => item.id === updatedFormAnswer.id)
    );
    this.globalSpinnerService.hide(true);
    return updatedFormAnswer;
  }
}
