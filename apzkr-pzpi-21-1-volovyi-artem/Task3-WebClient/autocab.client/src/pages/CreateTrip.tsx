import DriveEtaIcon from '@mui/icons-material/DriveEta';
import PersonIcon from '@mui/icons-material/Person';
import {
    Autocomplete,
    Box,
    Button,
    Container,
    FormControl,
    InputLabel,
    MenuItem,
    Paper,
    Select,
    TextField,
    Typography
} from '@mui/material';
import React, { useEffect, useState } from "react";
import { useTranslation } from 'react-i18next';
import addressService from '../features/addressService';
import carService from '../features/carService';
import tripService from "../features/tripService";
import useAuth from '../hooks/useAuth';
import { AddressDto } from '../interfaces/address';
import { CarForTripDto } from '../interfaces/car';
import { TripInfoDto } from "../interfaces/trip";

const CreateTrip = () => {
  const { auth } = useAuth();
  const { t } = useTranslation();

  const [addresses, setAddresses] = useState<AddressDto[]>();
  const [trip, setTrip] = useState<TripInfoDto>();
  const [startAddress, setStartAddress] = useState<AddressDto | null>();
  const [destinationAddress, setDestinationAddress] = useState<AddressDto | null>();
  const [carsForTrip, setCarsForTrip] = useState<CarForTripDto[]>();
  const [selectedCar, setSelectedCar] = useState<CarForTripDto>();

  useEffect(() => {
    const fetchAddresses = async () => {
      try {
        if (auth.bearer) {
          const response = await addressService.getAddresses();
          setAddresses(response);
        }
      } catch (error) {
        console.error('Error fetching addresses:', error);
      }
    };

    fetchAddresses();
  }, []);

  useEffect(() => {
    const fetchCarsForTrip = async () => {
      try {
        if (auth.bearer) {
          const response = await carService.getCarsForTrip(
            startAddress,
            destinationAddress,
            auth.bearer!
          );
          setCarsForTrip(response);
        }
      } catch (error) {
        console.error('Error fetching cars for trip:', error);
      }
    };

    if (startAddress && destinationAddress) {
      fetchCarsForTrip();
    }
  }, [startAddress, destinationAddress]);

  const handleCreateTrip = async () => {
    try {
      const response = await tripService.createTrip(
        selectedCar?.price,
        startAddress,
        destinationAddress,
        selectedCar?.id,
        auth.bearer!
      );
      setTrip(response);
    } catch (error) {
      console.error('Error');
    }
  };

  return (
    <Container maxWidth="xs">
      {trip ? (
        <Typography variant="h5" gutterBottom align="center" mt={4} color="primary">
          {t("tripCreatedSuccessfully")}
        </Typography>
      ) :
        (
          <React.Fragment>
            <Typography variant="h5" gutterBottom align="center" mt={2} mb={2}>
              {t("createTrip")}
            </Typography>
            <Paper elevation={3} style={{ padding: '30px' }}>
              <Box>
                <Autocomplete
                  fullWidth
                  clearOnEscape
                  id="combo-box-start-address"
                  options={addresses}
                  onChange={
                    (event, newValue) => {
                      setStartAddress(newValue);
                    }
                  }
                  getOptionLabel={
                    (option: AddressDto) => addressService.getFullAddress(option)
                  }
                  filterOptions={(options, { inputValue }) =>
                    options.filter((option) =>
                      addressService.getFullAddress(option).toLowerCase().includes(inputValue.toLowerCase())
                    )
                  }
                  renderInput={
                    (params) => <TextField {...params} label={t("startAddress")} />
                  }
                  isOptionEqualToValue={(option, value) => option.id === value.id}
                  getOptionKey={(option) => option.id}
                />
              </Box>
              <Box mt={3}>
                <Autocomplete
                  fullWidth
                  clearOnEscape
                  id="combo-box-destination-address"
                  options={addresses}
                  onChange={
                    (event, newValue) => {
                      setDestinationAddress(newValue);
                    }
                  }
                  getOptionLabel={
                    (option: AddressDto) => addressService.getFullAddress(option)
                  }
                  filterOptions={(options, { inputValue }) =>
                    options.filter((option) =>
                      addressService.getFullAddress(option).toLowerCase().includes(inputValue.toLowerCase())
                    )
                  }
                  renderInput={
                    (params) => <TextField {...params} label={t("destinationAddress")} />
                  }
                  isOptionEqualToValue={(option, value) => option.id === value.id}
                  getOptionKey={(option) => option.id}
                />
              </Box>
              <Box mt={3}>
                <FormControl fullWidth>
                  <InputLabel id="car-select-label">{t("car")}</InputLabel>
                  <Select
                    labelId="car-select-label"
                    id="car-select"
                    value={selectedCar}
                    label={t("car")}
                    onChange={(event) => setSelectedCar(event.target.value)}
                  >
                    {carsForTrip?.map((car) => (
                      <MenuItem key={car.id} value={car}>
                        <Box display="flex" flexDirection="row" justifyContent="space-between" alignItems="center" width="100%">
                          <Box display="flex" flexDirection="row" gap="15px">
                            <Box display="flex" flexDirection="column-reverse" justifyContent="center">
                              <DriveEtaIcon fontSize="large" color="primary" />
                            </Box>
                            <Box display="flex" flexDirection="column" justifyContent="center">
                              <Typography fontWeight="bold"> {car.brand + " " + car.model} </Typography>
                              <Box display="flex">
                                <PersonIcon font-size="small" /> {car.passengerSeatsNum}
                              </Box>
                            </Box>
                          </Box>
                          <Box display="flex" flexDirection="column-reverse" justifyContent="center">
                            <Typography fontWeight="bold" fontSize="22px"> {car.price?.toPrecision(3)}$ </Typography>
                          </Box>
                        </Box>
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Box>
              <Box display="flex" justifyContent="center" mt={3}>
                <Button variant="contained" color="primary" onClick={handleCreateTrip}>
                  {t("createTrip")}
                </Button>
              </Box>
            </Paper>
          </React.Fragment>
        )}
    </Container>
  )
};

export default CreateTrip;
