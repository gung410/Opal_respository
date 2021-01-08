export interface ILectureIdMapNameModel {
  lectureId: string;
  name: string;
}

export class LectureIdMapNameModel {
  public lectureId: string;
  public name: string;
  constructor(data?: ILectureIdMapNameModel) {
    if (!data) {
      return;
    }
    this.lectureId = data.lectureId;
    this.name = data.name;
  }
}
