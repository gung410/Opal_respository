import { PrefixSumComputer, PrefixSumIndexOfResult } from './PrefixSumComputer';

import { Position } from './Position';
import { Range } from './Range';
import { TextEdit } from './TextEdit';

/**
 * Represents a line of text, such as a line of source code.
 *
 * TextLine objects are __immutable__. When a [document](#TextDocument) changes,
 * previously retrieved lines will not represent the latest state.
 */
export interface ITextLine {
  /**
   * The zero-based line number.
   */
  readonly lineNumber: number;

  /**
   * The text of this line without the line separator characters.
   */
  readonly text: string;

  /**
   * The range this line covers without the line separator characters.
   */
  readonly range: Range;

  /**
   * The range this line covers with the line separator characters.
   */
  readonly rangeIncludingLineBreak: Range;

  /**
   * The offset of the first character which is not a whitespace character as defined
   * by `/\s/`. **Note** that if a line is all whitespace the length of the line is returned.
   */
  readonly firstNonWhitespaceCharacterIndex: number;

  /**
   * Whether this line is whitespace only, shorthand
   * for [TextLine.firstNonWhitespaceCharacterIndex](#TextLine.firstNonWhitespaceCharacterIndex) === [TextLine.text.length](#TextLine.text).
   */
  readonly isEmptyOrWhitespace: boolean;
}

export class TextDocument {
  private readonly _textLines: ITextLine[] = [];
  private readonly _lines: string[];
  private _lineStarts: PrefixSumComputer | null = null;
  private _eol: string = '\n';

  constructor(text: string) {
    this._lines = text.split(this._eol);
  }
  public applyEdits(edits: TextEdit[]): string {
    let text: string = this.getText();
    const sortedEdits: TextEdit[] = this.mergeSort(edits, (a: TextEdit, b: TextEdit) => {
      const diff: number = a.range.start.line - b.range.start.line;
      if (diff === 0) {
        return a.range.start.character - b.range.start.character;
      }
      return diff;
    });
    let lastModifiedOffset: number = text.length;
    for (let i: number = sortedEdits.length - 1; i >= 0; i--) {
      const e: TextEdit = sortedEdits[i];
      const startOffset: number = this.offsetAt(e.range.start);
      const endOffset: number = this.offsetAt(e.range.end);
      if (endOffset <= lastModifiedOffset) {
        text = text.substring(0, startOffset) + e.newText + text.substring(endOffset, text.length);
      } else {
        throw new Error('Overlapping edit');
      }
      lastModifiedOffset = startOffset;
    }
    return text;
  }

  public getText(range?: Range): string {
    if (range) {
      return this._getTextInRange(range);
    }

    return this._lines.join(this._eol);
  }

  public lineAt(lineOrPosition: number | Position): ITextLine {
    let line: number | undefined;
    if (lineOrPosition instanceof Position) {
      line = lineOrPosition.line;
    } else if (typeof lineOrPosition === 'number') {
      line = lineOrPosition;
    }

    if (typeof line !== 'number' || line < 0 || line >= this._lines.length) {
      throw new Error('Illegal value for `line`');
    }

    let result: ITextLine = this._textLines[line];
    if (!result || result.lineNumber !== line || result.text !== this._lines[line]) {
      const text: string = this._lines[line];
      const firstNonWhitespaceCharacterIndex: number = /^(\s*)/.exec(text)![1].length;
      const range: Range = new Range(line, 0, line, text.length);
      const rangeIncludingLineBreak: Range = line < this._lines.length - 1 ? new Range(line, 0, line + 1, 0) : range;

      result = Object.freeze({
        lineNumber: line,
        range,
        rangeIncludingLineBreak,
        text,
        firstNonWhitespaceCharacterIndex, // TODO@api, rename to 'leadingWhitespaceLength'
        isEmptyOrWhitespace: firstNonWhitespaceCharacterIndex === text.length
      });

      this._textLines[line] = result;
    }

    return result;
  }

  public positionAt(offset: number): Position {
    offset = Math.floor(offset);
    offset = Math.max(0, offset);

    this._ensureLineStarts();
    const out: PrefixSumIndexOfResult = this._lineStarts!.getIndexOf(offset);

    const lineLength: number = this._lines[out.index].length;

    // Ensure we return a valid position
    return new Position(out.index, Math.min(out.remainder, lineLength));
  }

  private offsetAt(position: Position): number {
    position = this._validatePosition(position);
    this._ensureLineStarts();
    return this._lineStarts!.getAccumulatedValue(position.line - 1) + position.character;
  }

  private _ensureLineStarts(): void {
    if (!this._lineStarts) {
      const eolLength: number = this._eol.length;
      const linesLength: number = this._lines.length;
      const lineStartValues: Uint32Array = new Uint32Array(linesLength);

      for (let i: number = 0; i < linesLength; i++) {
        lineStartValues[i] = this._lines[i].length + eolLength;
      }

      this._lineStarts = new PrefixSumComputer(lineStartValues);
    }
  }

  private _getTextInRange(range: Range): string {
    range = this._validateRange(range);

    if (range.isEmpty) {
      return '';
    }

    if (range.isSingleLine) {
      return this._lines[range.start.line].substring(range.start.character, range.end.character);
    }

    const lineEnding: string = this._eol,
      startLineIndex: number = range.start.line,
      endLineIndex: number = range.end.line,
      resultLines: string[] = [];

    resultLines.push(this._lines[startLineIndex].substring(range.start.character));
    for (let i: number = startLineIndex + 1; i < endLineIndex; i++) {
      resultLines.push(this._lines[i]);
    }
    resultLines.push(this._lines[endLineIndex].substring(0, range.end.character));

    return resultLines.join(lineEnding);
  }

  private _validateRange(range: Range): Range {
    if (!(range instanceof Range)) {
      throw new Error('Invalid argument');
    }

    const start: Position = this._validatePosition(range.start);
    const end: Position = this._validatePosition(range.end);

    if (start === range.start && end === range.end) {
      return range;
    }
    return new Range(start.line, start.character, end.line, end.character);
  }

  private _validatePosition(position: Position): Position {
    if (!(position instanceof Position)) {
      throw new Error('Invalid argument');
    }

    let { line, character } = position;
    let hasChanged: boolean = false;

    if (line < 0) {
      line = 0;
      character = 0;
      hasChanged = true;
    } else if (line >= this._lines.length) {
      line = this._lines.length - 1;
      character = this._lines[line].length;
      hasChanged = true;
    } else {
      const maxCharacter: number = this._lines[line].length;
      if (character < 0) {
        character = 0;
        hasChanged = true;
      } else if (character > maxCharacter) {
        character = maxCharacter;
        hasChanged = true;
      }
    }

    if (!hasChanged) {
      return position;
    }
    return new Position(line, character);
  }

  private mergeSort<T>(data: T[], compare: (a: T, b: T) => number): T[] {
    if (data.length <= 1) {
      // sorted
      return data;
    }
    // tslint:disable-next-line:no-bitwise
    const p: number = (data.length / 2) | 0;
    const left: T[] = data.slice(0, p);
    const right: T[] = data.slice(p);

    this.mergeSort(left, compare);
    this.mergeSort(right, compare);

    let leftIdx: number = 0;
    let rightIdx: number = 0;
    let i: number = 0;
    while (leftIdx < left.length && rightIdx < right.length) {
      const ret: number = compare(left[leftIdx], right[rightIdx]);
      if (ret <= 0) {
        // smaller_equal -> take left to preserve order
        data[i++] = left[leftIdx++];
      } else {
        // greater -> take right
        data[i++] = right[rightIdx++];
      }
    }
    while (leftIdx < left.length) {
      data[i++] = left[leftIdx++];
    }
    while (rightIdx < right.length) {
      data[i++] = right[rightIdx++];
    }
    return data;
  }
}
