import { useTranslation } from 'react-i18next';
import { CarStatus, TripStatus } from '../interfaces/enums';

const useStatusConverter = () => {
  const { t } = useTranslation();

  const TripStatusLabels = {
    [TripStatus.Created]: t("created"),
    [TripStatus.InProgress]: t("inProgress"),
    [TripStatus.WaitingForPassenger]: t("waitingForPassenger"),
    [TripStatus.Completed]: t("completed"),
    [TripStatus.Cancelled]: t("cancelled"),
  };

  const TripStatusColors = {
    [TripStatus.Created]: '#DE5DF1',
    [TripStatus.InProgress]: '#9C9405',
    [TripStatus.WaitingForPassenger]: '#9631F5',
    [TripStatus.Completed]: '#34C42F',
    [TripStatus.Cancelled]: '#F54C64',
  };

  const CarStatusLabels = {
    [CarStatus.Inactive]: t("inactive"),
    [CarStatus.Idle]: t("idle"),
    [CarStatus.EnRoute]: t("enRoute"),
    [CarStatus.OnTrip]: t("onTrip"),
    [CarStatus.WaitingForPassenger]: t("waitingForPassenger"),
    [CarStatus.Maintenance]: t("maintenance"),
    [CarStatus.Danger]: t("danger"),
  };

  const CarStatusColors = {
    [CarStatus.Inactive]: '#44474D',
    [CarStatus.Idle]: '#C77a1C',
    [CarStatus.EnRoute]: '#DE5DF1',
    [CarStatus.OnTrip]: '#9C9405',
    [CarStatus.WaitingForPassenger]: '#9631F5',
    [CarStatus.Maintenance]: '#2355A6',
    [CarStatus.Danger]: '#F54C64',
  };

  return {
    TripStatusLabels,
    TripStatusColors,
    CarStatusLabels,
    CarStatusColors,
  };
};

export default useStatusConverter;