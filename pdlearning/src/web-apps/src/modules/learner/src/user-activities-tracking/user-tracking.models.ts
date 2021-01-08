export type UserTrackingEvent = {
  eventName: EventName;
  time: Date;
  sessionId: string;
  userId: string;

  payload: EventPayload;
};

export type EventTrackParam = {
  eventName: EventName;
  payload: EventPayload;
};

export type EventName =
  | ('ReachLandingPage' | 'OutlandingPage')
  | ('ReachPDOPage' | 'OutPDOPage')
  | ('StartLecture' | 'FinishLecture')
  | 'RevisitMLU'
  | 'BookmarkItem'
  | 'LearningTracking'
  | ('PlayAssignment' | 'StopAssignment' | 'AnswerAssignment')
  | ('PlayQuiz' | 'StopQuiz' | 'SubmittedQuizAnswer')
  | 'SearchCatalog';

export type EventPayload =
  | LandingPageEventPayload
  | BookmarkEventPayload
  | LectureEventPayload
  | LearningTrackingEventPayload
  | PDODetailEventPayload
  | AssignmentPlayingEventPayload
  | QuizPlayingEventPlayload
  | SearchCatalogEventPayload;
export type LandingPageEventPayload = {};

export type BookmarkEventPayload = {
  isUnBookmark: boolean;
  courseId: string;
};

export type LectureEventPayload = {
  courseId: string;
  lectureid: string;
};

export type RevisitMLUEventPayload = {
  courseId: string;
  visitMode: 'viewOnly' | 'learnAgain';
};

export type LearningTrackingEventPayload = {
  itemId: string;
  trackingType: 'digitalContent' | 'course' | 'microlearning';
  trackingAction: 'view' | 'downloadContent' | 'like' | 'share';
};

export type PDODetailEventPayload = {
  itemId: string;
  trackingType: 'digitalContent' | 'course' | 'microlearning';
};

export type AssignmentPlayingEventPayload = {
  playingSessionId: string;
  assignmentId: string;
  participantAssignmentTrackId: string;
  quizAssignmentFormQuestionId?: string;
};

export type QuizPlayingEventPlayload = {
  playingSessionId: string;
  formId: string;
  formAnswerId?: string;
};

export type SearchCatalogEventPayload = {
  searchText: string;
  page: number;
  limit: number;
  types: string[]; // ('course' | 'content' | 'microlearning' | 'learningpath' | 'community' | 'createdby' | 'memberships.id')[];
  searchFields: string[]; // currently receives these values: ['title', 'description', 'code', 'externalcode', 'tag']
  tags: {
    [key: string]: string;
  };
};
