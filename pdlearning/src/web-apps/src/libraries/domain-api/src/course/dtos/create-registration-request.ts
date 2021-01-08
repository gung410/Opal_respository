export interface ICreateRegistrationRequest {
  approvingOfficer: string;
  alternativeApprovingOfficer?: string;
  registrationType?: string;
  registrations: Registrations[];
}

class Registrations {
  public courseId: string;
  public classRunId: string;
  public classRunChangeId?: string;
}
