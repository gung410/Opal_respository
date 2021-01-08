# CxCommon

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 7.3.3.

## Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI README](https://github.com/angular/angular-cli/blob/master/README.md).

### Library: Coding space:
* Code library at path: `"/projects/cx-angular-common/*"`
* `/projects/cx-angular-common/src/lib/abstracts/*`: contain some abstractions class for component.
* `/projects/cx-angular-common/src/lib/component/*`: contain all shared components in library.
* `/projects/cx-angular-common/src/lib/directives/*`: contain some directives.
* `/projects/cx-angular-common/src/lib/styles/*`: contain common css, variables, mixin, placeholder of scss.
* `/projects/cx-angular-common/src/lib/utils/*`: contain some utils with basic object.
* `/projects/cx-angular-common/src/lib/cx-angular-common.module.ts`: inject new component into declarations and export.
* `/projects/cx-angular-common/src/public_api.ts`: when you've created a new shared component then you can import it at this file. Should be public model if you want.

### Test function: coding space:
* Code library at path: `"/src/app/*"`

### Naming:
* Use snake case for naming selector
* Type of component separated by dot, ex: *.component, *.service, *.pipe, *.constant, *.abstract
There are currently 8 types of file in the source code of project: module, component, service, pipe, constant, abstract, model, index (for exporting api from components)
* Name of shared component with prefix 'cx-' (component name, class scss, selector, html....)
* Model: should be use prefix 'cx-'
* For route config: do not include slash at the end, ex: https://vip24.cloud.tyk.io instead of https://vip24.cloud.tyk.io/

### Build library (Compile):
* After build library you should increase new version on `/projects/cx-angular-common/package.json` ("version": "1.0.12")
* Build: cd to root path -> run terminal and input command: `ng build CxCommon`
* To try to use package locally without publishing, do the following steps:
    - Run script 'npm run build:lib' to build the current source to library
    - In the consuming project, install locally library with command: npm i <absolute-path-to-local-library-project>/dist/cx-angular-common