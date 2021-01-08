npm run build:lib
cd dist/cx-angular-common
echo "registry = https://cx-npm.development.opal2.conexus.net" >> .npmrc
echo "_auth = YWRtaW46VGhyZWUya2lsbA==" >> .npmrc
echo "email = conexus@conexus.net" >> .npmrc
echo "always-auth = true" >> .npmrc
npm version 3.0.30
npm publish
