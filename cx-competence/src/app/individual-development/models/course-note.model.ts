import { isNullOrUndefined } from 'util';
import { OpportunityDTO } from 'app-models/opportunity.model';

// tslint:disable:variable-name
export class CoursepadNoteDto extends OpportunityDTO {}

export class CoursepadInfoDto {
  Id: string;
  Name: string;
  CourseType: number;
  ThumbnailUrl: string;
  TrailerlUrl: string;
  CreatedUtc: Date;
  Description: string;
  DurationMinutes: string;
  ModifiedUtc: Date;
  constructor(data?: Partial<CoursepadInfoDto>) {
    if (!data) {
      return;
    }
    this.Id = data.Id;
    this.Name = data.Name !== undefined ? data.Name : undefined;
    this.CourseType = !isNullOrUndefined(data.CourseType) ? data.CourseType : 0;
    this.ThumbnailUrl =
      data.ThumbnailUrl !== undefined ? data.ThumbnailUrl : undefined;
    this.TrailerlUrl =
      data.TrailerlUrl !== undefined ? data.TrailerlUrl : undefined;
    this.CreatedUtc =
      data.CreatedUtc !== undefined ? data.CreatedUtc : undefined;
    this.Description =
      data.Description !== undefined ? data.Description : undefined;
    this.DurationMinutes =
      data.DurationMinutes !== undefined ? data.DurationMinutes : undefined;
    this.ModifiedUtc =
      data.ModifiedUtc !== undefined ? data.ModifiedUtc : undefined;
  }
}

export class CoursepadContentDto {
  Id: string;
  MimeType: string;
  Rank: number;
  Value: string;
  Width: number;
  Height: number;
  CreatedUtc: Date;
  ModifiedUtc: Date;
  constructor(data?: Partial<CoursepadContentDto>) {
    if (!data) {
      return;
    }
    this.Id = data.Id;
    this.MimeType = data.MimeType ? data.MimeType : undefined;
    this.Rank = !isNullOrUndefined(data.Rank) ? data.Rank : 0;
    this.Value = data.Value ? data.Value : undefined;
    this.Width = !isNullOrUndefined(data.Width) ? data.Width : 0;
    this.Height = !isNullOrUndefined(data.Height) ? data.Height : 0;
    this.CreatedUtc = data.CreatedUtc ? data.CreatedUtc : undefined;
    this.ModifiedUtc = data.ModifiedUtc ? data.ModifiedUtc : undefined;
  }
}

export class CoursepadNoteResponse extends CoursepadNoteDto {
  totalItems: number;
  pageIndex: number;
  pageSize: number;
  hasMoreData: boolean;
  items: OpportunityDTO[];
}
