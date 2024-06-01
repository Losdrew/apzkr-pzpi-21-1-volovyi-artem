export enum Roles {
  None = "",
  Administrator = "admin",
  Customer = "customer"
}

export enum CarStatus {
  Inactive,
  Idle,
  EnRoute,
  WaitingForPassenger,
  OnTrip,
  Maintenance,
  Danger
}

export enum TripStatus {
  Created,
  InProgress,
  WaitingForPassenger,
  Completed,
  Cancelled
}