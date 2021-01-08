import { CommonModule } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {
  CUSTOM_ELEMENTS_SCHEMA,
  Pipe,
  PipeTransform,
  NO_ERRORS_SCHEMA
} from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { MatTabsModule } from '@angular/material';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { NgSelectModule } from '@ng-select/ng-select';
import {
  MissingTranslationHandler,
  TranslateCompiler,
  TranslateLoader,
  TranslateParser,
  TranslateService,
  TranslateStore,
  USE_DEFAULT_LANG,
  USE_STORE
} from '@ngx-translate/core';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { UserRoleEnum } from 'app/shared/constants/user-roles.enum';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';

import { UserManagement } from '../models/user-management.model';
import { UserAccountsDataService } from '../user-accounts-data.service';
import { EditUserDialogComponent } from './edit-user-dialog.component';

@Pipe({ name: 'translate' })
class MockTranslatePipe implements PipeTransform {
  transform(value: number): number {
    return value;
  }
}

// tslint:disable-next-line:max-classes-per-file
export class TranslateServiceStub {
  get(key: any): any {
    return Observable.of(key);
  }

  getCurrentLanguage(): string {
    return 'Norsk';
  }

  getValueBasedOnKey(key: string | string[]): Observable<any> {
    return Observable.create();
  }

  getValueImmediately(key: string): string {
    return '';
  }
}

describe('EditUserDialogComponent', () => {
  let fixture: ComponentFixture<EditUserDialogComponent>;
  let component: EditUserDialogComponent;
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      providers: [
        UserAccountsDataService,
        { provide: ToastrService, useValue: { error: () => {} } },
        [{ provide: USE_DEFAULT_LANG, useValue: {} }],
        [{ provide: USE_STORE, useValue: {} }],
        HttpHelpers,
        MissingTranslationHandler,
        TranslateParser,
        TranslateCompiler,
        TranslateLoader,
        TranslateStore,
        TranslateService,
        [
          {
            provide: TranslateAdapterService,
            useValue: new TranslateServiceStub()
          }
        ]
      ],
      imports: [
        HttpClientTestingModule,
        CxCommonModule,
        MatTabsModule,
        CommonModule,
        FormsModule,
        NgSelectModule,
        NoopAnimationsModule
      ],
      schemas: [NO_ERRORS_SCHEMA],
      declarations: [EditUserDialogComponent, MockTranslatePipe]
    }).compileComponents();
  }));

  beforeEach(() => {
    TestBed.overrideTemplate(EditUserDialogComponent, '');
    fixture = TestBed.createComponent(EditUserDialogComponent);
    component = fixture.componentInstance;
    component.fullUserInfoJsonData = {};
    component.user = {
      identity: { id: 0 },
      systemRoles: [{ identity: { extId: UserRoleEnum.ReportingOfficer } }]
    } as UserManagement;
    component.surveyjsOptions = {
      showModalHeader: true,
      modalHeaderText: '',
      cancelName: '',
      submitName: '',
      variables: [{ name: '', value: '' }]
    };
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
