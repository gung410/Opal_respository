import { SpawnSyncReturns, spawnSync } from 'child_process';

import { Utilities } from './Utilities';

/**
 * @public
 */
export interface IGitResult {
  stderr: string;
  stdout: string;
  success?: boolean;
}

/**
 * @public
 */
export interface IGitOption {
  cwd?: string;
}

/**
 * @public
 */
export class Git {
  /**
   * Runs git command - use this for read only commands
   * @public
   * @param args - Git command arguments
   * @param options - Git options
   * @returns `IGitReturn`
   */
  public static execute(args: string[], options?: IGitOption): IGitResult {
    const spawnResult: SpawnSyncReturns<Buffer> = spawnSync('git', args, options);

    return {
      stderr: spawnResult.stderr.toString().trimRight(),
      stdout: spawnResult.stdout.toString().trimRight(),
      success: spawnResult.status === 0
    };
  }

  /**
   * Runs git command - use to get git result as a string when success.
   * @public
   * @param args - Git command arguments
   * @param options - Git options
   * @returns Result string or undefined
   */
  public static executeFast(args: string[], options?: IGitOption): string | undefined {
    const gitResult: IGitResult = this.execute(args, options);

    if (gitResult.success) {
      return gitResult.stdout;
    }

    return undefined;
  }

  /**
   * Runs git command - use this for commands that makes changes to the file system
   * @public
   * @param args - Git command arguments
   * @param options - Git options
   */
  public static executeFailFast(args: string[], options?: IGitOption): void {
    const gitResult: IGitResult = Git.execute(args, options);

    if (!gitResult.success) {
      console.error(`CRITICAL ERROR: running git command: git ${args.join(' ')}!`);
      console.error(gitResult.stdout && gitResult.stdout.toString().trimRight());
      console.error(gitResult.stderr && gitResult.stderr.toString().trimRight());
      process.exit(1);
    }
  }

  /**
   * Run git command - returns files which have changes are staged
   * @public
   * @param excludeDeletedOrRenamedFiles - The deleted or renamed files will be excluded
   * @param cwd - Current working directory
   * @returns - Staged files
   */
  public static getStagedChanges(excludeDeletedOrRenamedFiles: boolean = true, cwd: string = process.cwd()): string[] {
    const args: string[] = ['--no-pager', 'diff', '--staged', '--name-only'];

    if (excludeDeletedOrRenamedFiles) {
      args.push('--diff-filter=dr');
    }

    const changes: string | undefined = Git.executeFast(args, { cwd });

    if (!changes) {
      return [];
    }

    const lines: string[] = changes.split(/\n/) || [];

    return lines.filter(line => Utilities.isNonemptyString(line.trim())).map(line => line.trim());
  }

  /**
   * Run git command - returns files which are tracked by git
   * @public
   * @param cwd - Current working directory
   * @returns - Tracked files
   */
  public static getTrackedFiles(cwd: string = process.cwd()): string[] {
    const result: string | undefined = Git.executeFast(['ls-tree', '-r', '--name-only', '--full-tree', 'HEAD'], { cwd });

    if (result) {
      return result.split(/\n/);
    }

    return [];
  }

  /**
   * Run git command - sets the git config include.path
   * @public
   * @param path - The local include.path
   * @returns - Tracked files
   */
  public static setLocalConfigPath(path: string): void {
    Git.executeFailFast(['config', '--local', 'include.path', path]);
  }
}
