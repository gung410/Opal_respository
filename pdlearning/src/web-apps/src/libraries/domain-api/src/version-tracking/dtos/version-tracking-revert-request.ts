import { VersionTrackingObjectType } from '../models/version-tracking';

export interface IVersionTrackingRevertRequest {
  revertFromRecordId: string;
  versionTrackingId: string;
  currentActiveId: string;
  objectType: VersionTrackingObjectType;
}
