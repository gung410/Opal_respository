import { ChangeDetectorRef, Component, Input, OnInit } from '@angular/core';
import { ClassRunModel } from 'app-models/classrun.model';
import { LearnerInfoDTO } from 'app-models/common/learner-info.model';
import { SessionModel } from 'app-models/session.model';
import { UserService } from 'app-services/user.service';
import { Dictionary } from 'lodash';

@Component({
  selector: 'classrun-info',
  templateUrl: './classrun-info.component.html',
  styleUrls: ['./classrun-info.component.scss'],
})
export class ClassrunInfoComponent implements OnInit {
  @Input() classrunDetail: ClassRunModel;
  @Input() sessions: SessionModel[];
  @Input() isExpanding: boolean = true;
  public isExpandingClassRun: boolean = true;
  public expandingSessions: Dictionary<boolean> = {};
  public upcomingSession: SessionModel;
  public facilitators: LearnerInfoDTO[] = [];

  constructor(
    private userService: UserService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.getFaciliator();
  }

  public onToggleSession(sessionId: string): void {
    this.expandingSessions[sessionId] = !this.expandingSessions[sessionId];
  }

  public async getFaciliator(): Promise<void> {
    if (
      this.classrunDetail &&
      this.classrunDetail.facilitatorIds &&
      this.classrunDetail.facilitatorIds.length > 0
    ) {
      const dto = await this.userService.getLearnerInfoAsync(
        this.classrunDetail.facilitatorIds.map((p) => p)
      );

      this.facilitators = dto.data;
    }
    this.changeDetectorRef.detectChanges();
  }

  public getDateTime(startDate: Date, startTime: Date): Date {
    const datetime = startDate;
    datetime.setHours(startTime.getHours());
    datetime.setMinutes(startTime.getMinutes());

    return datetime;
  }
}
