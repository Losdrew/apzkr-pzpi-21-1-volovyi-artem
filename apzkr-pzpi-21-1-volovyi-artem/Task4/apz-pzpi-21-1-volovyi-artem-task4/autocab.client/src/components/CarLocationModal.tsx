import { Button, Dialog, DialogActions, DialogContent, DialogTitle } from '@mui/material';
import 'mapbox-gl/dist/mapbox-gl.css';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { LocationDto } from '../interfaces/geolocation';
import MapboxMap from './MapboxMap';

const CarLocationModal = ({ open, handleClose, selectedCar }) => {
  const [currentLocation, setCurrentLocation] = useState<LocationDto>();
  const { t } = useTranslation();

  useEffect(() => {
    setCurrentLocation(selectedCar?.location);
  }, [open, selectedCar]) 

  return (
    <Dialog open={open} onClose={handleClose}>
      <DialogTitle>{t("carLocation")}</DialogTitle>
      <DialogContent>
        <MapboxMap currentLocation={[currentLocation?.x, currentLocation?.y]} />
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose} color="primary">
          {t("close")}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default CarLocationModal;
