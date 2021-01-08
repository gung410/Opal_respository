import { EndOfLine } from './EndOfLine';
import { Position } from './Position';
import { Range } from './Range';
// tslint:disable:no-any

/**
 * These classes are copied from https://github.com/microsoft/vscode/blob/master/src/vs/workbench/api/common/extHostTypes.ts
 * So please don't review or write unit test scripts for them.
 */
export class TextEdit {
  get range(): Range {
    return this._range;
  }

  set range(value: Range) {
    if (value && !Range.isRange(value)) {
      throw new Error('TextEdit > range error');
    }
    this._range = value;
  }

  get newText(): string {
    return this._newText || '';
  }

  set newText(value: string) {
    if (value && typeof value !== 'string') {
      throw new Error('TextEdit > newText error');
    }
    this._newText = value;
  }

  get newEol(): EndOfLine | undefined {
    return this._newEol;
  }

  set newEol(value: EndOfLine | undefined) {
    if (value && typeof value !== 'number') {
      throw new Error('TextEdit > newEol');
    }
    this._newEol = value;
  }

  protected _range: Range;
  protected _newText: string | null;
  protected _newEol?: EndOfLine;
  public static isTextEdit(thing: any): thing is TextEdit {
    if (thing instanceof TextEdit) {
      return true;
    }
    if (!thing) {
      return false;
    }
    return Range.isRange(<TextEdit>thing) && typeof (<TextEdit>thing).newText === 'string';
  }

  public static replace(range: Range, newText: string): TextEdit {
    return new TextEdit(range, newText);
  }

  public static insert(position: Position, newText: string): TextEdit {
    return TextEdit.replace(new Range(position, position), newText);
  }

  public static delete(range: Range): TextEdit {
    return TextEdit.replace(range, '');
  }

  public static setEndOfLine(eol: EndOfLine): TextEdit {
    const ret: TextEdit = new TextEdit(new Range(new Position(0, 0), new Position(0, 0)), '');
    ret.newEol = eol;
    return ret;
  }

  constructor(range: Range, newText: string | null) {
    this._range = range;
    this._newText = newText;
  }

  public toJSON(): any {
    return {
      range: this.range,
      newText: this.newText,
      newEol: this._newEol
    };
  }
}
