import { Component, Input, OnInit, ViewChild } from '@angular/core';
import {
  CxSurveyjsComponent,
  CxSurveyjsEventModel,
  CxSurveyJsModeEnum,
} from '@conexus/cx-angular-common';
import { IdpDto } from 'app/organisational-development/models/idp.model';
import { cloneDeep } from 'lodash';

@Component({
  selector: 'learning-needs-preview-answer',
  templateUrl: './learning-needs-preview-answer.component.html',
  styleUrls: ['./learning-needs-preview-answer.component.scss'],
})
export class LearningNeedsPreviewAnswerComponent implements OnInit {
  @Input() learningNeeds: IdpDto;
  @Input() learningNeedsConfig: any;

  answer: any;
  surveyForm: any;

  @ViewChild('surveyReviewAnswer', { static: true })
  private surveyComp: CxSurveyjsComponent;

  constructor() {}

  ngOnInit(): void {
    this.initSurveyForm();
  }

  onAfterSurveyRendered(event: CxSurveyjsEventModel): void {
    this.surveyComp.changeMode(CxSurveyJsModeEnum.Display);
  }

  reloadSurvey(): void {
    setTimeout(() => {
      this.answer = cloneDeep(this.learningNeeds.answer);
    });
  }

  private initSurveyForm(): void {
    this.surveyForm = cloneDeep(this.learningNeedsConfig);
    this.surveyForm.pages = this.flattenPages(this.surveyForm.pages);
    this.fixMissingCareerAspirationPanelTitle();
  }

  private fixMissingCareerAspirationPanelTitle(): void {
    const careerAspirationPanel = this.surveyForm.pages[0].elements.find(
      (e) => e.name === 'CareerAspirationPanel'
    );
    if (careerAspirationPanel && !careerAspirationPanel.title) {
      careerAspirationPanel.title = this.surveyForm.pages[0].name;
    }
  }

  private flattenPages(pages: any[]): any[] {
    const numberOfPages = pages.length;
    if (numberOfPages <= 1) {
      return pages;
    }

    const clonedPages = cloneDeep(pages);
    const firstPage = clonedPages[0];
    for (let pageIndex = 1; pageIndex < numberOfPages; pageIndex++) {
      const page = clonedPages[pageIndex];
      const elements = page.elements;
      if (page.name === 'AO comment' || !elements) {
        continue;
      } // TODO: Remove the "AO comment" page.

      for (const element of elements) {
        firstPage.elements.push(element);
      }
    }

    return [firstPage];
  }
}
