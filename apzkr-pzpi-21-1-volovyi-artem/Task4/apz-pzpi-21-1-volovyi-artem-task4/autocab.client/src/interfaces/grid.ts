import { GridValidRowModel } from "@mui/x-data-grid/models";
import { UserInfoDto } from "./account";
import { CarInfoDto } from "./car";
import { ServiceInfoDto } from "./service";
import { TripFullInfo } from "./trip";

export interface GridCar extends CarInfoDto, GridValidRowModel { }
export interface GridUser extends UserInfoDto, GridValidRowModel { }
export interface GridTrip extends TripFullInfo, GridValidRowModel { }
export interface GridService extends ServiceInfoDto, GridValidRowModel { }