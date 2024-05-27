import axios from "axios";
import apiClient from "../config/apiClient";
import { AddressDto } from "../interfaces/address";
import { CreateServiceCommand, EditServiceCommand, ServiceInfoDto } from "../interfaces/service";
import { CancelOwnTripCommand, CreateTripCommand, TripFullInfo, TripInfoDto, UpdateTripServicesCommand } from "../interfaces/trip";
import { CarInfoDto } from "../interfaces/car";

const getTrips = async (
  bearerToken: string
): Promise<TripFullInfo[]> => {
  try {
    const headers = {
      'Authorization': 'Bearer ' + bearerToken
    };
    const tripResponse = await apiClient.get<TripInfoDto[]>(
      'api/Trip/trips',
      { headers }
    );
    const tripsFullInfo = [];

    for (const trip of tripResponse.data) {
      const carResponse = await apiClient.get<CarInfoDto>(
        '/api/Car/car?carId=' + trip.carId,
        { headers }
      );

      const tripFullInfo: TripFullInfo = {
        ...trip,
        car: carResponse.data,
        startDateTime: new Date(Date.parse(trip.startDateTime?.toString())),
      };

      tripsFullInfo.push(tripFullInfo);
    }

    return tripsFullInfo;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      throw new Error(error.response?.data.message);
    } else {
      throw new Error("Unknown error occurred.");
    }
  }
};

const getUserTrips = async (
  bearerToken: string
): Promise<TripFullInfo[]> => {
  try {
    const headers = {
      'Authorization': 'Bearer ' + bearerToken
    };
    const tripResponse = await apiClient.get<TripInfoDto[]>(
      'api/Trip/user-trips',
      { headers }
    );
    const tripsFullInfo = [];

    for (const trip of tripResponse.data) {
      const carResponse = await apiClient.get<CarInfoDto>(
        '/api/Car/car?carId=' + trip.carId,
        { headers }
      );

      const tripFullInfo: TripFullInfo = {
        ...trip,
        car: carResponse.data,
        startDateTime: new Date(Date.parse(trip.startDateTime?.toString())),
      };

      tripsFullInfo.push(tripFullInfo);
    }

    return tripsFullInfo;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      throw new Error(error.response?.data.message);
    } else {
      throw new Error("Unknown error occurred.");
    }
  }
};

const createTrip = async (
  price: number,
  startAddress: AddressDto,
  destinationAddress: AddressDto,
  carId: string,
  bearerToken: string
): Promise<TripInfoDto> => {
  try {
    const request: CreateTripCommand = {
      price,
      startAddress,
      destinationAddress,
      carId
    }
    const headers = {
      'Authorization': 'Bearer ' + bearerToken
    };
    const response = await apiClient.post<TripInfoDto>(
      'api/Trip/create',
      request,
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

const getServices = async (
): Promise<ServiceInfoDto[]> => {
  try {
    const response = await apiClient.get<ServiceInfoDto[]>(
      'api/Service/services'
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

const createService = async (
  name: string,
  command: string,
  bearerToken: string
): Promise<ServiceInfoDto> => {
  try {
    const request: CreateServiceCommand = {
      name,
      command
    }
    const headers = {
      'Authorization': 'Bearer ' + bearerToken
    };
    const response = await apiClient.post<ServiceInfoDto>(
      'api/Service/create',
      request,
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

const editService = async (
  bearerToken: string,
  id?: string,
  name?: string,
  command?: string,
): Promise<ServiceInfoDto> => {
  try {
    const request: EditServiceCommand = {
      id,
      name,
      command
    }
    const headers = {
      'Authorization': 'Bearer ' + bearerToken
    };
    const response = await apiClient.post<ServiceInfoDto>(
      'api/Service/edit',
      request,
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

const updateTripServices = async (
  tripId: string,
  services: string[],
  bearerToken: string
): Promise<ServiceInfoDto> => {
  try {
    const request: UpdateTripServicesCommand = {
      tripId,
      services
    }
    const headers = {
      'Authorization': 'Bearer ' + bearerToken
    };
    const response = await apiClient.post<ServiceInfoDto>(
      'api/Trip/update-services',
      request,
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

const cancelOwnTrip = async (
  tripId: string,
  bearerToken: string,
): Promise<boolean> => {
  try {
    const request: CancelOwnTripCommand = { tripId };
    const headers = {
      'Authorization': 'Bearer ' + bearerToken
    };
    await apiClient.post(
      '/api/Trip/cancel',
      request,
      { headers }
    );
    return true;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      throw new Error(error.response?.data.message);
    } else {
      throw new Error("Unknown error occurred.");
    }
  }
};

const tripService = {
  getTrips,
  getUserTrips,
  createTrip,
  getServices,
  createService,
  editService,
  updateTripServices,
  cancelOwnTrip
};

export default tripService;