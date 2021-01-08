// @ts-check

const chalk = require('chalk').default;
const fs = require('fs');
const path = require('path');
const child_process = require('child_process');

let forcedRebuild = false;
let ignoreMonorepoTasks = false;

try {
  const argv = JSON.parse(process.env.npm_config_argv).original;
  forcedRebuild = argv.indexOf('--rebuild') > -1;
  ignoreMonorepoTasks = argv.indexOf('--ignore-monorepo-tasks') > -1;
} catch {}

// Build tool projects
build(path.join(__dirname, '..', 'tools', 'prettier-plugin-formatter'), '@thunder/prettier-plugin-formatter', forcedRebuild);
build(path.join(__dirname, '..', 'tools', 'build-system'), '@thunder/build-system', forcedRebuild);
build(path.join(__dirname, '..', 'tools', 'bump'), '@thunder/bump', forcedRebuild);
build(path.join(__dirname, '..', 'src', 'toolkit', 'plugin-builders'), '@thunder/angular-plugin-builders', forcedRebuild);

const { Git, Repo } = require('@thunder/build-system');

if (!ignoreMonorepoTasks) {
  // Git v2.9.0 supports a custom hooks directory, so we include local .gitconfig to apply core.hooksPath
  Git.setLocalConfigPath('../.gitconfig');

  // Register monorepo into git pre-commit
  const monorepoName = '@opal20/platform';
  const monorepoJsonPath = path.join(Repo.gitRoot, '.githooks', 'monorepo.json');
  const monorepoJson = JSON.parse(fs.readFileSync(monorepoJsonPath, 'utf-8'));
  const monorepoRelativePath = path.relative(Repo.gitRoot, Repo.repoRoot);
  monorepoJson[monorepoName] = path.join(monorepoRelativePath, '.githooks', 'pre-commit.js');
  fs.writeFileSync(monorepoJsonPath, JSON.stringify(monorepoJson, null, 2));
  console.log(chalk.green(`The monorepo ${monorepoName} is registered into git pre-commit!`));
}

console.log(`
${chalk.green('All depenencies are installed! This repo no longer automatically run builds when installing dependencies.')}
For innerloop development, run these commands:
  ${chalk.yellow('cd ./apps/abc')}
  ${chalk.yellow('yarn start')}
`);

function build(project, name, forcedRebuild) {
  if (!fs.existsSync(path.join(project, 'lib')) || forcedRebuild === true) {
    console.log(`${chalk.green(`Build ${name} package...`)}`);
    child_process.execSync('yarn build', {
      stdio: 'inherit',
      cwd: project
    });
  }
}
