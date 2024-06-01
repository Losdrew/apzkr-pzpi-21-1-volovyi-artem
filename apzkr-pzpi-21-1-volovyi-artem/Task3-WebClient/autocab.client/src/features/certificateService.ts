import axios from "axios";
import apiClient from "../config/apiClient";
import { CertificateInfoDto } from "../interfaces/certificate";

const getCertificateInfo = async (
  bearerToken: string
): Promise<CertificateInfoDto> => {
  try {
    const headers = {
      'Authorization': 'Bearer ' + bearerToken,
    };
    const response = await apiClient.get<CertificateInfoDto>(
      '/api/Certificate/get-certificate',
      { headers }
    );

    return {
      ...response.data,
      issuedDate: new Date(Date.parse(response.data.issuedDate?.toString())),
      expiryDate: new Date(Date.parse(response.data.issuedDate?.toString()))
    };
  } catch (error) {
    if (axios.isAxiosError(error)) {
      throw new Error(error.response?.data.message);
    } else {
      throw new Error("Unknown error occurred.");
    }
  }
};

const exportCertificate = async (
  bearerToken: string
) => {
  try {
    const headers = {
      'Authorization': 'Bearer ' + bearerToken,
    };
    const response = await apiClient.get(
      '/api/Certificate/export',
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

const importCertificate = async (
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
      '/api/Certificate/import',
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

const certificateService = {
  getCertificateInfo,
  exportCertificate,
  importCertificate
};

export default certificateService;