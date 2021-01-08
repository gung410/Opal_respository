export enum UserEntityStatusEnum {
  ManualUserAccount = 1,
  SynchronizedUserAccount = 2
}
// tslint:disable-next-line:variable-name
export const UserEntityStatusConst = [
  { key: UserEntityStatusEnum.ManualUserAccount, description: 'External user' },
  { key: UserEntityStatusEnum.SynchronizedUserAccount, description: 'MOE user' }
];
