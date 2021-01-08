import { SectionModel } from '@opal20/domain-api';
import { Utils } from '@opal20/infrastructure';

export class SectionDetailViewModel {
  public data: SectionModel = new SectionModel();
  public originalData: SectionModel = new SectionModel();

  constructor(section?: SectionModel) {
    if (section) {
      this.updateSectionData(section);
    }
  }

  public get title(): string {
    return this.data.title;
  }
  public set title(title: string) {
    this.data.title = title;
  }

  public get description(): string {
    return this.data.description;
  }
  public set description(description: string) {
    this.data.description = description;
  }
  public get creditsAward(): number {
    return this.data.creditsAward;
  }
  public set creditsAward(creditsAward: number) {
    this.data.creditsAward = creditsAward;
  }

  public updateSectionData(section: SectionModel): void {
    this.originalData = Utils.cloneDeep(section);
    this.data = Utils.cloneDeep(section);
  }

  public reset(): void {
    this.data = Utils.cloneDeep(this.originalData);
  }
}
