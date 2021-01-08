import { Identity } from 'app-models/common.model';
import { IdpDto } from 'app/organisational-development/models/idp.model';

export interface ILearningPlanGridRowModel {
  name: {
    display: unknown;
    route: string;
    params?: unknown;
  };
  period: string;
  learningDirections: number;
  status: string;
  completionRate?: string;
  plan: IdpDto;
  identity?: Identity;
  detail?: {
    display: unknown;
    route: string;
    params?: unknown;
  };
}
