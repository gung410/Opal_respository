import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  Input,
  OnInit,
} from '@angular/core';
import {
  CxNode,
  CxSurveyjsEventModel,
  CxSurveyjsVariable,
} from '@conexus/cx-angular-common';
import { AuthService } from 'app-auth/auth.service';
import { PDPlanDto } from 'app-models/pdplan.model';
import { CxLazySurveyjsService } from 'app-services/cx-lazy-surveyjs.service';
import { StartingHierarchyDepartment } from 'app/cx-people-picker/cx-people-picker-dialog/starting-hierarchy-department.enum';
import { CxPeoplePickerService } from 'app/cx-people-picker/cx-people-picker.service';
import { CommentData } from 'app/individual-development/cx-comment/comment.model';
import {
  OdpActivity,
  OdpActivityName,
} from 'app/organisational-development/learning-plan-detail/odp.constant';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import { cloneDeep } from 'lodash';

@Component({
  selector: 'overall-organizational-development-content',
  templateUrl: './overall-organizational-development-content.component.html',
  styleUrls: ['./overall-organizational-development-content.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OverallOrganizationalDevelopmentContentComponent
  extends BaseScreenComponent
  implements OnInit {
  @Input() planNode: CxNode;
  @Input() planFormJSON: any;
  @Input() directionFormJSON: any;
  @Input() programmeFormJSON: any;

  title: string;
  formVariables: CxSurveyjsVariable[];
  comments: CommentData[];

  // flags
  titleNode: CxNode;
  constructor(
    protected changeDetectorRef: ChangeDetectorRef,
    protected authService: AuthService,
    private peoplePickerService: CxPeoplePickerService,
    private cxLazySurveyjsService: CxLazySurveyjsService
  ) {
    super(changeDetectorRef, authService);
    this.formVariables = [];
  }
  ngOnInit(): void {
    this.cxLazySurveyjsService.init();
    if (this.planNode) {
      this.titleNode = cloneDeep(this.planNode);
      this.titleNode.hideChildren = true;
      this.titleNode.children.forEach((element) => {
        element.hideChildren = true;
        element.name = OdpActivityName.LearningDirection;
        if (element.children) {
          element.children.forEach((sub) => {
            sub.name = OdpActivityName.LearningProgramme;
          });
        }
      });
    }
  }
  isKeyLearningProgram(pdplanDto: PDPlanDto): boolean {
    return pdplanDto.pdPlanActivity === OdpActivity.Programme;
  }
  afterQuestionsRendered(event: CxSurveyjsEventModel): void {
    const question = event.options.question;
    const htmlElement = event.options.htmlElement;
    const questionType = question.getType();
    if (questionType === 'cxpeoplepicker') {
      const klpDepartmentId =
        this.planNode.dataObject.pdplanDto &&
        this.planNode.dataObject.pdplanDto.objectiveInfo &&
        this.planNode.dataObject.pdplanDto.objectiveInfo.identity
          ? this.planNode.dataObject.pdplanDto.objectiveInfo.identity.id
          : 0;
      this.peoplePickerService.initQuestionPeoplePicker({
        currentUser: this.currentUser,
        question,
        htmlElement,
        startingHierarchyDepartment:
          StartingHierarchyDepartment.SpecifiedDepartment,
        specificDepartmentId: klpDepartmentId,
        windowClass: 'add-individual-learners-klp-modal',
      });
    }
  }
}
