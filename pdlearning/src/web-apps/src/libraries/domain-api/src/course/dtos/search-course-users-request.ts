export interface ISearchCourseUserRequest {
  searchText: string;
  forCourse?: ISearchUsersQueryForCourseInfo;
  skipCount: number;
  maxResultCount: number;
}

export interface ISearchUsersQueryForCourseInfo {
  courseId: string;
  followCourseTargetParticipant: boolean;
}
