export class Validation {
  control: string = '';
  isError: boolean = false;
  message: string = '';

  constructor(data?: Partial<Validation>) {
    if (!data) {
      return;
    }

    this.control = data.control;
    this.isError = data.isError;
    this.message = data.message;
  }
}
