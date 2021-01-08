import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import {
  CxGlobalLoaderService,
  CxSurveyJsUtil,
} from '@conexus/cx-angular-common';
import { TranslateService } from '@ngx-translate/core';
import { PDPlanDto } from 'app-models/pdplan.model';
import { DuplicatePDPlanRequest } from 'app/organisational-development/dtos/duplicate-pd-plan.dto';
import { OdpService } from 'app/organisational-development/odp.service';
import { ToastrService } from 'ngx-toastr';
import { OdpActivity } from '../odp.constant';

@Component({
  selector: 'duplicate-learning-direction-dialog',
  templateUrl: './duplicate-learning-direction-dialog.component.html',
  styleUrls: ['./duplicate-learning-direction-dialog.component.scss'],
})
export class DuplicateLearningDirectionDialogComponent implements OnInit {
  @Output() cancel: EventEmitter<void> = new EventEmitter<void>();
  @Output() success: EventEmitter<object> = new EventEmitter<object>();
  jsonForm: any = {
    type: 'panel',
    innerIndent: 1,
    name: 'DuplicateDirection',
    title: 'Duplicate Learning Direction',
    elements: [
      {
        type: 'text',
        name: 'learningDirectionTitle',
        title: 'Learning Direction Name',
        isRequired: true,
        requiredErrorText: 'Learning Direction Name is required',
      },
      {
        type: 'radiogroup',
        name: 'duplicationDirectory',
        title: 'Duplicate in',
        choices: [
          {
            value: 'sameLP',
            text: 'The same Learning Plan',
          },
          {
            value: 'specificLP',
            text: 'Target Learning Plan',
          },
        ],
      },
      {
        type: 'dropdown',
        name: 'selectLP',
        visibleIf: "{duplicationDirectory} = 'specificLP'",
        title: 'Select from the list',
        isRequired: true,
        choices: [],
      },
    ],
  };
  @Input() currentPDPlanDto: PDPlanDto;
  cloneData: DuplicatePDPlanRequest;
  formData: any;
  isFormReady: boolean;
  constructor(
    private router: Router,
    private odpService: OdpService,
    private toastrService: ToastrService,
    private translateService: TranslateService,
    private globalLoader: CxGlobalLoaderService
  ) {}

  ngOnInit(): void {
    this.initJsonForm();
    this.formData = {
      learningDirectionTitle: `Copy of ${this.currentPDPlanDto.answer.Title}`,
      duplicationDirectory: 'sameLP',
    };
  }

  onCancel(): void {
    this.cancel.emit();
  }

  onSubmit($event: any): void {
    const data = $event.survey.data;
    const parentId =
      data.duplicationDirectory === 'sameLP'
        ? this.currentPDPlanDto.parentResultExtId
        : data.selectLP.resultIdentity.extId;
    this.cloneData = {
      sourceIdentity: this.currentPDPlanDto.resultIdentity,
      newAnswer: this.currentPDPlanDto.answer,
      newParentResultExtId: parentId,
    };
    this.cloneData.newAnswer.Title = data.learningDirectionTitle;

    this.odpService
      .duplicatePlan(this.cloneData, OdpActivity.Direction)
      .subscribe(
        (pdplanResult) => {
          if (
            pdplanResult &&
            pdplanResult.length &&
            pdplanResult[0].resultIdentity
          ) {
            const resultExtId = {
              planExtId: parentId,
              directionExtId: pdplanResult[0].resultIdentity.extId,
            };
            this.toastrService.success(
              this.translateService.instant(
                'Odp.DuplicateLearningDirection.SubmitSuccess'
              )
            );
            this.success.emit(resultExtId);
          } else {
            this.toastrService.error(
              this.translateService.instant('Common.Message.APINotCompatible')
            );
          }
        },
        (error: HttpErrorResponse) => {
          // tslint:disable-next-line:no-magic-numbers
          if (error.status === 409) {
            // Conflict
            this.toastrService.warning(
              this.translateService.instant(
                'Odp.DuplicateLearningDirection.SubmitFail'
              )
            );
          } else {
            this.toastrService.error(`${error.error.error}`);
          }
        }
      );
    this.cancel.emit();
  }

  async initJsonForm(): Promise<void> {
    this.globalLoader.showLoader();
    this.odpService
      .getLearningPlanList({
        departmentIds: [this.currentPDPlanDto.objectiveInfo.identity.id],
      })
      .subscribe((result) => {
        if (result) {
          this.globalLoader.hideLoader();
          const choices = result
            .filter(
              (item) =>
                item.resultIdentity.extId !==
                this.currentPDPlanDto.parentResultExtId
            )
            .map((item) => {
              return {
                text: item.answer.Title,
                value: item,
              };
            });
          CxSurveyJsUtil.addProperty(
            this.jsonForm,
            'selectLP',
            'choices',
            choices
          );
          this.isFormReady = true;
        }
      });
  }
}
