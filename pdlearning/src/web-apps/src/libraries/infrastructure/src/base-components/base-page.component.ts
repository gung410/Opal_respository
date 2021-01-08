import { BaseComponent } from './base-component';
import { ModuleFacadeService } from '../services/module-facade.service';

export abstract class BasePageComponent extends BaseComponent {
  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  /**
   * This is a protected virtual method to support config permission prefixes,
   * which support user which have permission startWith one of allowPermissionPrefixes items
   * can check that does the user have permission to access this page
   */
  protected allowPermissionKeyPrefixes(): string[] {
    return [];
  }

  /**
   * Check the user has permission to do/access/view something for the current page, based on allowPermissionKeyPrefixes.
   */
  protected hasPermissionAction(permissionAction: string): boolean {
    if (this.allowPermissionKeyPrefixes().length === 0) {
      return this.hasPermission(permissionAction);
    }

    const matchedPermissionKey = this.allowPermissionKeyPrefixes()
      .map(prefix => prefix + permissionAction)
      .find(permissionKey => this.hasPermission(permissionKey));

    return matchedPermissionKey != null;
  }
}
