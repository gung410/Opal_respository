import * as buildCommon from '@microsoft/sp-build-common';
import * as coreBuild from '@microsoft/gulp-core-build';
import * as coreSass from '@microsoft/gulp-core-build-sass';
import * as coreServe from '@microsoft/gulp-core-build-serve';
import * as coreTasks from '@microsoft/sp-build-core-tasks';
import * as coreTypescript from '@microsoft/gulp-core-build-typescript';
import * as coreWebpack from '@microsoft/gulp-core-build-webpack';
import * as webpack from 'webpack';

import { Argv } from 'yargs';
import { ConfigureRigTask } from '@microsoft/sp-build-core-tasks/lib/configJson/ConfigureRigTask';
import { PrettierTask } from '../prettier/PrettierTask';
import { SpfxServeTask } from '@microsoft/sp-build-core-tasks/lib/spfxServe/SpfxServeTask';
import { ThunderWebBuildRigTask } from './ThunderWebBuildRigTask';
import { TslintTask } from '../tslint/TslintTask';
import { fileLoaderExts } from '@microsoft/sp-build-core-tasks/lib/configureWebpack/ConfigureWebpackTask';

/**
 * @public
 */
export interface IThunderWebRigArgs extends buildCommon.ISPBuildRigArgs {
  debug: boolean;
}

/**
 * @public
 */
export class ThunderWebBuildRig extends buildCommon.SPBuildRig {
  protected args!: IThunderWebRigArgs;

  constructor() {
    super();
    this._postBuildTasks = [];
  }

  protected getYargs(): Argv {
    return super
      .getYargs()
      .option('debug', {
        describe: 'runs tests in unit mode'
      })
      .option('upgrade', {
        describe: 'upgrades outdated files in the project'
      })
      .command(ThunderWebBuildRigTask.Bundle, 'builds, localizes, and bundles the project')
      .command(ThunderWebBuildRigTask.Test, 'builds, localizes, and bundles the project and runs tests, and verifies the coverage')
      .command(ThunderWebBuildRigTask.Serve, 'builds and bundles the project and runs the development server')
      .command(ThunderWebBuildRigTask.TrustDevCert, "generates and trusts a development certificate if one isn't already present")
      .command(ThunderWebBuildRigTask.UntrustDevCert, 'untrusts and deletes the development certificate if it exists')
      .command(ThunderWebBuildRigTask.Prettier, 'formats source code using prettier cli')
      .command(ThunderWebBuildRigTask.Tslint, 'runs tslint cli on git staged files and specific projects')
      .command(ThunderWebBuildRigTask.Bump, 'bumps version and updates change note')
      .command('default', 'equivalent to bundle and test')
      .option('entry', {
        describe: 'Select which entries should be bundled. This can match the GUID or the alias of the entry.',
        string: true
      });
  }

  protected getTasks(): Map<string, buildCommon.ITaskDefinition> {
    const tasks: Map<string, buildCommon.ITaskDefinition> = super.getTasks();

    tasks.set(ThunderWebBuildRigTask.Bundle, {
      executable: this.getBundleTask()
    });
    tasks.set(ThunderWebBuildRigTask.Test, {
      executable: this.getTestTask(),
      arguments: (testYargs: Argv) => {
        return testYargs
          .option('debug', {
            describe: 'run tests in debug mode'
          })
          .option('match', {
            describe: 'regular expression. Only run tests that match',
            string: true
          });
      }
    });
    tasks.set(ThunderWebBuildRigTask.Serve, {
      executable: buildCommon.serial(
        coreTasks.serve,
        buildCommon.watch(
          ['src/**/*.{ts,tsx,scss,resx,js,json,html}', '!src/**/*.{scss.ts,resx.ts}'],
          buildCommon.serial(tasks.get(ThunderWebBuildRigTask.Bundle)!.executable, coreServe.reload)
        )
      ),
      arguments: (serveYargs: Argv) => {
        return serveYargs
          .option('port', {
            description: 'the port to serve on should be the next argument (e.g. "--port 80")'
          })
          .option('nobrowser', {
            description: "don't open a browser after initial bundle"
          })
          .option('config', {
            description: 'use this option to specify which configuration to use in the serve.json file'
          });
      }
    });
    tasks.set(ThunderWebBuildRigTask.TrustDevCert, {
      executable: coreServe.trustDevCert
    });
    tasks.set(ThunderWebBuildRigTask.UntrustDevCert, {
      executable: coreServe.untrustDevCert
    });
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
    tasks.set(ThunderWebBuildRigTask.Prettier, {
      executable: new PrettierTask(),
      arguments: (prettierYargs: Argv) => getRepoYargs(prettierYargs, 'prettier')
    });
    tasks.set(ThunderWebBuildRigTask.Tslint, {
      executable: new TslintTask(),
      arguments: (tslintYargs: Argv) => getRepoYargs(tslintYargs, 'tslint')
    });

    return tasks;
  }

  protected getBundleTask(): coreBuild.IExecutable {
    return buildCommon.serial(
      this.getBuildTask(),
      coreTasks.collectLocalizedResources,
      this.getPreWebpackTasks(),
      coreTasks.configureWebpack,
      // Add first webpack instance to create the base bundle.
      coreWebpack.default,
      coreTasks.configureExternalBundlingWebpack,
      // Second webpack instance to optionally expand the base bundle.
      // This task is disabled if configureExternalBundlingWebpack isn't configured to do anything
      coreWebpack.default,
      coreTasks.copyAssets,
      coreTasks.writeManifests,
      buildCommon.parallel(this._postBuildTasks)
    );
  }

  protected getPreWebpackTasks(): buildCommon.IExecutable {
    return buildCommon.serial();
  }

  protected getTestTask(): buildCommon.IExecutable {
    return buildCommon.serial(this.getBundleTask(), coreBuild.jest);
  }

  protected getConfigureRigTask(): ConfigureRigTask {
    return coreTasks.configureRig;
  }

  protected setupSharedConfig(): void {
    super.setupSharedConfig();

    coreWebpack.default.taskConfig.webpack = webpack;
    coreSass.default.mergeConfig({
      dropCssFiles: true,
      warnOnNonCSSModules: true
    });
    coreTasks.configureWebpack.mergeConfig({
      webpack: coreWebpack.default,
      configureExternalBundlingWebpackTask: coreTasks.configureExternalBundlingWebpack
    });
    coreTasks.configureExternalBundlingWebpack.mergeConfig({
      webpack: coreWebpack.default,
      configureWebpackTask: coreTasks.configureWebpack,
      buildSingleLocale: this.args.locale
    });
    coreTasks.writeManifests.mergeConfig({
      buildSingleLocale: this.args.locale
    });
    coreBuild.copyStaticAssets.setConfig({
      includeExtensions: fileLoaderExts
    });
    coreTypescript.tscCmd.mergeConfig({
      allowBuiltinCompiler: true
    });
    coreTypescript.tslintCmd.mergeConfig({
      allowBuiltinCompiler: true
    });
    coreTypescript.apiExtractor.mergeConfig({
      allowBuiltinCompiler: true
    });

    if (this.args.production) {
      coreTasks.copyAssets.mergeConfig({
        extsToIgnore: ['.map', '.stats.json', '.stats.html']
      });
    }
  }

  protected finalizeSharedConfig(): void {
    super.finalizeSharedConfig();

    if (this.args.lite) {
      this._disableTasks(coreBuild.jest);
    }

    if (!coreTasks.writeManifests.taskConfig.deployCdnPath) {
      coreTasks.writeManifests.mergeConfig({
        deployCdnPath: coreTasks.copyAssets.taskConfig.deployCdnPath
      });
    }

    const serve: SpfxServeTask = coreTasks.serve;

    coreTasks.writeManifests.mergeConfig({
      debugBasePath: `${serve.taskConfig.https ? 'https' : 'http'}://${serve.taskConfig.hostname}:${serve.taskConfig.port}/`
    });

    if (this.args.production) {
      coreTasks.packageSolution.mergeConfig({
        paths: {
          distributionDir: coreTasks.copyAssets.taskConfig.deployCdnPath
        }
      });
    }

    // Pipe the cdnBasePath into package solution, so that it can warn
    coreTasks.packageSolution.cdnBasePath = coreTasks.writeManifests.taskConfig.cdnBasePath || '';
  }
}
