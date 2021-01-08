import { CreateSystemRoleRequest } from './create-system-role-request.dto';

export class CreateSystemRoleResponse extends CreateSystemRoleRequest {
  statusCode: string;
  message: string;
}
