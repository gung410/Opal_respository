import { UserRoleEnum } from 'app/shared/constants/user-roles.enum';

export function AddingMoreOptionsBaseOnRole(
  defaultOptionModel: { [k: string]: any },
  role: string
): object {
  switch (role) {
    case UserRoleEnum.BranchAdmin:
    case UserRoleEnum.SuperAdministrator:
      return defaultOptionModel;

    case UserRoleEnum.DivisionAdmin:
      defaultOptionModel.exportOption.exportFields.departmentName = 'branch';

      return defaultOptionModel;

    case UserRoleEnum.OverallSystemAdministrator:
    case UserRoleEnum.UserAccountAdministrator:
      defaultOptionModel.exportOption.exportFields.departmentName =
        'Place of Work';
      defaultOptionModel.exportOption.summaryOption.countByFieldValueCaption =
        'Total number of accounts (in each organisation)';
      defaultOptionModel.exportOption.summaryOption.countByField =
        'departmentName';
      defaultOptionModel.exportOption.summaryOption.showTotalBeforeCountByField = false;

      return defaultOptionModel;
    default:
      return defaultOptionModel;
  }
}

export function DefaultOptionModel(
  parentDepartmentId: number
): { [k: string]: any } {
  const locale = window.navigator.language;
  const date = new Intl.DateTimeFormat(locale).formatToParts(new Date());
  const dateFormat = date
    .map((obj) => {
      switch (obj.type) {
        case 'day':
          return 'dd';
        case 'month':
          return 'MM';
        case 'year':
          return 'yyyy';
        default:
          return obj.value;
      }
    })
    .join('');

  return {
    parentDepartmentId: [parentDepartmentId],
    userEntityStatuses: ['Inactive', 'Active', 'New', 'Archived'],
    filterOnSubDepartment: true,
    exportOption: {
      exportFields: {
        firstName: 'Name of staff',
        emailAddress: 'Email Address',
        systemRoles: 'System roles',
        statusId: 'Account status',
        created: 'Date account created',
        firstLoginDate: 'Date activated',
        lastLoginDate: 'Date of last login',
        expirationDate: {
          caption: 'Account Termination Date/Expiry',
          diplayFormat: dateFormat
        }
      },
      summaryOption: {
        countTotal: true,
        countTotalDisplayText: 'Total number of accounts'
      },
      summaryPosition: 'Bottom',
      showRecordType: true,
      exportType: 'Excel',
      dateTimeFormat: dateFormat + ' HH:mm'
    },
    pageSize: 0,
    pageIndex: 0,
    orderBy: 'Created desc',
    sendEmail: true,
    emailOption: {
      subject: 'Exporting User Accounts'
    }
  };
}

export class InstructionReporting {
  downloadUrl: string;
  fileName: string;
  message: string;
  filePath: string;
}
