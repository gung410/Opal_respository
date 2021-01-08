import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import {
  Course,
  CourseCriteria,
  CourseCriteriaRepository,
  CourseRepository,
  ISaveCourseCriteriaRequest,
  LearningCatalogRepository,
  OrganizationRepository,
  TaggingRepository
} from '@opal20/domain-api';
import { CourseCriteriaDetailMode, CourseCriteriaDetailViewModel } from '@opal20/domain-components';
import { Observable, Subscription, combineLatest, from } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
@Component({
  selector: 'course-criteria-detail-page',
  templateUrl: './course-criteria-detail-page.component.html'
})
export class CourseCriteriaDetailPageComponent extends BaseFormComponent {
  @Input() public courseCriteriaId: string;
  @Input() public mode: CourseCriteriaDetailMode;
  @Input() public course: Course;
  @Input() public stickyDependElement: HTMLElement;
  @Input() public stickySpacing: number;

  public courseCriteria: CourseCriteriaDetailViewModel = new CourseCriteriaDetailViewModel();
  private loadCourseCriteriaInfoSub = new Subscription();
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private courseCriteriaRepository: CourseCriteriaRepository,
    private taggingRepository: TaggingRepository,
    private courseRepository: CourseRepository,
    private organizationRepository: OrganizationRepository,
    private learningRepository: LearningCatalogRepository
  ) {
    super(moduleFacadeService);
  }
  public loadCourseCriteriaInfo(): void {
    this.loadCourseCriteriaInfoSub.unsubscribe();

    const courseCriteriaObs = this.courseCriteriaRepository.loadCourseCriteria(this.courseCriteriaId);
    const taggingObs = this.taggingRepository.loadAllMetaDataTags();
    const departmentsUnitsObs = this.learningRepository.loadUserTypeOfOrganizationList();
    const departmentsLevelsObs = this.organizationRepository.loadOrganizationalLevels();

    this.loadCourseCriteriaInfoSub = combineLatest(courseCriteriaObs, taggingObs, departmentsUnitsObs, departmentsLevelsObs)
      .pipe(
        switchMap(([courseCriteria, metadata, departmentsUnits, departmentsLevels]) =>
          CourseCriteriaDetailViewModel.create(
            courseCriteria,
            metadata,
            departmentsUnits,
            departmentsLevels,
            ids => this.organizationRepository.loadOrganizationalUnitsByIds(ids),
            ids => this.courseRepository.loadCourses(ids)
          )
        ),
        this.untilDestroy()
      )
      .subscribe(_ => {
        this.courseCriteria = _;
      });
  }

  public validateAndSaveCourseCriteria(): Observable<void> {
    return from(
      new Promise<void>((resolve, reject) => {
        this.validate().then(valid => {
          if (valid) {
            this.saveCourseCriteria().then(_ => {
              this.showNotification();
              resolve();
            }, reject);
          } else {
            reject();
          }
        });
      })
    );
  }

  public reuseCourseExistingDataForCourseCriteria(): void {
    this.courseCriteria.reuseCourseExistingData(this.course);
  }

  protected onInit(): void {
    this.loadCourseCriteriaInfo();
  }
  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'course-criteria-detail',
      controls: {
        accountType: {
          defaultValue: null,
          validators: null
        },
        serviceScheme: {
          defaultValue: null,
          validators: null
        },
        selectedServiceScheme: {
          defaultValue: null,
          validators: null
        },
        track: {
          defaultValue: null,
          validators: null
        },
        developmentRole: {
          defaultValue: null,
          validators: null
        },
        teachingLevel: {
          defaultValue: null,
          validators: null
        },
        teachingCourseOfStudy: {
          defaultValue: null,
          validators: null
        },
        jobFamily: {
          defaultValue: null,
          validators: null
        },
        coCurricularActivity: {
          defaultValue: null,
          validators: null
        },
        subGradeBanding: {
          defaultValue: null,
          validators: null
        },
        placeOfWork: {
          defaultValue: null,
          validators: null
        },
        deparmentUnitTypes: {
          defaultValue: null,
          validators: null
        },
        deparmentLevelTypes: {
          defaultValue: null,
          validators: null
        },
        specificDeparments: {
          defaultValue: null,
          validators: null
        },
        selectedPlaceOfWork: {
          defaultValue: null,
          validators: null
        },
        preRequisiteCourse: {
          defaultValue: null,
          validators: null
        }
      }
    };
  }

  private saveCourseCriteria(): Promise<CourseCriteria> {
    return new Promise((resolve, reject) => {
      const request: ISaveCourseCriteriaRequest = {
        data: Utils.clone(this.courseCriteria.courseCriteriaData, cloneData => {
          cloneData.id = this.courseCriteriaId;
        })
      };
      this.courseCriteriaRepository
        .saveCourseCriteria(request)
        .pipe(
          map(courseCriteria => {
            this.courseCriteria.courseCriteriaData.id = courseCriteria.id;
            return courseCriteria;
          }),
          this.untilDestroy()
        )
        .subscribe(courseCriteria => {
          resolve(courseCriteria);
        }, reject);
    });
  }
}
