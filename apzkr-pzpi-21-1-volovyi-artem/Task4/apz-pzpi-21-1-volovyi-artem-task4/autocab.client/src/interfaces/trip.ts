import { AddressDto } from "./address";
import { CarInfoDto } from "./car";
import { TripStatus } from "./enums";
import { ServiceInfoDto } from "./service";

export interface CreateTripCommand {
  price: number;
  startAddress?: AddressDto;
  destinationAddress?: AddressDto;
  carId: string;
}

export interface TripInfoDto {
  id: string;
  userId: string;
  tripStatus: TripStatus;
  startDateTime: Date;
  price: number;
  startAddress?: AddressDto;
  destinationAddress?: AddressDto;
  carId: string;
  services?: ServiceInfoDto[];
}

export interface TripFullInfo {
  id: string;
  userId: string;
  tripStatus: TripStatus;
  startDateTime: Date;
  price: number;
  startAddress?: AddressDto;
  destinationAddress?: AddressDto;
  car: CarInfoDto;
  services?: ServiceInfoDto[];
}

export interface CancelOwnTripCommand {
  tripId: string;
}

export interface UpdateTripServicesCommand {
  tripId: string;
  services: string[];
}