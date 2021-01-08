export enum TabIndex {
  basicInfo,
  advanceInfo,
  professionalDevelopmentInfo,
  approvalInfo,
  auditHistory
}

/**
 * The tab index of the audit history using for editing pending user.
 * Since the editing pending user dialog hide two tabs "Professional Development" and "Approval Information"
 * so this tab index is different from the editing normal user.
 */
export const auditHistoryTabIndexOnPendingUser: number = 2;
