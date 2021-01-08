export interface ISendOrderRefreshmentRequest {
  sendToEmails: string[];
  emailCC: string[];
  subject: string;
  base64Message: string;
}
