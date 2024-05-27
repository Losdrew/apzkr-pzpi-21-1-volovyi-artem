import {
    Button,
    Checkbox,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    FormControlLabel,
    FormGroup
} from '@mui/material';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import tripService from '../features/tripService';
import useAuth from '../hooks/useAuth';
import { ServiceInfoDto } from '../interfaces/service';

const SelectServicesModal = ({ open, onClose, tripId }) => {
  const { auth } = useAuth();
  const { t } = useTranslation();
  const [services, setServices] = useState<ServiceInfoDto[]>([]);
  const [selectedServices, setSelectedServices] = useState<string[]>([]);

  const handleCheckboxChange = (serviceId: string) => {
    if (selectedServices.includes(serviceId)) {
      setSelectedServices(selectedServices.filter(id => id !== serviceId));
    } else {
      setSelectedServices([...selectedServices, serviceId]);
    }
  };

  useEffect(() => {
    const fetchServices = async () => {
      try {
        const response = await tripService.getServices();
        setServices(response);
      } catch (error) {
        console.error('Error fetching available cars:', error);
      }
    };

    if (open) {
      fetchServices();
    }
  }, [auth.bearer, open]);

  const handleUpdateServices = async () => {
    await tripService.updateTripServices(
      tripId,
      selectedServices,
      auth.bearer!
    );
    onClose();
  };

  return (
    <Dialog open={open} onClose={onClose}>
      <DialogTitle>{t("selectServices")}</DialogTitle>
      <DialogContent>
        <FormGroup>
          {services.map((service) => (
            <FormControlLabel
              key={service.id}
              control={
                <Checkbox
                  checked={selectedServices.includes(service.id)}
                  onChange={() => handleCheckboxChange(service.id)} />
              }
              label={service.name}
            />
          ))}
        </FormGroup>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} color="primary">
          {t("cancel")}
        </Button>
        <Button onClick={handleUpdateServices} color="primary">
          {t("confirm")}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default SelectServicesModal;
