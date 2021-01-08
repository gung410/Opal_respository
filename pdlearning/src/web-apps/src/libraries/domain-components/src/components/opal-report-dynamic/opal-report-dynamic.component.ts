import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, HostListener, Input, ViewEncapsulation } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { IOpalReportDynamicParams, OpalReportDynamicParamsSchema } from '../../models/opal-report-dynamic-params.model';

import { EncryptUrlPathHelper } from '../../helpers/encrypt-url-path.helper';

@Component({
  selector: 'opal-report-dynamic',
  templateUrl: './opal-report-dynamic.component.html',
  encapsulation: ViewEncapsulation.None
})
export class OpalReportDynamicComponent extends BaseComponent {
  public safeUrl: SafeResourceUrl = '';
  public height: string = 'auto';

  private _paramsReportDynamic: IOpalReportDynamicParams | null;
  public get paramsReportDynamic(): IOpalReportDynamicParams | null {
    return this._paramsReportDynamic;
  }

  @Input()
  public set paramsReportDynamic(v: IOpalReportDynamicParams | null) {
    this._paramsReportDynamic = v;
  }

  public static buildListOfApplicationStatusReportParams(courseID: string, isMicrolearning: boolean = false): IOpalReportDynamicParams {
    if (isMicrolearning) {
      return {
        reportName: 'MLUStatistic_SpecificallyCourse',
        filter: `CourseId eq ${courseID} and Status eq 'Published'`
      };
    }

    return {
      reportName: 'ApplicationStatus_V2',
      reportDisplayName: 'Application Status',
      filter: `CourseId eq ${courseID}`
    };
  }

  public static buildSummaryDataByMLUParams(): IOpalReportDynamicParams {
    return {
      reportName: 'Summary data by MLU',
      schema: OpalReportDynamicParamsSchema.external,
      viewName: 'MLUStatistic_ListAll',
      select:
        'CourseName,NumberOfRating,AverageRating,TotalLearners,NumOfUserCompleted,CompletedRate' +
        ',NumOfUserInprogress,InprogressRate,NumOfUserNotStarted,NotStartedRate',
      colPatterns_0: '4|{{NumOfUserCompleted}} ({{CompletedRate}}%)',
      colPatterns_1: '6|{{NumOfUserInprogress}} ({{InprogressRate}}%)',
      colPatterns_2: '8|{{NumOfUserNotStarted}} ({{NotStartedRate}}%)',
      colformats_0: '5|d-none',
      colformats_1: '7|d-none',
      colformats_2: '9|d-none',
      filter: "Status eq 'Published'"
    };
  }

  public static buildAssignmentTracking(assignmentId: string): IOpalReportDynamicParams {
    return {
      reportName: 'AssignmentTrackingProgress',
      filter: `AssignmentId eq ${assignmentId}`
    };
  }

  public static buildAttendanceSummaryByPDOpportunity(
    ids: string[],
    courseId: string,
    classrunId: string,
    sessionId: string,
    status: string
  ): IOpalReportDynamicParams {
    let idsparams = '';
    // Check list have id checked
    for (let i = 0; i < ids.length; i++) {
      if (i === 0) {
        idsparams += ' and ';
      } else if (i < ids.length) {
        idsparams += ' or ';
      }

      idsparams += 'userId eq ' + ids[i];
    }

    let statusName = '';
    if (status !== undefined && status !== '') {
      statusName = ` and status eq '${status}'`;
    }

    return {
      reportName: 'AttendanceTracking',
      filter: `courseId eq ${courseId} and classrunId eq ${classrunId} and sessionId eq ${sessionId}` + statusName + `${idsparams}`
    };
  }

  public static buildCourseCompletion(courseId: string, classrunId: string): IOpalReportDynamicParams {
    return {
      reportName: 'CourseCompletion',
      filter: `courseId eq ${courseId} and classrunId eq ${classrunId}`
    };
  }

  public static buildLearnerReportList(): IOpalReportDynamicParams {
    return {
      reportName: 'LearnerReports'
    };
  }

  public static buildCamReportList(): IOpalReportDynamicParams {
    return {
      reportName: 'CAMReports'
    };
  }

  public static buildLmmReportList(): IOpalReportDynamicParams {
    return {
      reportName: 'LMMReports'
    };
  }

  public static buildCCPMReportList(): IOpalReportDynamicParams {
    return {
      reportName: 'CCPMReports'
    };
  }

  constructor(protected moduleFacadeService: ModuleFacadeService, private domSanitizer: DomSanitizer) {
    super(moduleFacadeService);
  }

  @HostListener('window:message', ['$event'])
  public onMessage(event: MessageEvent): void {
    this.receiveMessageFromIframe(event);
  }

  public ngOnChanges(): void {
    this.buildUrlIframe();
  }

  protected onInit(): void {
    this.buildUrlIframe();
  }

  private buildUrlIframe(): void {
    const url = AppGlobal.environment.reportUrl + '/?q=';
    const paramsReport = [];
    for (const key in this.paramsReportDynamic) {
      const value = this.paramsReportDynamic[key];

      if (this.paramsReportDynamic.hasOwnProperty(key)) {
        const keySplit = key.split('_')[0];
        paramsReport.push(encodeURI(keySplit + '=' + value));
      }
    }

    // Encrypt AES querystring url
    const encrypted = EncryptUrlPathHelper.encrypt('dynamic/content?' + paramsReport.join('&'));

    this.safeUrl = this.domSanitizer.bypassSecurityTrustResourceUrl(url + encrypted);
  }

  private receiveMessageFromIframe(event: MessageEvent): void {
    const data = event.data.params;

    if (!data) {
      return;
    }

    if (!data.height) {
      return;
    }

    this.height = data.height;
  }
}
