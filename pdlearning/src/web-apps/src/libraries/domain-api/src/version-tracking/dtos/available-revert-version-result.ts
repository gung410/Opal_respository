import { IVersionTracking } from '../models/version-tracking';

export interface IAvaliableRevertVesionResult {
  items: IVersionTracking[];
  activeVersion: IVersionTracking;
}

export class AvaliableRevertVesionResult implements IAvaliableRevertVesionResult {
  public items: IVersionTracking[];
  public activeVersion: IVersionTracking;

  constructor(data: IAvaliableRevertVesionResult) {
    if (!data) {
      return;
    }

    this.items = data.items;
    this.activeVersion = data.activeVersion;
  }
}
