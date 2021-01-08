export function any<T>(
  collection: ArrayLike<T> | undefined,
  predicate: (item: T) => boolean
): boolean {
  if (collection === undefined) {
    return false;
  }
  for (const row of this.collection) {
    const element = row;
    if (predicate(element)) {
      return true;
    }
  }
  return false;
}
