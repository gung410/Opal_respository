import { CxTreeButtonCondition } from 'projects/cx-angular-common/src';

export const buttonConditionsData = new CxTreeButtonCondition({
    enableAdd: x => true,
    enableEdit: x => true,
    enableMove: x => true,
    enableRemove: x => true,
});
