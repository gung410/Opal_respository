import { IVideoChapter } from '../models/video-chapter.model';

export interface IVideoChapterSaveRequest {
  objectId: string;
  chapters: IVideoChapter[];
}
