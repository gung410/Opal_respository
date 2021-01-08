import * as colors from 'colors';
import * as fs from 'fs';
import * as path from 'path';

import { IThunderPackage, Repo } from '../../Repo';
import { ThunderGitMode, ThunderTask } from '../ThunderTask';

import { IRSCTaskConfig } from '@microsoft/gulp-core-build-typescript/lib/RSCTask';
import { JsonFile } from '@microsoft/node-core-library';
import { PrettierRunner } from './PrettierRunner';

export interface IPretterTaskConfig extends IRSCTaskConfig {
  extensions?: string[];
}

/**
 * @public
 */
export class PrettierTask extends ThunderTask<IPretterTaskConfig> {
  private readonly _prettierExtensions: string[] = ['ts', 'tsx', 'js', 'jsx', 'json', 'scss', 'md'];
  private readonly _prettierRunner: PrettierRunner;
  private readonly _jsBeautyOptions: { [key: string]: string | number | boolean } = {
    indent_size: 2,
    eol: '\r\n',
    wrap_attributes: 'force-aligned',
    end_with_newline: false,
    wrap_line_length: 140
  };
  private readonly _attrSortOptions: string[] = [
    '\\#\\w+',
    '\\*ngIf',
    '\\*\\w+',
    'class',
    'id',
    'name',
    'data-.+',
    'src',
    'for',
    'type',
    'href',
    'values',
    'title',
    'alt',
    'role',
    'aria-.+',
    '\\[ngClass]',
    '\\[class.[\\w\\-]+\\]',
    '\\[ngStyle]',
    '\\[style.[\\w\\-]+\\]',
    '\\[\\(\\w+\\)\\]',
    '\\[\\w+\\]',
    '\\(\\w+\\)',
    'let-.+',
    '$unknown$'
  ];

  constructor() {
    super('thunder-prettier', {
      allowBuiltinCompiler: true,
      buildDirectory: Repo.repoRoot
    });

    this._prettierRunner = new PrettierRunner(
      {
        fileError: this.fileError.bind(this),
        fileWarning: this.fileWarning.bind(this)
      },
      this.buildFolder,
      this._terminalProvider
    );
  }

  public loadSchema(): Object | undefined {
    return JsonFile.load(path.resolve(__dirname, '..', 'schemas', 'prettier-cmd.schema.json'));
  }

  public executeTask(): void | Promise<void | Object> | NodeJS.ReadWriteStream {
    const args: { [name: string]: string | boolean } = this.buildConfig.args;

    if (args.project) {
      return this.runPrettierForProject(args.project);
    } else if (args.all) {
      return this.runPrettierForGit(ThunderGitMode.TrackedFiles);
    }

    return this.runPrettierForGit();
  }

  private runPrettierForGit(mode: ThunderGitMode = ThunderGitMode.StagedFiles): Promise<void> {
    const packages: IThunderPackage[] = this.getPackagesByGit(mode).filter((pkg: IThunderPackage) => pkg.taskFiles.length > 0);

    return this.runPrettier(packages);
  }

  private runPrettierForProject(project: string | boolean): Promise<void> {
    const packages: IThunderPackage[] = this.getPackagesByProject(project);

    return this.runPrettier(packages, true);
  }

  private runPrettier(packages: IThunderPackage[], runForProject: boolean = false): Promise<void> {
    const promises: Promise<void>[] = [];

    packages.forEach((pkg: IThunderPackage) => {
      const files: string[] = [];
      const htmlFiles: string[] = [];

      pkg.taskFiles.forEach((file: string) => {
        if ((this.taskConfig.extensions || this._prettierExtensions).some((ext: string) => `.${ext}` === path.extname(file))) {
          files.push(file);
        }

        if (path.extname(file) === '.html') {
          htmlFiles.push(file);
        }
      });
      const promise: Promise<void> = Promise.resolve()
        .then(() => this.log(colors.bgCyan(`Project: ${pkg.packageJson.name}-${pkg.packageJson.version}`)))
        .then(() => {
          const prettierFiles: string[] = runForProject
            ? [path.join(pkg.packagePath, '**', `*.{${(this.taskConfig.extensions || this._prettierExtensions).join(',')}}`)]
            : files;

          if (prettierFiles.length === 0) {
            return Promise.resolve();
          }

          this.runHtmlFormatter(htmlFiles);

          return this._prettierRunner.invoke({
            files: (!runForProject && files) || [],
            configPath: pkg.prettierConfigPath,
            ignorePath: pkg.prettierIgnorePath,
            logErrorsOnly: true
          });
        });

      promises.push(promise);
    });

    return Promise.all(promises).then(() => this.log('🙌 All done! 🙌'));
  }

  private runHtmlFormatter(files: string[] = []): Promise<void> {
    const promises: Promise<void>[] = [];

    files.forEach(file => {
      const promise: Promise<void> = Promise.resolve().then(() => {
        const text: string = fs.readFileSync(file, 'utf8');

        let result: string = require('posthtml')()
          .use(require('posthtml-attrs-sorter')(this._attrSortOptions))
          .process(text, { sync: true }).html;

        result = require('js-beautify').html(result, this._jsBeautyOptions);

        fs.writeFileSync(file, result + '\r\n', 'utf8');
      });

      promises.push(promise);
    });

    return Promise.all(promises).then(() => {
      /** Ignore */
    });
  }
}
