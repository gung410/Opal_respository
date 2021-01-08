export class OpportunityDTO {
  public courseId: string;
  public name: string;
  public description: string;
  public thumbnailUrl: string;
  public durationInMinutes: number;
  public tags: string[];
  public created: string;
  public modified: string;
  constructor(data?: Partial<OpportunityDTO>) {
    if (!data) {
      return;
    }
    this.courseId = data.courseId ? data.courseId : '';
    this.name = data.name ? data.name : '';
    this.description = data.description ? data.description : '';
    this.thumbnailUrl = data.thumbnailUrl ? data.thumbnailUrl : '';
    this.durationInMinutes = data.durationInMinutes
      ? data.durationInMinutes
      : 0;
    this.created = data.created ? data.created : '';
    this.modified = data.modified ? data.modified : '';
    this.tags = data.tags ? data.tags : [];
  }
}
