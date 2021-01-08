import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CxDialogTemplateComponent } from '@conexus/cx-angular-common';

import { UserPendingActionDialogComponent } from './user-pending-action-dialog.component';

describe('UserPendingActionDialogComponent', () => {
  let component: UserPendingActionDialogComponent<any>;
  let fixture: ComponentFixture<UserPendingActionDialogComponent<any>>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [
        UserPendingActionDialogComponent,
        CxDialogTemplateComponent
      ]
    }).compileComponents();
    TestBed.overrideTemplate(CxDialogTemplateComponent, '<div></div>');
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserPendingActionDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});
