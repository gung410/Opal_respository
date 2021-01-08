import * as path from 'path';

import { IResolvedConfig, getConfig } from 'import-sort-config';
import importSort, { ISortResult } from 'import-sort';

import { ImportManager } from './ImportManager';
import { Parser } from 'prettier';
import { parsers as javascriptParsers } from 'prettier/parser-babylon';
import { parsers as typescriptParsers } from 'prettier/parser-typescript';

/**
 * @public
 */
export interface IPrettierParser {
  [parserName: string]: { preprocess: (text: string) => string } & Parser;
}

/**
 * @public
 */
export class PrettierImportPlugin {
  get parsers(): IPrettierParser {
    return {
      typescript: {
        ...typescriptParsers.typescript,
        preprocess: (text: string) => {
          return this.organizeImports(text, '.ts');
        }
      },
      babel: {
        ...javascriptParsers.babel,
        preprocess: (text: string) => {
          return this.organizeImports(text, '.js');
        }
      }
    };
  }

  private organizeImports(unsortedCode: string, extension: string): string {
    const result: string = new ImportManager().removeUnsedImports(unsortedCode, extension);

    return this.sortImports(result, extension);
  }

  private sortImports(unsortedCode: string, extension: string): string {
    const config: IResolvedConfig | undefined = this.getConfig(extension, path.resolve(__dirname, '..', '..'));
    const { parser: parserConfig, style, config: rawConfig } = config!;
    const sortResult: ISortResult = importSort(unsortedCode, parserConfig!, style!, `dummy${extension}`, rawConfig.options);

    return sortResult.code;
  }

  private getConfig(extension: string, directory?: string): IResolvedConfig | undefined {
    const resolvedConfig: IResolvedConfig | undefined = getConfig(extension, directory);

    this.throwIf(!resolvedConfig, `No configuration found for file type ${extension}`);

    const rawParser: string | undefined = resolvedConfig!.config.parser;
    const rawStyle: string | undefined = resolvedConfig!.config.style;

    this.throwIf(!rawParser, `No parser defined for file type ${extension}`);
    this.throwIf(!rawStyle, `No style defined for file type ${extension}`);

    const { parser: parserConfig, style } = resolvedConfig!;

    this.throwIf(!parserConfig, `Parser "${rawParser}" could not be resolved`);
    this.throwIf(!style || style === rawStyle, `Style "${rawStyle}" could not be resolved`);

    return resolvedConfig;
  }

  private throwIf(condition: boolean, message: string): void {
    if (condition) {
      throw new Error(`PrettierImportsPlugin error:  ${message}`);
    }
  }
}

/**
 * @public
 */
export const parsers: IPrettierParser = new PrettierImportPlugin().parsers;
