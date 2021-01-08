export class ArrayUtilities {
  public static paginate(
    array: Array<any>,
    pageSize: number,
    pageNumber: number
  ) {
    return array.slice(pageNumber * pageSize, (pageNumber + 1) * pageSize);
  }
}
