export interface IDataWithCallback<T> {
  data: T;
  success: () => void;
  error?: () => void;
}
