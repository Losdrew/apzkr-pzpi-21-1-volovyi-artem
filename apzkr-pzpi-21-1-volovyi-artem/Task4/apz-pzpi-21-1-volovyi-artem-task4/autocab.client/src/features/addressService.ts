import axios from "axios";
import apiClient from "../config/apiClient";
import { AddressDto } from "../interfaces/address";

const getFullAddress = (address: AddressDto) => {
  const fullAddress: string[] = [];

  if (address.addressLine1) {
    fullAddress.push(`${address.addressLine1} `);
  }

  if (address.addressLine2) {
    fullAddress.push(`${address.addressLine2} `);
  }

  if (address.addressLine3) {
    fullAddress.push(`${address.addressLine3} `);
  }

  if (address.addressLine4) {
    fullAddress.push(`${address.addressLine4} `);
  }

  if (address.townCity) {
    fullAddress.push(`, ${address.townCity}`);
  }

  if (address.region) {
    fullAddress.push(`, ${address.region}`);
  }

  if (address.country) {
    fullAddress.push(`, ${address.country}`);
  }

  return fullAddress.join('');
}

const getAddresses = async (
): Promise<AddressDto[]> => {
  try {
    const response = await apiClient.get<AddressDto[]>(
      'api/Address/addresses'
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

const addressService = {
  getFullAddress,
  getAddresses
};

export default addressService;