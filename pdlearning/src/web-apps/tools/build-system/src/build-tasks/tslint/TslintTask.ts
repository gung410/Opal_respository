import * as colors from 'colors';
import * as minimatch from 'minimatch';
import * as path from 'path';

import { IThunderPackage, Repo } from '../../Repo';
import { ThunderGitMode, ThunderTask } from '../ThunderTask';

import { IRSCTaskConfig } from '@microsoft/gulp-core-build-typescript/lib/RSCTask';
import { JsonFile } from '@microsoft/node-core-library';
import { TslintRunner } from './TslintRunner';

export interface ITslintTaskConfig extends IRSCTaskConfig {
  extensions?: string[];
  tslintIgnore?: string[];
}
export class TslintTask extends ThunderTask<ITslintTaskConfig> {
  private readonly _tslintRunner: TslintRunner;
  private readonly _tslintExtensions: string[] = ['ts', 'tsx'];

  constructor() {
    super('thunder-tslint', {
      allowBuiltinCompiler: true,
      buildDirectory: Repo.repoRoot
    });

    this._tslintRunner = new TslintRunner(
      {
        fileError: this.fileError.bind(this),
        fileWarning: this.fileWarning.bind(this),
        displayAsError: true
      },
      this.buildFolder,
      this._terminalProvider
    );
  }

  public loadSchema(): Object | undefined {
    return JsonFile.load(path.resolve(__dirname, '..', 'schemas', 'tslint-cmd.schema.json'));
  }

  public executeTask(): void | Promise<void | Object> | NodeJS.ReadWriteStream {
    const args: { [name: string]: string | boolean } = this.buildConfig.args;

    if (args.project) {
      return this.runTslintForProject(args.project);
    } else if (args.all) {
      return this.runTslintForGit(ThunderGitMode.TrackedFiles);
    }

    return this.runTslintForGit();
  }

  private runTslintForGit(mode: ThunderGitMode = ThunderGitMode.StagedFiles): Promise<void> {
    const packages: IThunderPackage[] = this.getPackagesByGit(mode).filter(
      (pkg: IThunderPackage) => pkg.packagePath !== Repo.repoRoot && pkg.taskFiles.length > 0
    );

    return this.runTslint(packages);
  }

  private runTslintForProject(project: string | boolean): Promise<void> {
    const packages: IThunderPackage[] = this.getPackagesByProject(project);

    return this.runTslint(packages, true);
  }

  private runTslint(packages: IThunderPackage[], runForProject: boolean = false): Promise<void> {
    const promises: Promise<void>[] = [];

    packages.forEach((pkg: IThunderPackage) => {
      const files: string[] = pkg.taskFiles.filter((file: string) =>
        (this.taskConfig.extensions || this._tslintExtensions).some((ext: string) => {
          const isValidExtension: boolean = `.${ext}` === path.extname(file);
          const isIgnored: boolean =
            !!this.taskConfig.tslintIgnore && this.taskConfig.tslintIgnore.some((ignorePattern: string) => minimatch(file, ignorePattern));

          return isValidExtension && !isIgnored;
        })
      );
      const promise: Promise<void> = Promise.resolve()
        .then(() => this.log(colors.bgCyan(`Project: ${pkg.packageJson.name}-${pkg.packageJson.version}`)))
        .then(() => {
          if (files.length === 0 && !runForProject) {
            return Promise.resolve();
          }

          return this._tslintRunner.invoke({
            files,
            configPath: pkg.tslintConfigPath!,
            tsconfigPath: pkg.tsconfigPath!,
            project: (runForProject && pkg.packagePath) || undefined
          });
        });

      promises.push(promise);
    });

    return Promise.all(promises).then(() => this.log('🙌 All done! 🙌'));
  }
}
