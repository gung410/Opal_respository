import { BlockoutDateStatus } from '../models/blockout-date.model';

export interface ISaveBlockoutDateRequest {
  data: SaveBlockoutDateDto;
}

export interface SaveBlockoutDateDto {
  id?: string;
  title: string;
  description: string;
  startDay: number;
  startMonth: number;
  endDay: number;
  endMonth: number;
  validYear: number;
  status?: BlockoutDateStatus;
  planningCycleId?: string;
  serviceSchemes: string[];
}
