import axios from "axios";
import apiClient from "../config/apiClient";

const exportData = async (
  bearerToken: string
) => {
  try {
    const headers = {
      'Authorization': 'Bearer ' + bearerToken,
    };
    const response = await apiClient.get(
      '/api/Data/export-database',
      { headers, responseType: 'arraybuffer' }
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

const importData = async (
  file: File,
  bearerToken: string
) => {
  try {
    const formData = new FormData();
    formData.append('file', file);
    const headers = {
      'Authorization': 'Bearer ' + bearerToken,
      'Content-Type': 'multipart/form-data',
    };
    const response = await apiClient.post(
      '/api/Data/import-database',
      formData,
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

const dataService = {
  exportData,
  importData
};

export default dataService;