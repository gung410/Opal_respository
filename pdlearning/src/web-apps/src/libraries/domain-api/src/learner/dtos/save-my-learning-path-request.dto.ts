export interface ISaveMyLearningPath {
  id?: string;
  title: string;
  thumbnailUrl?: string;
  courses: ISaveMyLearningPathCourse[];
  createdBy?: string;
}

export interface ISaveMyLearningPathCourse {
  id?: string;
  courseId: string;
  order?: number;
  learningPathId: string;
}
