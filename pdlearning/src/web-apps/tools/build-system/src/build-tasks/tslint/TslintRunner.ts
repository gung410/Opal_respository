import * as path from 'path';

import { IRushStackCompilerBaseOptions, ToolPaths, WriteFileIssueFunction } from '@microsoft/rush-stack-compiler-3.5';
import { IThunderInvokationOptions, ThunderCompilerBase } from '../ThunderCompilerBase';

import { ITerminalProvider } from '@microsoft/node-core-library';

/**
 * @public
 */
export interface ITslintInvokationOptions extends IThunderInvokationOptions {
  project?: string;
  files: string[];
  configPath: string;
  tsconfigPath: string;
}

export interface ITslintTaskOptions extends IRushStackCompilerBaseOptions {
  displayAsError?: boolean;
}

export interface ITslintPosition {
  character: number;
  line: number;
  position: number;
}

export interface ITslintError {
  name: string;
  ruleName: string;
  ruleSeverity: string;
  failure: string;
  startPosition: ITslintPosition;
  endPosition: ITslintPosition;
}

/**
 * @public
 */
export class TslintRunner extends ThunderCompilerBase<ITslintInvokationOptions, ITslintTaskOptions> {
  constructor(options: ITslintTaskOptions, rootPath: string, terminalProvider: ITerminalProvider) {
    super(options, rootPath, terminalProvider, {
      packagePath: ToolPaths.tslintPackagePath,
      packageJson: ToolPaths.tslintPackageJson,
      packageBinPath: path.join('bin', 'tslint')
    });
  }

  public invoke(options: ITslintInvokationOptions): Promise<void> {
    const args: string[] = ['--format', 'json', '--config', options.configPath];

    if (options.project) {
      args.push('--project', options.project);
    } else {
      args.push(...options.files);
    }

    return this.cmdRunner.runCmd({
      args: args,
      onData: (buffer: Buffer) => {
        const data: string = buffer.toString().trim();
        const logFn: WriteFileIssueFunction = this._taskOptions.displayAsError
          ? this._taskOptions.fileError
          : this._taskOptions.fileWarning;

        // Tslint errors are logged to stdout
        try {
          const errors: ITslintError[] = JSON.parse(data);

          for (const error of errors) {
            const pathFromRoot: string = path.relative(this._standardBuildFolders.projectFolderPath, error.name);

            logFn(pathFromRoot, error.startPosition.line + 1, error.startPosition.character + 1, error.ruleName, error.failure);
          }
        } catch (e) {
          // If we fail to parse the JSON, it's likely TSLint encountered an error parsing the config file,
          // or it experienced an inner error. In this case, log the output as an error regardless of the
          // displayAsError value
          this._terminal.writeErrorLine(data);
        }
      },
      onClose: (code, hasErrors, resolve, reject) => {
        if (this._taskOptions.displayAsError && (code !== 0 || hasErrors)) {
          reject(new Error(`exited with code ${code}`));
        } else {
          resolve();
        }
      }
    });
  }
}
