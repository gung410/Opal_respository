import * as path from 'path';

import { IRSCTaskConfig, RSCTask } from '@microsoft/gulp-core-build-typescript/lib/RSCTask';
import { IThunderPackage, Repo } from '../Repo';

import { Git } from '../Git';
import { PackageJsonLookup } from '@microsoft/node-core-library';
import { Utilities } from '../Utilities';

/**
 * @public
 */
export interface IThunderPackageCache {
  repoRoot: string;
  gitRoot: string;
  packages: Map<string, IThunderPackage>;
}

/**
 * @public
 */
export enum ThunderGitMode {
  TrackedFiles,
  StagedFiles
}

/**
 * @public
 */
export abstract class ThunderTask<TConfig extends IRSCTaskConfig> extends RSCTask<TConfig> {
  private static _lookup: PackageJsonLookup;
  private static _packagePathCache: IThunderPackageCache;

  constructor(taskName: string, config: TConfig) {
    super(taskName, config);
  }

  protected static get packageJsonLookup(): PackageJsonLookup {
    if (!ThunderTask._lookup) {
      ThunderTask._lookup = new PackageJsonLookup();
    }

    return ThunderTask._lookup;
  }

  public get packagePathCache(): IThunderPackageCache {
    if (!ThunderTask._packagePathCache) {
      ThunderTask._packagePathCache = {
        gitRoot: Repo.gitRoot,
        repoRoot: Repo.repoRoot,
        packages: Repo.allPackages
      };
    }

    return ThunderTask._packagePathCache;
  }

  public abstract executeTask(): Promise<Object | void> | NodeJS.ReadWriteStream | void;

  protected getPackagesByGit(mode: ThunderGitMode): IThunderPackage[] {
    // Get staged files in monorepo scope by mode.
    let gitFiles: string[] = [];

    if (mode === ThunderGitMode.StagedFiles) {
      gitFiles.push(...Git.getStagedChanges());
    } else {
      gitFiles.push(...Git.getTrackedFiles());
    }

    gitFiles = gitFiles.map((file: string) => path.join(Repo.gitRoot, file)).filter((file: string) => file.startsWith(Repo.repoRoot));
    // Arranges staged files into thunder packages by extensions.
    const removedIndexes: number[] = [];
    const packages: IThunderPackage[] = Array.from(this.packagePathCache.packages.values()).map((pkg: IThunderPackage) => {
      if (pkg.packagePath !== Repo.repoRoot) {
        pkg.taskFiles = gitFiles.filter((file: string, index: number) => {
          const valid: boolean = file.startsWith(pkg.packagePath);
          if (valid) {
            removedIndexes.push(index);
          }
          return valid;
        });
      }
      return pkg;
    });
    // Arrage staged files that are belong to monorepo.
    const remainingFiles: string[] = gitFiles.filter((file: string, index: number) => !removedIndexes.some((i: number) => index === i));
    const foundPkg: IThunderPackage | undefined = packages.find((pkg: IThunderPackage) => pkg.packagePath === Repo.repoRoot);

    if (foundPkg) {
      foundPkg.taskFiles = remainingFiles;
    }

    return packages;
  }

  protected getPackagesByProject(project: string | boolean): IThunderPackage[] {
    const projects: string[] = [];

    if (project === true) {
      if (Repo.buildFolder && Repo.buildFolder !== Repo.repoRoot) {
        const packageJson: string = path.join(Repo.buildFolder, Repo.packageJsonFile);
        const packageName: string = ThunderTask.packageJsonLookup.loadPackageJson(packageJson).name;

        projects.push(packageName);
      }
    } else if (typeof project === 'string') {
      const parts: string[] = project.split(',').filter((x: string) => Utilities.isNonemptyString(x));

      projects.push(...parts);
    }

    const packages: IThunderPackage[] = Array.from(this.packagePathCache.packages.entries())
      .filter((pkg: [string, IThunderPackage]) => projects.some((proj: string) => proj === pkg[0]))
      .map((pkg: [string, IThunderPackage]) => pkg[1]);

    return packages;
  }
}
