import { DateUtils, Utils } from '@opal20/infrastructure';

import { CoursePlanningCycle } from '@opal20/domain-api';
import { CoursePlanningCycleViewModel } from '../models/course-planning-cycle-view.model';

export class CoursePlanningCycleDetailViewModel {
  public coursePlanningCycleData: CoursePlanningCycle = new CoursePlanningCycle();
  public originCoursePlanningCycleData: CoursePlanningCycle = new CoursePlanningCycle();
  public yearCycleItems: number[] = [];
  //#endregion

  constructor(coursePlanningCycle?: CoursePlanningCycle, allExistedPlanningCycles: CoursePlanningCycleViewModel[] = []) {
    if (coursePlanningCycle) {
      this.updateCoursePlanningCycleData(coursePlanningCycle);
    }

    this.yearCycleItems = this.buildYearCycleItems(allExistedPlanningCycles.map(i => i.yearCycle));
  }

  public get title(): string {
    return this.coursePlanningCycleData.title;
  }
  public set title(title: string) {
    this.coursePlanningCycleData.title = title;
  }

  public get yearCycle(): number {
    return this.coursePlanningCycleData.yearCycle;
  }
  public set yearCycle(yearCycle: number) {
    this.coursePlanningCycleData.yearCycle = yearCycle;
  }

  public get startDate(): Date {
    return this.coursePlanningCycleData.startDate;
  }
  public set startDate(startDate: Date) {
    this.coursePlanningCycleData.startDate = startDate;
  }

  public get endDate(): Date {
    return this.coursePlanningCycleData.endDate;
  }
  public set endDate(endDate: Date) {
    this.coursePlanningCycleData.endDate = endDate;
  }

  public get description(): string {
    return this.coursePlanningCycleData.description;
  }
  public set description(description: string) {
    this.coursePlanningCycleData.description = description;
  }

  public get isConfirmedBlockoutDate(): boolean {
    return this.coursePlanningCycleData.isConfirmedBlockoutDate;
  }

  public set isConfirmedBlockoutDate(isConfirmedBlockoutDate: boolean) {
    this.coursePlanningCycleData.isConfirmedBlockoutDate = isConfirmedBlockoutDate;
  }

  public canEditStartDate(): boolean {
    return (
      this.originCoursePlanningCycleData.isConfirmedBlockoutDate &&
      (this.originCoursePlanningCycleData.startDate == null ||
        DateUtils.removeTime(new Date()) < DateUtils.removeTime(this.originCoursePlanningCycleData.startDate))
    );
  }

  public updateCoursePlanningCycleData(coursePlanningCycle: CoursePlanningCycle): void {
    this.originCoursePlanningCycleData = Utils.cloneDeep(coursePlanningCycle);
    this.coursePlanningCycleData = Utils.cloneDeep(coursePlanningCycle);
  }

  public dataHasChanged(): boolean {
    return Utils.isDifferent(this.originCoursePlanningCycleData, this.coursePlanningCycleData);
  }

  private buildYearCycleItems(existedItems: number[]): number[] {
    const currentYear = new Date().getFullYear();
    const range = currentYear + 20;
    const items: number[] = [];
    for (let i = currentYear; i <= range; i++) {
      if (
        (existedItems && !existedItems.includes(i)) ||
        (this.coursePlanningCycleData.yearCycle && this.coursePlanningCycleData.yearCycle === i)
      ) {
        items.push(i);
      }
    }

    return items;
  }
}
