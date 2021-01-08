export interface IGetModulePermissionRequest {
  grantedTypes?: PermissionGrantedType;
  objectTypes?: PermissionObjectType;
  modules?: PermissionModuleType[];
  actions?: string;
  parentAccessRightIds?: string[];
  accessRightIds?: string[];
  includeChildren?: boolean;
  includeLocalizedData?: boolean;
}
