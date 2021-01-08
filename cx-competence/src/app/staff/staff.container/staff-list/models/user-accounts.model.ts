export enum UserEntityStatusEnum {
  ManualUserAccount = 1,
  SynchronizedUserAccount = 2,
}
export const UserEntityStatusConst = [
  {
    key: UserEntityStatusEnum.ManualUserAccount,
    description: 'Manually created user account',
  },
  {
    key: UserEntityStatusEnum.SynchronizedUserAccount,
    description: 'Synchronised HR user account',
  },
];
