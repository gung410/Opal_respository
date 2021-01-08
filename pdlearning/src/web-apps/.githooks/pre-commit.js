// @ts-check
const { execSync } = require('child_process');

try {
  execSync('yarn lint-staged', {
    stdio: 'inherit'
  });
} catch (error) {
  console.error('Please fix lint-staged errors and try again!');
  process.exit(1);
}
