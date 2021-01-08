export class StringUtil {
  static readonly ENDING_ES_CASES: string[] = ['s', 'x', 'z', 'ch', 'sh'];
  static capitalize(word: string): string {
    return word.charAt(0).toUpperCase() + word.slice(1);
  }

  static compareStringsCaseSensitive(
    strValue: string,
    comparedStrValue: string
  ): number {
    return strValue.toLowerCase().localeCompare(comparedStrValue.toLowerCase());
  }

  static numericGrammaticalize(itemLength: number, unit: string): string {
    if (isNaN(itemLength)) {
      throw new Error(
        `itemLength param format is required a number, but receive a/an ${typeof itemLength}`
      );
    }

    if (itemLength <= 1) {
      return `${itemLength} ${unit}`;
    }

    const isEndingES = this.ENDING_ES_CASES.some((char) => unit.endsWith(char));

    return isEndingES
      ? `${itemLength} ${unit.concat('es')}`
      : `${itemLength} ${unit.concat('s')}`;
  }
}
