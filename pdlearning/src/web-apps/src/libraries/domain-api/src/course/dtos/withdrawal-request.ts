import { WithdrawalStatus } from '../models/registrations.model';

export interface IWithdrawalRequest {
  ids: string[];
  withdrawalStatus: WithdrawalStatus;
  comment: string;
}
