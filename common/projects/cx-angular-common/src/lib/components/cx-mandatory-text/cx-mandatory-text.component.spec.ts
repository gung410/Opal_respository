import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CxMandatoryTextComponent } from './cx-mandatory-text.component';

describe('CxMandatoryTextComponent', () => {
  let component: CxMandatoryTextComponent;
  let fixture: ComponentFixture<CxMandatoryTextComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CxMandatoryTextComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CxMandatoryTextComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
