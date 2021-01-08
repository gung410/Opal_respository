import { NO_ERRORS_SCHEMA } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserFilterTagGroupComponent } from './user-filter-tag-group.component';

describe('UserFilterTagGroupComponent', () => {
  let component: UserFilterTagGroupComponent;
  let fixture: ComponentFixture<UserFilterTagGroupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [UserFilterTagGroupComponent],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserFilterTagGroupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
