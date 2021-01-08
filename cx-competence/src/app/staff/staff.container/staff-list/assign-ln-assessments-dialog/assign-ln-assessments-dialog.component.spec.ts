import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignLnAssessmentsDialogComponent } from './assign-ln-assessments-dialog.component';
import { NO_ERRORS_SCHEMA, ChangeDetectionStrategy } from '@angular/core';
import { LnAssessmentsDataService } from './ln-assessments-data.service';
import { UserService } from 'app-services/user.service';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import { of, Observable } from 'rxjs';
import {
  TranslateService,
  USE_STORE,
  USE_DEFAULT_LANG,
  MissingTranslationHandler,
  TranslateCompiler,
  TranslateParser,
  TranslateLoader,
  TranslateStore,
  TranslatePipe,
  TranslateModule,
} from '@ngx-translate/core';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {
  CxLoaderModule,
  CxLoaderModuleConfig,
} from '@conexus/cx-angular-common';
import { IdpStatusEnum } from 'app/individual-development/idp.constant';

describe('AssignLnAssessmentsDialogComponent', () => {
  let component: AssignLnAssessmentsDialogComponent;
  let fixture: ComponentFixture<AssignLnAssessmentsDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [AssignLnAssessmentsDialogComponent],
      schemas: [NO_ERRORS_SCHEMA],
      imports: [
        HttpClientTestingModule,
        CxLoaderModule,
        TranslateModule.forRoot(),
        ToastrModule.forRoot(),
      ],
      providers: [HttpHelpers, CxLoaderModuleConfig, LnAssessmentsDataService],
    }).overrideComponent(AssignLnAssessmentsDialogComponent, {
      set: { changeDetection: ChangeDetectionStrategy.Default },
    });

    TestBed.overrideTemplate(AssignLnAssessmentsDialogComponent, '<div></div>');
  }));

  describe('Assign to just users in list', () => {
    beforeEach(() => {
      fixture = TestBed.createComponent(AssignLnAssessmentsDialogComponent);
      component = fixture.componentInstance;
      component.selectedEmployees = [
        {
          assessmentInfos: {
            LearningNeed: {
              statusInfo: {
                assessmentStatusId: IdpStatusEnum.NotAdded,
              },
            },
          },
        },
        {
          assessmentInfos: {
            LearningNeed: {
              statusInfo: {
                assessmentStatusId: IdpStatusEnum.Approved,
              },
            },
          },
        },
      ];
      fixture.detectChanges();
    });

    it('should create', () => {
      expect(component).toBeTruthy();
    });

    it('should have 1 unAssignedSelectedEmployee', () => {
      component.ngOnInit();
      expect(component.unassignedSelectedEmployees.length).toEqual(1);
    });
  });
});
