import axios from "axios";
import apiClient from "../config/apiClient";
import { AuthResultDto, CreateAdminCommand, CreateCustomerCommand, SignInCommand } from "../interfaces/account";

const signIn = async (
  email : string,
  password : string
): Promise<AuthResultDto> => {
  try {
    const request: SignInCommand = { email, password };
    const response = await apiClient.post<AuthResultDto>(
      '/api/Account/sign-in',
      request
    );
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      throw new Error(error.response?.data.message);
    } else {
      throw new Error("Unknown error occurred.");
    }
  }
};

const signUpCustomer = async (
  email : string,
  password : string,
  firstName : string,
  lastName: string,
  phoneNumber?: string
): Promise<AuthResultDto> => {
  try {
    const request: CreateCustomerCommand = { 
      email, 
      password, 
      firstName, 
      lastName,
      phoneNumber
    };
    const response = await apiClient.post<AuthResultDto>(
      '/api/Account/customer/create',
      request
    );
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      throw new Error(error.response?.data.message);
    } else {
      throw new Error("Unknown error occurred.");
    }
  }
};

const signUpAdmin = async (
  email: string,
  password: string,
  firstName: string,
  lastName: string,
  phoneNumber?: string
): Promise<AuthResultDto> => {
  try {
    const request: CreateAdminCommand = {
      email,
      password,
      firstName,
      lastName,
      phoneNumber
    };
    const response = await apiClient.post<AuthResultDto>(
      '/api/Account/administrator/create',
      request
    );
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      throw new Error(error.response?.data.message);
    } else {
      throw new Error("Unknown error occurred.");
    }
  }
};

const authService = {
  signIn,
  signUpCustomer,
  signUpAdmin
};

export default authService;