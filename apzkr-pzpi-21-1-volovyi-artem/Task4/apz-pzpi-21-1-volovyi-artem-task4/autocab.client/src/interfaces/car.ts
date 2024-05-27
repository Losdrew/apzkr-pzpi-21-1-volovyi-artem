import { AddressDto } from "./address";
import { CarStatus } from "./enums";
import { LocationDto } from "./geolocation";

export interface CarInfoDto {
  brand?: string;
  model?: string | undefined;
  licencePlate ?: string;
  status?: CarStatus;
  location?: LocationDto;
  passengerSeatsNum ?: number;
  temperature?: number;
  fuel?: number;
  id?: string;
}

export interface CarForTripDto {
  id?: string;
  brand?: string;
  model?: string | undefined;
  licencePlate?: string;
  location?: LocationDto;
  passengerSeatsNum?: number;
  price?: number;
}

export interface CreateCarCommand {
  brand?: string;
  model?: string | undefined;
  licencePlate?: string;
  passengerSeatsNum?: number;
  deviceId?: string | undefined;
}

export interface EditCarCommand {
  id?: string;
  brand?: string;
  model?: string | undefined;
  licencePlate?: string;
  passengerSeatsNum?: number;
  deviceId?: string | undefined;
}

export interface SetCarStatusCommand {
  id?: string;
  newStatus?: CarStatus;
}

export interface GetCarsForTripQuery {
  startAddress: AddressDto;
  destinationAddress: AddressDto;
}
