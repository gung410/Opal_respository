import {
  Directive,
  EmbeddedViewRef,
  Input,
  TemplateRef,
  ViewContainerRef
} from '@angular/core';
import { AuthService } from 'app-auth/auth.service';
import { Utils } from 'app-utilities/utils';

@Directive({
  selector: '[hasPermission]'
})
export class PermissionDirective {
  private readonly _thenTemplateRef: TemplateRef<unknown> | null = null;

  private _thenViewRef: EmbeddedViewRef<unknown> | null = null;
  private _elseTemplateRef: TemplateRef<unknown> | null = null;
  private _elseViewRef: EmbeddedViewRef<unknown> | null = null;

  private _hasPermission = false;
  private _isConditionPassed = false;

  constructor(
    private readonly viewRef: ViewContainerRef,
    private readonly templateRef: TemplateRef<unknown>,
    private readonly authService: AuthService
  ) {
    this._thenTemplateRef = this.templateRef;
  }

  /**
   * Tuple input parameter
   *
   * @param ArrayString (1) Require, array of permission key.
   *
   * @param boolean (2) Optional, the extra condition to replace when using with [shorthand-form] of "ngIf" directive.
   *
   * @returns return the embedded view if all permission and condition is true
   *
   * @example
   * <div *hasPermission="[[content.create, content.update], ngIfCondition]"></div>
   *
   */
  @Input() set hasPermission(value: [string[], boolean?]) {
    if (Utils.isNullOrEmpty(value)) {
      this._hasPermission = true;
      this._isConditionPassed = true;
      this._updateView();

      return;
    }

    const [permissionKeys, condition] = value;
    const user = this.authService.userDataInfo();

    this._hasPermission = permissionKeys.every((value) =>
      user.hasPermission(value)
    );
    this._isConditionPassed = condition ? condition : true;

    this._updateView();
  }

  /**
   *  Input a templateRef
   *
   *  A template to show if the permission condition to false.
   *
   * @example
   * <div *hasPermission="condition; else elseBlock"></div>
   * <div #elseBlock></div>
   *
   */
  @Input() set hasPermissionElse(templateRef: TemplateRef<unknown>) {
    const isTemplateRefOrNull = !!(
      !templateRef || templateRef.createEmbeddedView
    );
    if (!isTemplateRefOrNull) {
      throw new Error(`Else block must be a TemplateRef.`);
    }

    this._elseTemplateRef = templateRef;
    this._elseViewRef = null;
    this._updateView();
  }

  private _updateView(): void {
    if (this._isConditionPassed && this._hasPermission) {
      if (!this._thenViewRef) {
        this.viewRef.clear();
        this._elseViewRef = null;
        if (this._thenTemplateRef) {
          this._thenViewRef = this.viewRef.createEmbeddedView(
            this._thenTemplateRef,
            {
              hasPermission: this._hasPermission,
              condition: this._isConditionPassed
            }
          );
          this._thenViewRef.markForCheck();
        }
      }
    } else {
      if (!this._elseViewRef) {
        this.viewRef.clear();
        this._thenViewRef = null;
        if (this._elseTemplateRef) {
          this._elseViewRef = this.viewRef.createEmbeddedView(
            this._elseTemplateRef,
            {
              hasPermission: this._hasPermission,
              condition: this._isConditionPassed
            }
          );
          this._elseViewRef.markForCheck();
        }
      }
    }
  }
}
