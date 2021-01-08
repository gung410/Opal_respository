// tslint:disable:no-any

export class ScheduledTask {
  constructor(
    public id: string,
    public intervalTimer: number,
    public action: Function,
    public isAsync: boolean
  ) {}
}
