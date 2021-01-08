import { WorkBook, read as xlsxRead, utils as xlsxUtils } from 'xlsx';

export class ExcelError {
  constructor(public sheetName?: string, public errorDescription?: string) {}
}

export class ExcelReader {
  /*
   * Parse sheet data to <T> Object model
   * T[key] map with column name in excel
   */
  public static ParseSheetDataToObject<T>(file: File, sheetName?: string): Promise<Array<T>> {
    return new Promise((resolve, reject) => {
      const reader: FileReader = new FileReader();
      let value: T[];

      reader.onload = ev => {
        const excelData: string = ((ev.target as unknown) as { result: string }).result as string;
        const workBook: WorkBook = xlsxRead(excelData, { type: 'binary' });
        const sheetToParse = workBook.SheetNames.find(name => name === sheetName);

        if (!sheetToParse) {
          reject(`The import file is not for ${sheetName}. Please use the correct file.`);
        }

        const val = xlsxUtils.sheet_to_json(workBook.Sheets[sheetToParse]);

        if (val.some(x => Object.keys(x).some(key => key.includes('__EMPTY')))) {
          reject(`The import file is not for ${sheetName}. Please use the correct file.`);
        }

        value = val as T[];
      };

      reader.onloadend = () => {
        resolve(value);
      };

      reader.onerror = () => {
        reject('Something error while reading excel file.');
      };

      if (file) {
        reader.readAsBinaryString(file);
      }
    });
  }
}

export class ExcelReaderException {
  public static invalidFormat(sheetName: string, columnName: string, index: number): ExcelError {
    return new ExcelError(sheetName, `Invalid ${columnName} format at row ${index}`);
  }

  public static entityNotFound(sheetName: string, columnName: string, index: number): ExcelError {
    return new ExcelError(sheetName, `The ${columnName} at row ${index} in ${sheetName} spreadsheet is not found`);
  }

  public static duplicateUniqueField(sheetName: string, columnName: string, index: number, duplicateValue: string | number): ExcelError {
    return new ExcelError(sheetName, `The ${columnName} '${duplicateValue}' at row ${index} is duplicated`);
  }

  public static dataNotFound(sheetName: string, columnName: string, index: number): ExcelError {
    return new ExcelError(sheetName, `Can't not find ${columnName} at row ${index}`);
  }

  public static reachedMaxValue(sheetName: string, columnName: string, index: number): ExcelError {
    return new ExcelError(sheetName, `Reached max value of ${columnName} at row ${index}`);
  }
}
