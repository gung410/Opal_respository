export class AsyncResponse<T> {
  data: T;
  error: Error;
  constructor(res: T, err: Error) {
    this.data = res;
    this.error = err;
  }
}

export function toCxAsync<T>(promise: Promise<T>): Promise<AsyncResponse<T>> {
  return promise
    .then((data) => {
      return new AsyncResponse(data, null);
    })
    .catch((err: Error) => {
      return new AsyncResponse(undefined, err);
    });
}
