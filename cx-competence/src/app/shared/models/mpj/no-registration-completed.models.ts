export class GetNoRegistrationFinisedPayload {
  courseId: string;
  departmentId: number;
  forClassRunEndAfter: string;
  forClassRunEndBefore: string;
}

export class NoRegistrationFinishedDto {
  courseId: string;
  totalFinishedLearner: number;
}
