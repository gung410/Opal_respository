import * as fs from 'fs';
import * as path from 'path';

import { IPackageJson, JsonFile, PackageJsonLookup } from '@microsoft/node-core-library';

import { Git } from './Git';
import { Utilities } from './Utilities';

/**
 * @public
 */
export interface IThunderPackage {
  packagePath: string;
  packageJson: IPackageJson;
  tslintConfigPath?: string;
  tsconfigPath?: string;
  prettierConfigPath?: string;
  prettierIgnorePath?: string;
  taskFiles: string[];
}

/**
 * @public
 */
export class Repo {
  public static readonly tslintFile: string = 'tslint.json';
  public static readonly prettierConfigFile: string = 'prettier.config.js';
  public static readonly prettierIgnoreFile: string = '.prettierignore';
  public static readonly tsconfigFile: string = 'tsconfig.json';
  public static readonly tsconfigAppFile: string = 'tsconfig.app.json';
  public static readonly tsconfigLibFile: string = 'tsconfig.lib.json';
  public static readonly packageJsonFile: string = 'package.json';

  private static _gitRoot: string | undefined;
  private static _repoRoot: string | undefined;
  private static _allPackages: Map<string, IThunderPackage> | undefined;

  /**
   * Starting from `cwd`, searches up the directory hierarchy for `name`
   * @param name - File or directory name
   * @param cwd - Current working directory
   * @returns - Absolute path found or undefined
   */
  public static searchUp(name: string, cwd: string = process.cwd()): string | undefined {
    const root: string = path.parse(cwd).root;
    let found: boolean = false;

    while (!found && cwd !== root) {
      if (fs.existsSync(path.join(cwd, name))) {
        found = true;
        break;
      }

      cwd = path.dirname(cwd);
    }

    if (found) {
      return cwd;
    }

    return undefined;
  }

  public static get gitRoot(): string {
    if (!Repo._gitRoot) {
      Repo._gitRoot = Repo.searchUp('.git');

      if (!Repo._gitRoot) {
        throw new Error('Can not find .git folder.');
      }
    }

    return Repo._gitRoot;
  }

  public static get repoRoot(): string {
    if (!Repo._repoRoot) {
      Repo._repoRoot = Repo.searchUp('.githooks');

      if (!Repo._repoRoot) {
        throw new Error('Can not find .githooks folder. This folder identifies the monorepo root.');
      }
    }

    return Repo._repoRoot;
  }

  public static get buildFolder(): string {
    return Repo.searchUp(Repo.packageJsonFile)!;
  }

  public static get prettierPackagePath(): string {
    const lookup: PackageJsonLookup = new PackageJsonLookup();
    const mainEntryPath: string = require.resolve('prettier');
    const packageJsonPath: string | undefined = lookup.tryGetPackageJsonFilePathFor(mainEntryPath);

    if (!packageJsonPath) {
      throw new Error('Can not find prettier library in node_modules, please install it.');
    }

    return path.dirname(packageJsonPath);
  }

  public static get prettierPackageJson(): IPackageJson {
    return JsonFile.load(path.join(Repo.prettierPackagePath, Repo.packageJsonFile));
  }

  public static get prettierConfigPath(): string {
    return path.join(Repo.repoRoot, Repo.prettierConfigFile);
  }

  public static get prettierIgnorePath(): string {
    return path.join(Repo.repoRoot, Repo.prettierIgnoreFile);
  }

  public static get allPackages(): Map<string, IThunderPackage> {
    if (!Repo._allPackages) {
      Repo._allPackages = new Map<string, IThunderPackage>();
      const trackedFiles: string[] = Git.getTrackedFiles(Repo.repoRoot);

      const packageJsonFiles: string[] = trackedFiles
        .map((line: string) => line.trim())
        .filter((line: string) => line.endsWith(Repo.packageJsonFile));

      for (const packageJsonFile of packageJsonFiles) {
        const packageJsonPath: string = path.join(Repo.gitRoot, packageJsonFile);

        if (!fs.existsSync(packageJsonPath)) {
          continue;
        }

        const packageJsonLookup: PackageJsonLookup = new PackageJsonLookup();
        const packageJson: IPackageJson = packageJsonLookup.loadPackageJson(packageJsonPath);
        const packagePath: string = path.dirname(packageJsonPath);
        const tslintConfigPath: string | undefined = Utilities.getFilePathIfExist(path.join(packagePath, Repo.tslintFile));
        const tsconfigPath: string | undefined =
          Utilities.getFilePathIfExist(path.join(packagePath, Repo.tsconfigAppFile)) ||
          Utilities.getFilePathIfExist(path.join(packagePath, Repo.tsconfigLibFile)) ||
          Utilities.getFilePathIfExist(path.join(packagePath, Repo.tsconfigFile));
        const prettierConfigPath: string | undefined = Utilities.getFilePathIfExist(path.join(packagePath, Repo.prettierConfigFile));
        const prettierIgnorePath: string | undefined = Utilities.getFilePathIfExist(path.join(packagePath, Repo.prettierIgnoreFile));

        Repo._allPackages.set(packageJson.name, {
          packagePath,
          packageJson,
          tslintConfigPath,
          tsconfigPath,
          prettierConfigPath,
          prettierIgnorePath,
          taskFiles: []
        });
      }
    }

    return Repo._allPackages;
  }
}
