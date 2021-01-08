The first Version: 1.0.12
### Release note: 
Initialize library project with button demo 
```html 
<cx-button [closeable]="true" [type]="'primary'" size="sm" 
      (clicked)="onBtnClicked($event)" (closed)="onBtnClosed($event)">Test button</cx-button>
```

### Library: Coding space:
* Code library at path: "/projects/cx-angular-common/*"
* /projects/cx-angular-common/src/lib/abstracts/*: contain some abstractions class for component.
* /projects/cx-angular-common/src/lib/component/*: contain all shared components in library.
* /projects/cx-angular-common/src/lib/directives/*: contain some directives.
* /projects/cx-angular-common/src/lib/styles/*: contain common css, variables, mixin, placeholder of scss.
* /projects/cx-angular-common/src/lib/utils/*: contain some utils with basic object.
* /projects/cx-angular-common/src/lib/cx-angular-common.module.ts: inject new component into declarations and export.
* /projects/cx-angular-common/src/public_api.ts: when you've created a new shared component then you can import it at this file.

### Test function: coding space:
* Code library at path: "/src/app/*"

### Build library (Compile):
* After build library you should increase new version on /projects/cx-angular-common/package.json ("version": "1.0.12")
* Build: cd to root path -> run terminal and input command: `ng build CxCommon`