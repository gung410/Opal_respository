export enum DigitalContentQueryMode {
  AllByCurrentUser = 'AllByCurrentUser',
  PendingApproval = 'PendingApproval',
  Approved = 'Approved',
  Archived = 'Archived'
}

export const DIGITAL_CONTENT_QUERY_MODE_LABEL = new Map<DigitalContentQueryMode, string>([
  [DigitalContentQueryMode.AllByCurrentUser, 'Files'],
  [DigitalContentQueryMode.PendingApproval, 'Pending Approval'],
  [DigitalContentQueryMode.Archived, 'Archival']
]);
