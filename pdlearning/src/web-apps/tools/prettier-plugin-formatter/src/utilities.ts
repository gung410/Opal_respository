import { SymbolSpecifier } from 'typescript-parser';

/**
 * Order specifiers by name.
 *
 * @export
 * @param {SymbolSpecifier} i1
 * @param {SymbolSpecifier} i2
 * @returns {number}
 */
export function specifierSort(i1: SymbolSpecifier, i2: SymbolSpecifier): number {
  return stringSort(i1.specifier, i2.specifier);
}

/**
 * String-Sort function.
 *
 * @export
 * @param {string} strA
 * @param {string} strB
 * @param {('asc' | 'desc')} [order='asc']
 * @returns {number}
 */
export function stringSort(strA: string, strB: string, order: 'asc' | 'desc' = 'asc'): number {
  let result: number = 0;
  if (strA < strB) {
    result = -1;
  } else if (strA > strB) {
    result = 1;
  }
  if (order === 'desc') {
    result *= -1;
  }
  return result;
}
