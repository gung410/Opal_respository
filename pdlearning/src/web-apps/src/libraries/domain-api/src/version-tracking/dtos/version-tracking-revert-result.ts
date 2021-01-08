export interface IRevertVersionTrackingResult {
  isSuccess: boolean;
  undoVersionId?: string;
  newActiveId?: string;
}

export class RevertVersionTrackingResult implements IRevertVersionTrackingResult {
  public isSuccess: boolean;
  public undoVersionId: string;
  public newActiveId: string;

  constructor(data: IRevertVersionTrackingResult) {
    if (data == null) {
      return;
    }

    this.isSuccess = data.isSuccess;
    this.undoVersionId = data.undoVersionId;
    this.newActiveId = data.newActiveId;
  }
}
