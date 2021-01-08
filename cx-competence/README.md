# CxAngularTemplate

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 7.3.1.

## Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory. Use the `--prod` flag for a production build.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via [Protractor](http://www.protractortest.org/).

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI README](https://github.com/angular/angular-cli/blob/master/README.md).

### Component file naming convention: Follow the naming convention of angular
* Use snake case for naming selector
* Type of component separated by dot, ex: *.component, *.service, *.pipe, *.constant, *abstract
There are currently 7 types of file in the source code of project: module, component, service, pipe, constant, abstract, model
* For route config: do not include slash at the end, ex: https://vip24.cloud.tyk.io instead of https://vip24.cloud.tyk.io/

### Style:
* Use Bootstrap 4 framework (with slim Jquery included) for faster development
* Follow BEM for easier to write and maintain
* Avoid css class name of Bootstrap to avoid style collision

### Oidc 
* This template is include oidc implicit work follow supportation
* For the Routes that we want to send them in the returnUrl when redirect to IDP for login 
* setting sendInReturnUrlOidc to true
* Example {
*        path: 'url',
*        canActivate: [AuthGuard],
*        loadChildren: 'app/landing-page/landing-page.module#module1',
*        data :{ sendInReturnUrlOidc : true }
*    },

### Folder structure: Only have Root module, Feature modules and share module (to share between features)
### For npm packages version: To maintain the stable of our app, we use compatible version by adding '~' to the beginning of package in package.json (or replace '^')

