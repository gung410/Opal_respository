import { IThunderInvokationOptions, ThunderCompilerBase } from '../ThunderCompilerBase';

import { IRushStackCompilerBaseOptions } from '@microsoft/rush-stack-compiler-3.5';
import { ITerminalProvider } from '@microsoft/node-core-library';
import { Repo } from '../../Repo';

/**
 * @public
 */
export interface IPrettierInvokationOptions extends IThunderInvokationOptions {
  files: string[];
  configPath?: string;
  ignorePath?: string;
  logErrorsOnly?: boolean;
}

/**
 * @public
 */
export class PrettierRunner extends ThunderCompilerBase<IPrettierInvokationOptions, IRushStackCompilerBaseOptions> {
  constructor(taskOptions: IRushStackCompilerBaseOptions, rootPath: string, terminalProvider: ITerminalProvider) {
    super(taskOptions, rootPath, terminalProvider, {
      packagePath: Repo.prettierPackagePath,
      packageJson: Repo.prettierPackageJson,
      packageBinPath: 'bin-prettier.js'
    });
  }

  public invoke(options: IPrettierInvokationOptions): Promise<void> {
    return this.cmdRunner.runCmd({
      args: [
        '--config',
        options.configPath || Repo.prettierConfigPath,
        '--ignore-path',
        options.ignorePath || Repo.prettierIgnorePath,
        ...(options.logErrorsOnly ? ['--loglevel', 'warn'] : []),
        '--write',
        ...options.files
      ]
    });
  }
}
