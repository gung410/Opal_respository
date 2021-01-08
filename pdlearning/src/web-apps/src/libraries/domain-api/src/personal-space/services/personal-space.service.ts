import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';

import { ICreatePersonalFilesRequest } from '../dtos/create-personal-file-request';
import { IPersonalSpaceModel } from '../models/personal-space.model';
import { Injectable } from '@angular/core';
import { SearchPersonalFileRequest } from './../dtos/search-personal-file-request';
import { SearchPersonalFileResponse } from '../dtos/search-personal-file-request';
import { map } from 'rxjs/operators';

@Injectable()
export class PersonalSpaceApiService extends BaseBackendService {
  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  protected get apiUrl(): string {
    return AppGlobal.environment.uploaderApiUrl;
  }

  public createPersonalFiles(request: ICreatePersonalFilesRequest, showSpinner: boolean = true): Promise<void> {
    return this.post<ICreatePersonalFilesRequest, void>('/personal-files/create', request, showSpinner).toPromise();
  }

  public getPersonalSpaceInfo(showSpinner: boolean = true): Promise<IPersonalSpaceModel> {
    return this.get<IPersonalSpaceModel>('/personal-spaces', null, showSpinner).toPromise();
  }

  public deletePersonalFile(fileId: string, showSpinner: boolean = true): Promise<void> {
    return this.delete<void>(`/personal-files/${fileId}`, showSpinner).toPromise();
  }

  public searchPersonalFiles(request: SearchPersonalFileRequest, showSpinner: boolean = true): Promise<SearchPersonalFileResponse> {
    return this.post<SearchPersonalFileRequest, SearchPersonalFileResponse>('/personal-files/search', request, showSpinner)
      .pipe(map(data => new SearchPersonalFileResponse(data)))
      .toPromise();
  }
}
