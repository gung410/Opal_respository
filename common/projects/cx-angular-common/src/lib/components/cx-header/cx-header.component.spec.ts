import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CxHeaderComponent } from './cx-header.component';
import { CurrentUser } from './models/current-user.model';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

describe('CxReuseableHeaderComponent', () => {
  let component: CxHeaderComponent;
  let fixture: ComponentFixture<CxHeaderComponent>;
  let element: HTMLElement;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [CxHeaderComponent],
      imports: [NgbModule],
      schemas: [NO_ERRORS_SCHEMA],
    })
      .compileComponents().then(() => {
        fixture = TestBed.createComponent(CxHeaderComponent);
        component = fixture.componentInstance;
        element = fixture.debugElement.nativeElement;
      });
  }));

  beforeEach(() => {
    component.currentUser = new CurrentUser({
      avatarUrl: 'http://www.gravatar.com/avatar/af7f20e59e5f486304ea2b0fe08adad0.jpg?s=80&d=mm',
      departmentId: 14350,
      emails: 'phat.nguyen@orientsoftware.com',
      familyName: 'Nguyen',
      givenName: 'Phat',
      id: '76f2b76d-f34f-4364-8627-935a79359fa9',
    });
    component.logo = {
      imageUrl: 'assets/images/logo-vip-24.png',
      imageAlt: 'PD Planner',
      routeLink: '/components/button'
    };
    component.topHeader = {
      linkHref: 'http://www.gov.sg',
      linkAlt: 'Singapore Government',
      iconClass: 'icon-staff-selected',
      text: 'A Singapore Government Agency Website'
    };
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit true when user clicks sign out button', () => {
    spyOn(component.signOut, 'emit');
    component.onLogout();
    expect(component.signOut.emit).toHaveBeenCalled();
    expect(component.signOut.emit).toHaveBeenCalledWith(true);
  });

  it('should emit true when user click edit profile button', () => {
    spyOn(component.editProfile, 'emit');
    component.onUpdateProfile();
    expect(component.editProfile.emit).toHaveBeenCalled();
    expect(component.editProfile.emit).toHaveBeenCalledWith(true);
  });

  it('should emit true when user click setting button', () => {
    spyOn(component.clickSettings, 'emit');
    component.onClickedSettings();
    expect(component.clickSettings.emit).toHaveBeenCalled();
    expect(component.clickSettings.emit).toHaveBeenCalledWith(true);
  });

  it('should emit true when user click support button', () => {
    spyOn(component.clickSupport, 'emit');
    component.onClickedSupport();
    expect(component.clickSupport.emit).toHaveBeenCalled();
    expect(component.clickSupport.emit).toHaveBeenCalledWith(true);
  });

  it('should emit search term when user inputs search text in search box', () => {
    spyOn(component.searchOnSearchBox, 'emit');
    const searchTerm = '1234';
    component.onSearch(searchTerm);
    expect(component.searchOnSearchBox.emit).toHaveBeenCalled();
    expect(component.searchOnSearchBox.emit).toHaveBeenCalledWith(searchTerm);
  });
});
