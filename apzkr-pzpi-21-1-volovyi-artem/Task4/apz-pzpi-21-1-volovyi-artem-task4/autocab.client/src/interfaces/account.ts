export interface AuthResultDto {
  userId?: string;
  bearer?: string | undefined;
  role?: string;
}

export interface SignInCommand {
  email?: string | undefined;
  password?: string | undefined;
}

export interface CreateUserCommand {
  email?: string | undefined;
  password?: string | undefined;
  firstName?: string | undefined;
  lastName?: string | undefined;
  phoneNumber?: string | undefined;
}

export interface CreateCustomerCommand extends CreateUserCommand { }
export interface CreateAdminCommand extends CreateUserCommand { }

export interface UserInfoDto {
  id?: string | undefined;
  email?: string | undefined;
  firstName?: string | undefined;
  lastName?: string | undefined;
  phoneNumber?: string | undefined;
  role?: string | undefined;
}
