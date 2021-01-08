export enum ClassRunDetailMode {
  NewClassRun = 'new-classrun',
  Edit = 'edit',
  View = 'view',
  ForApprover = 'for-approver'
}

export function showClassRunDetailViewOnly(mode: ClassRunDetailMode): boolean {
  return mode === ClassRunDetailMode.View || mode === ClassRunDetailMode.ForApprover;
}
