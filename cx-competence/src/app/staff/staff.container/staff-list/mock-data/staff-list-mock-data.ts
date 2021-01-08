export const StaffListMockData = {
  HierarchicalDepartmentsMockData: [
    {
      parentDepartmentId: 1,
      identity: {
        extId: 'HRMS001',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'DataOwner',
        id: 14350,
      },
      departmentName: 'MOE',
      departmentDescription: '',
    },
    {
      parentDepartmentId: 14350,
      identity: {
        extId: 'HRMS002',
        ownerId: 3001,
        customerId: 2052,
        archetype: 'OrganizationalUnit',
        id: 14351,
      },
      departmentName: 'Professional Wing',
      departmentDescription: '',
    },
  ],
};
