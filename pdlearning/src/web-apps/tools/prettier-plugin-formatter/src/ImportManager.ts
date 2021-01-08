import {
  ExternalModuleImport,
  File,
  Import,
  MultiLineImportRule,
  NamedImport,
  NamespaceImport,
  StringImport,
  TypescriptCodeGenerator
} from 'typescript-parser';

import { Position } from './Position';
import { Range } from './Range';
import { ScriptKind } from 'typescript';
import { TextDocument } from './TextDocument';
import { TextEdit } from './TextEdit';
import { ThunderTypescriptParser } from './ThunderTypescriptParser';
import { specifierSort } from './utilities';

export class ImportManager {
  private readonly _parser: ThunderTypescriptParser;

  constructor() {
    this._parser = new ThunderTypescriptParser();
  }

  public removeUnsedImports(unsortedCode: string, extension: string): string {
    const typescriptFile: File = this._parser.parseSource(unsortedCode, this.getSourceKind(extension));
    const keepImports: Import[] = this.getKeepImports(typescriptFile);

    this.removeTrailingIndexes(keepImports);

    const document: TextDocument = new TextDocument(unsortedCode);
    const edits: TextEdit[] = this.calculateTextEdits(document, typescriptFile, keepImports);

    return document.applyEdits(edits);
  }

  private getKeepImports(document: File): Import[] {
    const keepImports: Import[] = [];

    for (const actImport of document.imports) {
      if (actImport instanceof NamespaceImport || actImport instanceof ExternalModuleImport) {
        if (document.nonLocalUsages.indexOf(actImport.alias) > -1) {
          keepImports.push(actImport);
        }
      } else if (actImport instanceof NamedImport) {
        actImport.specifiers = actImport.specifiers
          .filter(o => document.nonLocalUsages.indexOf(o.alias || o.specifier) > -1)
          .sort(specifierSort);
        const defaultSpec: string | undefined = actImport.defaultAlias;
        const libraryAlreadyImported: Import | undefined = keepImports.find(d => d.libraryName === actImport.libraryName);
        if (actImport.specifiers.length || (!!defaultSpec && [...document.nonLocalUsages, ...document.usages].indexOf(defaultSpec) >= 0)) {
          if (libraryAlreadyImported) {
            if (actImport.defaultAlias) {
              (<NamedImport>libraryAlreadyImported).defaultAlias = actImport.defaultAlias;
            }
            (<NamedImport>libraryAlreadyImported).specifiers = [
              ...(<NamedImport>libraryAlreadyImported).specifiers,
              ...actImport.specifiers
            ];
          } else {
            keepImports.push(actImport);
          }
        }
      } else if (actImport instanceof StringImport) {
        keepImports.push(actImport);
      }
    }

    return keepImports;
  }

  private removeTrailingIndexes(imports: Import[]): void {
    const violatedImports: Import[] = imports.filter((imp: Import) => imp.libraryName.endsWith('/index'));

    for (const imp of violatedImports) {
      imp.libraryName = imp.libraryName.replace(/\/index$/, '');
    }
  }

  private calculateTextEdits(document: TextDocument, typescriptFile: File, imports: Import[]): TextEdit[] {
    const edits: TextEdit[] = [];

    if (typescriptFile.imports.length > 0) {
      const rawImports: string = imports
        .map((group: Import) => this.generator.generate(group))
        .filter(Boolean)
        .join('\n');

      if (!!rawImports) {
        edits.push(TextEdit.insert(new Position(0, 0), `${rawImports}\n`));
      }

      edits.push(
        TextEdit.delete(
          this.importRange(document, typescriptFile.imports[0].start, typescriptFile.imports[typescriptFile.imports.length - 1].end)
        )
      );
    }

    return edits;
  }

  private getSourceKind(ext: string): ScriptKind {
    switch (ext) {
      case '.ts':
        return ScriptKind.TS;
      case '.tsx':
        return ScriptKind.TSX;
      case '.js':
        return ScriptKind.JS;
      case '.jsx':
        return ScriptKind.JSX;
    }

    return ScriptKind.Unknown;
  }

  /**
   * Function that calculates the range object for an import.
   * @public
   * @param TextDocument - document
   * @param number - start
   * @param number - end
   * @returns Range
   */
  private importRange(document: TextDocument, start?: number, end?: number): Range {
    return start !== undefined && end !== undefined
      ? new Range(
          document.lineAt(document.positionAt(start).line).rangeIncludingLineBreak.start,
          document.lineAt(document.positionAt(end).line).rangeIncludingLineBreak.end
        )
      : new Range(new Position(0, 0), new Position(0, 0));
  }

  private get generator(): TypescriptCodeGenerator {
    return new TypescriptCodeGenerator({
      eol: ';',
      tabSize: 2,
      insertSpaces: false,
      multiLineTrailingComma: false,
      multiLineWrapThreshold: 0,
      spaceBraces: false,
      stringQuoteStyle: "'",
      wrapMethod: MultiLineImportRule.strictlyOneImportPerLine
    });
  }
}
