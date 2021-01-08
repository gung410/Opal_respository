import { Directive, EmbeddedViewRef, Input, TemplateRef, ViewContainerRef } from '@angular/core';

import { UserInfoModel } from '@opal20/domain-api';

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

  constructor(private readonly viewRef: ViewContainerRef, private readonly templateRef: TemplateRef<unknown>) {
    this._thenTemplateRef = this.templateRef;
  }

  @Input() set hasPermission(value: [string, boolean?]) {
    const user = UserInfoModel.getMyUserInfo();

    if (typeof value === 'string') {
      this._hasPermission = user.hasPermission(value);
      this._isConditionPassed = true;
      this._updateView();
      return;
    }

    const [permissionKeys, condition = true] = value;
    this._hasPermission = user.hasPermission(permissionKeys);
    this._isConditionPassed = condition;
    this._updateView();
  }

  @Input() set hasPermissionElse(templateRef: TemplateRef<unknown>) {
    const isTemplateRefOrNull = !!(!templateRef || templateRef.createEmbeddedView);
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
          this._thenViewRef = this.viewRef.createEmbeddedView(this._thenTemplateRef, {
            hasPermission: this._hasPermission,
            condition: this._isConditionPassed
          });
          this._thenViewRef.markForCheck();
        }
      }
    } else {
      if (!this._elseViewRef) {
        this.viewRef.clear();
        this._thenViewRef = null;
        if (this._elseTemplateRef) {
          this._elseViewRef = this.viewRef.createEmbeddedView(this._elseTemplateRef, {
            hasPermission: this._hasPermission,
            condition: this._isConditionPassed
          });
          this._elseViewRef.markForCheck();
        }
      }
    }
  }
}
