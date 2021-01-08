/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { CxNavbarDocComponent } from './cx-navbar-doc.component';

describe('CxNavbarDocComponent', () => {
  let component: CxNavbarDocComponent;
  let fixture: ComponentFixture<CxNavbarDocComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CxNavbarDocComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CxNavbarDocComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
