import { TestBed } from '@angular/core/testing';

import { CxCommonService } from './cx-angular-common.service';

describe('CxCommonService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CxCommonService = TestBed.get(CxCommonService);
    expect(service).toBeTruthy();
  });
});
