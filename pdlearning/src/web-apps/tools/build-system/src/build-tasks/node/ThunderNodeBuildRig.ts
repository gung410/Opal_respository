import * as buildCommon from '@microsoft/sp-build-common';
import * as buildNode from '@microsoft/sp-build-node';

import { Argv } from 'yargs';
import { PrettierTask } from '../prettier/PrettierTask';
import { ThunderNodeBuildRigTask } from './ThunderNodeBuildRigTask';
import { TslintTask } from '../tslint/TslintTask';

/**
 * @public
 */
export class ThunderNodeBuildRig extends buildNode.SPNodeBuildRig {
  protected getYargs(): Argv {
    return super
      .getYargs()
      .command(ThunderNodeBuildRigTask.Prettier, 'formats source code using prettier cli')
      .command(ThunderNodeBuildRigTask.Tslint, 'runs tslint cli on git staged files and specific projects')
      .command(ThunderNodeBuildRigTask.Bump, 'bumps version and updates change note')
      .command('default', 'equivalent to build and test');
  }

  protected getTasks(): Map<string, buildCommon.ITaskDefinition> {
    const tasks: Map<string, buildCommon.ITaskDefinition> = super.getTasks();

    const getRepoYargs: (yargs: Argv, cmdName: string) => Argv = (yargs: Argv, cmdName: string): Argv => {
      return yargs
        .option('all', {
          description: `runs ${cmdName} for all source files`
        })
        .option('git', {
          description: `runs ${cmdName} for git staged files`
        })
        .option('project', {
          description: `runs ${cmdName} for specific project(s)`
        });
    };
    tasks.set(ThunderNodeBuildRigTask.Prettier, {
      executable: new PrettierTask(),
      arguments: (prettierYargs: Argv) => getRepoYargs(prettierYargs, 'prettier')
    });
    tasks.set(ThunderNodeBuildRigTask.Tslint, {
      executable: new TslintTask(),
      arguments: (tslintYargs: Argv) => getRepoYargs(tslintYargs, 'tslint')
    });

    return tasks;
  }
}
