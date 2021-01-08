import { LectureDigitalContentConfigModel, LectureQuizConfigModel, LectureType } from '../models/lecture.model';

export interface ISaveLectureRequest {
  id?: string;
  lectureIcon: string;
  lectureName: string;
  description: string;
  type: LectureType;
  order: number;
  sectionId?: string;
  courseId: string;
  base64Value?: string;
  mimeType?: string;
  resourceId?: string;
  classRunId?: string;
  quizConfig?: LectureQuizConfigModel;
  digitalContentConfig?: LectureDigitalContentConfigModel;
}
