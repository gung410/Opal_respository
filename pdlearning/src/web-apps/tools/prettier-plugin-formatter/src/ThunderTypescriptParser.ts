import {
  ClassDeclaration,
  EnumDeclaration,
  ExportAssignment,
  ExportDeclaration,
  FunctionDeclaration,
  Identifier,
  ImportDeclaration,
  ImportEqualsDeclaration,
  InterfaceDeclaration,
  ModuleDeclaration,
  Node,
  ScriptKind,
  ScriptTarget,
  SourceFile,
  SyntaxKind,
  TypeAliasDeclaration,
  VariableStatement,
  createSourceFile
} from 'typescript';
import { File, Resource } from 'typescript-parser';
import { ParsedPath, parse } from 'path';

import { parseClass } from 'typescript-parser/node-parser/class-parser';
import { parseEnum } from 'typescript-parser/node-parser/enum-parser';
import { parseExport } from 'typescript-parser/node-parser/export-parser';
import { parseFunction } from 'typescript-parser/node-parser/function-parser';
import { parseIdentifier } from 'typescript-parser/node-parser/identifier-parser';
import { parseImport } from 'typescript-parser/node-parser/import-parser';
import { parseInterface } from 'typescript-parser/node-parser/interface-parser';
import { parseModule } from 'typescript-parser/node-parser/module-parser';
import { parseTypeAlias } from 'typescript-parser/node-parser/type-alias-parser';
import { parseVariable } from 'typescript-parser/node-parser/variable-parser';
import { readFileSync } from 'fs';
import { traverseAst } from 'typescript-parser/node-parser/traverse-ast';
// tslint:disable:no-any

/**
 * @public
 */
export interface IParserModule {
  moduleRoot: Node;
  moduleResource: Resource;
}

/**
 * Magic.happens('here');
 * This class is the parser of the whole extension. It uses the typescript compiler to parse a file or given
 * source code into the token stream and therefore into the AST of the source. Afterwards an array of
 * resources is generated and returned.
 *
 * @export
 * @class ThunderTypescriptParser
 */
export class ThunderTypescriptParser {
  /**
   * Parses the given source into an anonymous File resource.
   * Mainly used to parse source code of a document.
   *
   * @param {string} source
   * @param {ScriptKind} [scriptKind=ScriptKind.TS]
   * @returns {File}
   *
   * @memberof TsResourceParser
   */
  public parseSource(source: string, scriptKind: ScriptKind = ScriptKind.TS): File {
    return this.parseTypescript(createSourceFile('inline.tsx', source, ScriptTarget.ES2015, true, scriptKind), '/');
  }

  /**
   * Parses a single file into a parsed file.
   *
   * @param {string} filePath
   * @param {string} rootPath
   * @returns {File}
   *
   * @memberof TsResourceParser
   */
  public parseFile(filePath: string, rootPath: string): File {
    return this.parseFiles([filePath], rootPath)[0];
  }

  /**
   * Parses multiple files into parsed files.
   *
   * @param {string[]} filePathes
   * @param {string} rootPath
   * @returns {File[]}
   *
   * @memberof TsResourceParser
   */
  public parseFiles(filePathes: string[], rootPath: string): File[] {
    return filePathes
      .map(o => {
        let scriptKind: ScriptKind = ScriptKind.Unknown;
        const parsed: ParsedPath = parse(o);
        switch (parsed.ext.toLowerCase()) {
          case 'js':
            scriptKind = ScriptKind.JS;
            break;
          case 'jsx':
            scriptKind = ScriptKind.JSX;
            break;
          case 'ts':
            scriptKind = ScriptKind.TS;
            break;
          case 'tsx':
            scriptKind = ScriptKind.TSX;
            break;
        }
        return createSourceFile(o, readFileSync(o).toString(), ScriptTarget.ES2015, true, scriptKind);
      })
      .map(o => this.parseTypescript(o, rootPath));
  }

  /**
   * Parses the typescript source into the file instance. Calls .parse afterwards to
   * get the declarations and other information about the source.
   *
   * @private
   * @param {SourceFile} source
   * @param {string} rootPath
   * @returns {TsFile}
   *
   * @memberof TsResourceParser
   */
  private parseTypescript(source: SourceFile, rootPath: string): File {
    const file: File = new File(source.fileName, rootPath, source.getStart(), source.getEnd());
    const syntaxList: Node = source.getChildAt(0);

    this.parse(file, syntaxList);

    return file;
  }

  /**
   * Recursive function that runs through the AST of a source and parses the nodes.
   * Creates the class / function / etc declarations and instanciates a new module / namespace
   * resource if needed.
   *
   * @private
   * @param {Resource} resource
   * @param {Node} node
   *
   * @memberof TsResourceParser
   */
  private parse(resource: Resource, root: Node): void {
    const modules: IParserModule[] = [{ moduleRoot: root, moduleResource: resource }];

    for (let iter: IParserModule | undefined = modules.shift(); iter !== undefined; iter = modules.shift()) {
      const { moduleRoot, moduleResource } = iter;

      traverseAst(
        moduleRoot as any,
        (node: any) => {
          switch (node.kind) {
            case SyntaxKind.ImportDeclaration:
            case SyntaxKind.ImportEqualsDeclaration:
              parseImport(moduleResource, <ImportDeclaration | ImportEqualsDeclaration>node);
              break;
            case SyntaxKind.ExportDeclaration:
            case SyntaxKind.ExportAssignment:
              parseExport(moduleResource, <ExportAssignment | ExportDeclaration>node);
              break;
            case SyntaxKind.EnumDeclaration:
              parseEnum(moduleResource, <EnumDeclaration>node);
              break;
            case SyntaxKind.TypeAliasDeclaration:
              parseTypeAlias(moduleResource, <TypeAliasDeclaration>node);
              break;
            case SyntaxKind.FunctionDeclaration:
              parseFunction(moduleResource, <FunctionDeclaration>node);
              break;
            case SyntaxKind.VariableStatement:
              parseVariable(moduleResource, <VariableStatement>node);
              break;
            case SyntaxKind.InterfaceDeclaration:
              parseInterface(moduleResource, <InterfaceDeclaration>node);
              break;
            case SyntaxKind.ClassDeclaration:
              parseClass(moduleResource, <ClassDeclaration>node);
              break;
            case SyntaxKind.Identifier:
              parseIdentifier(moduleResource, <Identifier>node);
              break;
            case SyntaxKind.ModuleDeclaration:
              modules.push({
                moduleRoot: node,
                moduleResource: parseModule(moduleResource, <ModuleDeclaration>node)
              });
              break;
            default:
              break;
          }
        },
        (node: any) => {
          switch (node.kind) {
            case SyntaxKind.ClassDeclaration:
            case SyntaxKind.ModuleDeclaration:
            case SyntaxKind.FunctionDeclaration:
              return true;
            default:
              return false;
          }
        }
      );
    }
  }
}
