import { Assessment, AssessmentStatus, AssessmentType } from './../models/assessment.model';
import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';

import { Injectable } from '@angular/core';
import { SearchAssessmentResult } from './../dtos/search-assessment-result';
import { map } from 'rxjs/operators';
import { of } from 'rxjs';

@Injectable()
export class AssessmentApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.formApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public searchAssessments(
    skipCount: number | null = 0,
    maxResultCount: number | null = 10,
    showSpinner?: boolean
  ): Promise<SearchAssessmentResult> {
    return of({
      totalCount: 1,
      items: [
        {
          id: '069b187c-a8aa-4cc9-baae-addfba02f828',
          name: 'Assessment 1',
          status: AssessmentStatus.Published,
          type: AssessmentType.Analytic,
          criteria: [
            {
              id: '31739602-f0a0-4396-8102-d8d89c0d09c7',
              name: 'Criteria 1',
              scales: [
                {
                  id: '332d0324-83e2-420e-ae4e-f0c3c0204aaa',
                  scaleId: 'e06dd5ec-71ee-4585-8fd8-78b1e53929db',
                  content: 'Criteria-Scale 1-1'
                },
                {
                  id: '332d0324-83e2-420e-ae4e-f0c3c0204aac',
                  scaleId: 'e06dd5ec-71ee-4585-8fd8-78b1e53929dc',
                  content: 'Criteria-Scale 1-2'
                }
              ]
            },
            {
              id: '31739602-f0a0-4396-8102-d8d89c0d09c8',
              name: 'Criteria 2',
              scales: [
                {
                  id: '332d0324-83e2-420e-ae4e-f0c3c0204aa1',
                  scaleId: 'e06dd5ec-71ee-4585-8fd8-78b1e53929db',
                  content: 'Criteria-Scale 2-1'
                },
                {
                  id: '332d0324-83e2-420e-ae4e-f0c3c0204aa2',
                  scaleId: 'e06dd5ec-71ee-4585-8fd8-78b1e53929dc',
                  content: 'Criteria-Scale 2-2'
                }
              ]
            }
          ],
          scales: [
            {
              id: 'e06dd5ec-71ee-4585-8fd8-78b1e53929db',
              name: 'Scale 1',
              value: 10
            },
            {
              id: 'e06dd5ec-71ee-4585-8fd8-78b1e53929dc',
              name: 'Scale 2',
              value: 20
            }
          ]
        }
      ]
    })
      .pipe(
        map(_ => {
          return new SearchAssessmentResult(_);
        })
      )
      .toPromise();
  }

  public getAssessmentById(id: string, showSpinner?: boolean): Promise<Assessment> {
    return of({
      id: '069b187c-a8aa-4cc9-baae-addfba02f828',
      name: 'Assessment 1',
      status: AssessmentStatus.Published,
      type: AssessmentType.Analytic,
      criteria: [
        {
          id: '31739602-f0a0-4396-8102-d8d89c0d09c7',
          name: 'Criteria 1',
          scales: [
            {
              id: '332d0324-83e2-420e-ae4e-f0c3c0204aaa',
              scaleId: 'e06dd5ec-71ee-4585-8fd8-78b1e53929db',
              content: 'Criteria-Scale 1-1'
            },
            {
              id: '332d0324-83e2-420e-ae4e-f0c3c0204aac',
              scaleId: 'e06dd5ec-71ee-4585-8fd8-78b1e53929dc',
              content: 'Criteria-Scale 1-2'
            }
          ]
        },
        {
          id: '31739602-f0a0-4396-8102-d8d89c0d09c8',
          name: 'Criteria 2',
          scales: [
            {
              id: '332d0324-83e2-420e-ae4e-f0c3c0204aa1',
              scaleId: 'e06dd5ec-71ee-4585-8fd8-78b1e53929db',
              content: 'Criteria-Scale 2-1'
            },
            {
              id: '332d0324-83e2-420e-ae4e-f0c3c0204aa2',
              scaleId: 'e06dd5ec-71ee-4585-8fd8-78b1e53929dc',
              content: 'Criteria-Scale 2-2'
            }
          ]
        }
      ],
      scales: [
        {
          id: 'e06dd5ec-71ee-4585-8fd8-78b1e53929db',
          name: 'Scale 1',
          value: 10
        },
        {
          id: 'e06dd5ec-71ee-4585-8fd8-78b1e53929dc',
          name: 'Scale 2',
          value: 20
        }
      ]
    })
      .pipe(map(data => new Assessment(data)))
      .toPromise();
  }
}
