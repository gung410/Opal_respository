export class ButtonAction<T> {
  public id: string;
  public text: string;
  public icon?: string;
  public hiddenFn?: () => boolean;
  public conditionFn?: (dataItem: T) => boolean;
  public actionFn?: (dataItems: T[]) => Promise<unknown>;
}
