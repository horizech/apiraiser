/**
 * Authorization Roles
 */
const authRoles = {
  admin: ['admin', 'Admin', 'SuperAdmin'],
  staff: ['admin', 'staff'],
  user: ['admin', 'staff', 'user'],
  onlyGuest: [],
};

export default authRoles;
