export class FormOptions {
  public manualTrackingChange?: boolean = false;

  /**
   * Indicate the indicator will be automatically shown when the asynchronous validator is run.
   */
  public autoAsyncIndicator?: boolean = true;

  /**
   * Gets or sets ordered list of possible errors that defines the
   * priority with which errors are handling.
   * Error listed at the beginning has higher priority.
   *
   * When there are multiple errors, only the error with the highest
   * priority will be handled.
   *
   * @example validationErrorPriority = [
   * 'required',
   * 'maxLength',
   * 'pattern',
   * 'relation',
   * 'exist'
   * ]
   */
  public validationErrorPriority?: string[] = ['required'];
}
