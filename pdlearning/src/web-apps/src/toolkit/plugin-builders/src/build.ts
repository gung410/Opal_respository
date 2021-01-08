import * as chalk from 'chalk';
import * as colors from 'colors';
import * as fs from 'fs';
import * as fse from 'fs-extra';
import * as glob from 'glob';
import * as path from 'path';

import { argv } from 'yargs';
import { createHash } from 'crypto';
import { execSync } from 'child_process';

interface IModule {
  id: string;
  name: string;
  shortName: string;
  type: string;
  path: string;
  dependencies: string[];
  description?: string;
  module?: string;
  physicalPath?: string;
  development?: boolean;
  hash?: string;
}

interface IManifestJson {
  prefix: string;
  manifestPath: string;
  modulesPath: string;
  modulesUrl: string;
  moduleRegistration: { [key: string]: string };
  coreDependencies: { [key: string]: boolean };
  modules: { [key: string]: IModule };
}

const manifest: IManifestJson = require(path.join(__dirname, '..', '..', '..', 'angular.manifest.json'));

class Builder {
  private prefix: string = manifest.prefix;
  private cwd: string = path.join(__dirname, '..', '..', '..');
  private args: string[] = argv._;

  public run(): void {
    const tasks: Map<string, () => void> = this.getTasks();
    const taskName: string = this.args.shift()!;
    const task: (() => void) | undefined = tasks.get(taskName);

    if (!task) {
      console.error(chalk.default.red(`Cannot find task name: ${taskName}`));
    } else {
      task();
    }
  }

  private getTasks(): Map<string, () => void> {
    const tasks: Map<string, () => void> = new Map();

    tasks.set('builders', () => this.buildBuilders());
    tasks.set('build', () => this.build());
    tasks.set('reg', () => this.registerModules());
    tasks.set('copy-lang', () => this.copyLang());

    return tasks;
  }

  private buildBuilders(): void {
    const msg: string = `Building builders toolkit...`;
    const cmd: string = 'yarn build';

    this.exec(msg, cmd, path.join(__dirname, '..'));
  }

  private registerModules(): void {
    const contents: string[] = [];

    console.log(`${chalk.default.green('Registering modules...')}`);

    for (const id of Object.keys(manifest.modules)) {
      const module: IModule = manifest.modules[id];

      if (module.type !== 'module') {
        continue;
      }

      const modulesPath: string = path.resolve(path.join(this.cwd, 'modules', module.path));
      const moduleFile: string = module.module!.split('#')[0];
      const templatePath: string = path.resolve(path.join(this.cwd, manifest.moduleRegistration.path));
      const modulePath: string = path.relative(templatePath, modulesPath);

      contents.push(`{
    id: '${id}',
    name: '${module.name}',
    shortName: '${module.shortName}',
    description: '${module.description || ''}',
    development: ${module.development || false},
    loadNgModule: () => import(/*webpackChunkName:"${this.prefix}.${id}"*/ '${path.join(modulePath, moduleFile).replace(/\\/gi, '/')}')
  }`);
    }

    const template: string = fs
      .readFileSync(path.join(this.cwd, manifest.moduleRegistration.path, manifest.moduleRegistration.template))
      .toString();

    fs.writeFileSync(
      path.join(this.cwd, manifest.moduleRegistration.path, manifest.moduleRegistration.template.replace('tstpl', 'ts')),
      template.replace(
        '/*MODULES*/',
        contents.join(`,
  `)
      )
    );
  }

  private build(): void {
    {
      if (argv.dml) {
        for (const moduleName of Object.keys(manifest.modules)) {
          const hash: string = createHash('md5')
            .update(`${moduleName}.${this.getRandomInt(10000, 99999)}`)
            .digest('hex');
          manifest.modules[moduleName].hash = hash;
        }

        if (argv.all) {
          for (const moduleName of Object.keys(manifest.modules)) {
            this.buildModule(moduleName);
          }
        } else {
          const moduleNames: string[] = this.args;

          for (const moduleName of moduleNames) {
            this.buildModule(moduleName);
          }
        }

        if (argv.manifest) {
          this.writeManifest();
        }
      } else {
        console.error(chalk.default.red('Please input build parameters.'));
      }
    }
  }

  private buildModule(name: string): void {
    if (!name) {
      throw new Error('Please enter module name. E.g., yarn build:module infrastructure');
    }

    const module: IModule = manifest.modules[name];

    if (!module) {
      throw new Error('Module does not exist. Please check ./modules/modules.json');
    }

    if (module.type === 'library') {
      this.buildLibrary(name);
    }

    if (!argv.skipBundle) {
      this.bundleModule(name, module);
    }
  }

  private buildLibrary(name: string): void {
    const msg: string = `Building @${this.prefix}/${name} package...`;
    const cmd: string = `yarn ng build ${name}`;

    this.exec(msg, cmd);
  }

  private bundleModule(moduleName: string, module: IModule): void {
    const msg: string = `
Bundling ${this.prefix}.${moduleName} to AMD module...`;
    const cmd: string = [
      `yarn ng build --prod --project ${argv.local ? 'ngmodule-local' : 'ngmodule'}`,
      '--name',
      `${this.prefix}.${moduleName}.${module.hash}`,
      '--path',
      module.path,
      '--ngmodule',
      module.module,
      '--deleteOutputPath false',
      '--output-path',
      argv.outputPath || manifest.modulesPath
    ].join(' ');

    this.exec(msg, cmd);
  }

  private writeManifest(): void {
    const manifestFile: string = path.join(this.cwd, manifest.manifestPath);
    const result: { [name: string]: IModule } = {};

    console.log(`${chalk.default.green(`Writing manifest to ${manifestFile}`)}`);

    for (const id of Object.keys(manifest.modules)) {
      const module: IModule = manifest.modules[id];
      const amdName: string = `@${this.prefix}/${id}`;
      const amdFile: string = `${this.prefix}.${id}`;

      result[id] = {
        id: amdName,
        name: module.name,
        shortName: module.shortName,
        description: module.description,
        type: module.type,
        path: `${manifest.modulesUrl}/${amdFile}.${module.hash}.js`,
        dependencies: (module.dependencies || []).map(dependency => `@${this.prefix}/${dependency}`),
        development: module.development
      };
    }

    fs.writeFileSync(manifestFile, JSON.stringify(result, null, 2));
  }

  private copyLang(): void {
    const langFolders: string[] = glob.sync(path.join(this.cwd, '{modules,libraries}/**/languages/'));
    const langDirectory: string = path.join(this.cwd, 'app-shell', 'assets', 'localization');

    Promise.all(langFolders.map(folder => fse.copy(folder, langDirectory)));

    if (argv.watch) {
      console.log(colors.cyan('Watching languages: '));
      langFolders.forEach(folder => {
        console.log(colors.cyan(`- ${folder}`));

        fs.watch(folder, (eventType: string, filename: string) => {
          const source: string = path.join(folder, filename);
          const dest: string = path.join(langDirectory, filename);

          try {
            fs.copyFileSync(source, dest);
            console.log(colors.green(`Synced language file: ${filename}`));
          } catch {
            // Ignore errors
          }
        });
      });
    }
  }

  private exec(msg: string, cmd: string, cwd?: string): void {
    console.log(`${chalk.default.green(msg)}`);
    console.log(`${chalk.default.yellow(`Executing: ${cmd}`)}`);
    execSync(cmd, {
      stdio: 'inherit',
      cwd: cwd || this.cwd
    });
  }

  private getRandomInt(min: number, max: number): number {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min)) + min;
  }
}

new Builder().run();
