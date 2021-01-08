import { WithdrawalStatus } from '../models/registrations.model';
export interface IChangeRegistrationWithdrawalStatusRequest {
  ids: string[];
  withdrawalStatus: WithdrawalStatus;
  comment?: string;
  isManual: boolean;
}
