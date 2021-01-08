export class AsyncRespone<T> {
  data: T;
  error: Error;
  constructor(res: T, err: Error) {
    this.data = res;
    this.error = err;
  }
}

export function toCxAsync<T>(promise: Promise<T>): Promise<AsyncRespone<T>> {
  return promise
    .then((data) => {
      return new AsyncRespone(data, null);
    })
    .catch((err: Error) => {
      return new AsyncRespone(undefined, err);
    });
}
