import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserAccountConfirmationDialogComponent } from './user-account-confirmation-dialog.component';

xdescribe('UserAccountConfirmationDialogComponent', () => {
  let component: UserAccountConfirmationDialogComponent;
  let fixture: ComponentFixture<UserAccountConfirmationDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [UserAccountConfirmationDialogComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserAccountConfirmationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
