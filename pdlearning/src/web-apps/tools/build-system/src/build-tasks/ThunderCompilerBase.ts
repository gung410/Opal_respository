import { CmdRunner, IBaseTaskOptions } from '@microsoft/rush-stack-compiler-3.5/lib/shared/CmdRunner';
import { IRushStackCompilerBaseOptions, RushStackCompilerBase } from '@microsoft/rush-stack-compiler-3.5';

import { ITerminalProvider } from '@microsoft/node-core-library';

export interface IThunderInvokationOptions {}

export abstract class ThunderCompilerBase<
  TInvokationOptions extends IThunderInvokationOptions,
  TTaskOptions extends IRushStackCompilerBaseOptions
> extends RushStackCompilerBase<TTaskOptions> {
  protected cmdRunner: CmdRunner;

  constructor(taskOptions: TTaskOptions, rootPath: string, terminalProvider: ITerminalProvider, options: IBaseTaskOptions) {
    super(taskOptions, rootPath, terminalProvider);

    this.cmdRunner = new CmdRunner(this._standardBuildFolders, this._terminal, options);
  }

  public abstract invoke(options?: TInvokationOptions): Promise<void>;
}
