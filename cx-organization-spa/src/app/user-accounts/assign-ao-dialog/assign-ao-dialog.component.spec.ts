import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CxDialogTemplateComponent } from '@conexus/cx-angular-common';
import { ToastrModule } from 'ngx-toastr';

import { AssignAODialogComponent } from './assign-ao-dialog.component';

describe('AssignAoDialogComponent', () => {
  let component: AssignAODialogComponent;
  let fixture: ComponentFixture<AssignAODialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [ToastrModule.forRoot()],
      declarations: [AssignAODialogComponent, CxDialogTemplateComponent]
    }).compileComponents();
  }));

  beforeEach(() => {
    TestBed.overrideTemplate(CxDialogTemplateComponent, '<div></div>');
    fixture = TestBed.createComponent(AssignAODialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});
