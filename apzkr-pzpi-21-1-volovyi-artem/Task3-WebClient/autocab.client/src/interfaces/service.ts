export interface ServiceInfoDto {
  id: string;
  name?: string;
  command?: string;
}

export interface CreateServiceCommand {
  name: string;
  command: string;
}

export interface EditServiceCommand {
  id?: string;
  name?: string;
  command?: string;
}