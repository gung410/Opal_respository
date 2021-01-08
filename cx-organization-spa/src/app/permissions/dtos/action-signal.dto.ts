export class ActionSignal {
  colId: string;
  actionName: 'Cancel' | 'Delete' | 'Clone' | 'Save';
  isPerformAction: boolean;

  constructor(data?: Partial<ActionSignal>) {
    if (!data) {
      return;
    }

    this.actionName = data.actionName;
    this.isPerformAction = data.isPerformAction;
    this.colId = data.colId;
  }
}
