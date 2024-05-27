import axios from "axios";
import apiClient from "../config/apiClient";
import { UserInfoDto } from "../interfaces/account";

const getUsers = async (
  bearerToken: string
): Promise<UserInfoDto[]> => {
  try {
    const headers = {
      'Authorization': 'Bearer ' + bearerToken,
    };
    const response = await apiClient.get<UserInfoDto[]>(
      'api/Account/users',
      { headers }
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

const userService = {
  getUsers
};

export default userService;