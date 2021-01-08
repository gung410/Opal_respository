export interface CourseContentItemDTO {
  id: string;
  parentId?: string;
  title: string;
  icon?: string | undefined;
  type: CourseContentItemType;
  items?: CourseContentItemDTO[] | undefined;
  order?: number;
}

export class CourseContentItemModel {
  public id: string;
  public parentId?: string;
  public title: string;
  public icon?: string | undefined;
  public type: CourseContentItemType;
  public items: CourseContentItemDTO[] = [];
  public order?: number;

  constructor(dto?: CourseContentItemDTO) {
    if (dto == null) {
      return;
    }
    this.id = dto.id;
    this.parentId = dto.parentId;
    this.title = dto.title;
    this.icon = dto.icon;
    this.type = dto.type;
    this.items =
      dto.items != null
        ? dto.items.map((p) => new CourseContentItemModel(p))
        : [];
    this.order = dto.order;
  }
}

export enum CourseContentItemType {
  Course = 'Course',
  Section = 'Section',
  Lecture = 'Lecture',
  Assignment = 'Assignment',
}
