import { BehaviorSubject, Observable, from, of } from 'rxjs';
import { GlobalSpinnerService, Utils } from '@opal20/infrastructure';
import {
  IQuerySearchTagRequest,
  ISaveResourceMetadataRequest,
  MetadataTagModel,
  QuerySearchTagResult,
  ResourceModel,
  ResourceType,
  SearchTag,
  TaggingApiService
} from '@opal20/domain-api';
import { filter, map, switchMap } from 'rxjs/operators';

import { Injectable } from '@angular/core';
import { ResourceMetadataFormModel } from '../models/resource-metadata-form.model';

@Injectable()
export class MetadataEditorService {
  public get metadataTags$(): Observable<MetadataTagModel[] | undefined> {
    return this._metadataTagsSubject.asObservable();
  }
  public get resource$(): Observable<ResourceModel> {
    return this._resourceSubject.asObservable();
  }
  public get resourceId$(): Observable<string | undefined> {
    return this._resourceIdSubject.asObservable();
  }
  public get resourceType$(): Observable<ResourceType | undefined> {
    return this._resourceTypeSubject.asObservable();
  }
  public get resourceMetadataForm$(): Observable<ResourceMetadataFormModel | undefined> {
    return this._resourceMetadataFormSubject.asObservable();
  }

  public get digitalContentAutoSaveInformer$(): Observable<boolean> {
    return this._digitalContentAutoSaveInformerSubject.asObservable();
  }

  public get currentResource(): ResourceModel | undefined {
    return this._resourceMetadataFormSubject.value.resource;
  }

  private _digitalContentAutoSaveInformerSubject: BehaviorSubject<boolean> = new BehaviorSubject(false);
  private _metadataTagsSubject: BehaviorSubject<MetadataTagModel[] | undefined> = new BehaviorSubject(undefined);
  private _resourceSubject: BehaviorSubject<ResourceModel> = new BehaviorSubject(new ResourceModel());
  private _resourceIdSubject: BehaviorSubject<string | undefined> = new BehaviorSubject(undefined);
  private _resourceTypeSubject: BehaviorSubject<ResourceType | undefined> = new BehaviorSubject(undefined);
  private _resourceMetadataFormSubject: BehaviorSubject<ResourceMetadataFormModel> = new BehaviorSubject(
    new ResourceMetadataFormModel(undefined, [])
  );

  constructor(private globalSpinnerService: GlobalSpinnerService, private taggingBackendSvc: TaggingApiService) {
    this.metadataTags$.subscribe(data => {
      this.initResourceMetadataForm();
    });
    this.resourceId$.subscribe(data => {
      this.initResourceMetadataForm(true);
    });
    this.resource$.subscribe(data => {
      this.initResourceMetadataForm();
    });
  }

  public setResourceInfo(resourceId: string | undefined, resourceType: ResourceType | undefined): void {
    this._resourceIdSubject.next(resourceId);
    this._resourceTypeSubject.next(resourceType);
  }

  public loadResource(forceLoad?: boolean): Observable<ResourceModel> {
    if (
      this._resourceSubject.value !== undefined &&
      this._resourceSubject.value.resourceId === this._resourceIdSubject.value &&
      !forceLoad
    ) {
      return of(this._resourceSubject.value);
    }
    if (this._resourceIdSubject.value === undefined) {
      const newResource = new ResourceModel();
      this._resourceSubject.next(newResource);
      this.initResourceMetadataForm(true);
      return of(newResource);
    } else {
      return this.taggingBackendSvc.getResource(this._resourceIdSubject.value).pipe(
        map(_ => {
          if (_) {
            this._resourceSubject.next(_);
            this.initResourceMetadataForm(true);
          } else {
            this._resourceSubject.next(
              new ResourceModel({
                resourceId: this._resourceIdSubject.value,
                resourceType: this._resourceTypeSubject.value,
                tags: [],
                searchTags: []
              })
            );
            this.initResourceMetadataForm(true);
          }
          return _;
        })
      );
    }
  }

  public resetDigitalContentAutoSaveInformerSubjet(): void {
    this._digitalContentAutoSaveInformerSubject.next(false);
  }

  public loadMetadataTags(forceLoad?: boolean): Observable<MetadataTagModel[]> {
    if (!forceLoad && this._metadataTagsSubject.value !== undefined) {
      return of(this._metadataTagsSubject.value);
    }
    return this.taggingBackendSvc.getAllMetaDataTags().pipe(
      map(_ => {
        this._metadataTagsSubject.next(_);
        return _;
      })
    );
  }

  public updateResourceMetadataForm(value: ResourceMetadataFormModel | undefined): void {
    if (!Utils.isDifferent(value, this._resourceMetadataFormSubject.value)) {
      return;
    }
    const clonedData = Utils.clone(value, _ => {
      _.resource = Utils.cloneDeep(_.resource);
    });
    this._resourceMetadataFormSubject.next(clonedData);
  }

  public getSearchTagByNames(searchTags: string[]): Observable<SearchTag[]> {
    return this.taggingBackendSvc.getSearchTagByNames(searchTags);
  }

  public querySearchTag(searchText: string, skipCount: number, maxResultCount: number): Observable<QuerySearchTagResult> {
    const request: IQuerySearchTagRequest = {
      searchText: searchText,
      pagedInfo: {
        skipCount: skipCount,
        maxResultCount: maxResultCount
      }
    };
    return this.taggingBackendSvc.querySearchTag(request, false);
  }

  public saveCurrentResourceMetadataForm(
    resourceId?: string,
    resourceType?: ResourceType,
    skipValidation?: boolean,
    showSpinner: boolean = true,
    isAutoSave: boolean = false
  ): Observable<ResourceModel> {
    const resourceMetadataForm = this._resourceMetadataFormSubject.value;

    if (resourceMetadataForm === undefined) {
      return of(undefined);
    }

    const resource = Utils.cloneDeep(resourceMetadataForm.resource);
    if (resourceId != null && resourceType != null) {
      resource.resourceId = resourceId;
      resource.resourceType = resourceType;
    }

    const validationPromise = skipValidation ? Promise.resolve(true) : this._vaidateEditorForm();

    return from(validationPromise).pipe(
      filter(_ => _ === true),
      switchMap(_ => {
        const request: ISaveResourceMetadataRequest = {
          tagIds: resource.tags,
          mainSubjectAreaTagId: resource.mainSubjectAreaTagId,
          objectivesOutCome: resource.objectivesOutCome,
          preRequisties: resource.preRequisties,
          searchTags: [...new Set(resource.searchTags)]
        };
        if (resource.resourceType === ResourceType.Content) {
          return this.taggingBackendSvc.saveContentMetadata(resource.resourceId, request, showSpinner).pipe(map(__ => resource));
        }
        if (resource.resourceType === ResourceType.Course) {
          return this.taggingBackendSvc.saveCourseMetadata(resource.resourceId, request, showSpinner).pipe(map(__ => resource));
        }
        throw new Error('Invalid resource type');
      }),
      map(_ => {
        if (isAutoSave) {
          this._digitalContentAutoSaveInformerSubject.next(true);
        }
        this._resourceSubject.next(resource);
        if (this._resourceIdSubject.value !== resource.resourceId) {
          this._resourceIdSubject.next(resource.resourceId);
        }
        if (this._resourceTypeSubject.value !== resource.resourceType) {
          this._resourceTypeSubject.next(resource.resourceType);
        }
        return resource;
      })
    );
  }

  public resetMetadataSubjects(): void {
    this._resourceMetadataFormSubject.next(undefined);
    this._metadataTagsSubject.next(undefined);
  }

  public setValidateFormFn(fn: () => Promise<boolean>): void {
    this._vaidateEditorForm = fn;
  }

  public resetValidateFormFn(): void {
    this._vaidateEditorForm = () => Promise.resolve(false);
  }

  private initResourceMetadataForm(forceReset?: boolean): void {
    if (forceReset !== true && this._resourceMetadataFormSubject.value !== undefined) {
      return;
    }
    if (this._metadataTagsSubject.value === undefined) {
      this._resourceMetadataFormSubject.next(undefined);
    } else {
      this._resourceMetadataFormSubject.next(
        new ResourceMetadataFormModel(Utils.cloneDeep(this._resourceSubject.value), this._metadataTagsSubject.value)
      );
    }
  }

  private _vaidateEditorForm: () => Promise<boolean> = () => Promise.resolve(false);
}
