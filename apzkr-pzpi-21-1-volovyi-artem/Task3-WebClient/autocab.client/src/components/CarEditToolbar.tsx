import LocationOnIcon from "@mui/icons-material/LocationOn";
import { Button } from "@mui/material";
import { useTranslation } from "react-i18next";
import EditToolbar, { EditToolbarProps } from "./EditToolbar";

export interface CarEditToolbarProps extends EditToolbarProps {
  setModal: React.Dispatch<React.SetStateAction<boolean>>
}

const CarEditToolbar: React.FC<CarEditToolbarProps> = (props) => {
  const { setRows, setRowModesModel, setModal } = props;
  const { t } = useTranslation();

  const handleViewLocation = () => {
    setModal(true);
  };

  return (
    <EditToolbar setRows={setRows} setRowModesModel={setRowModesModel}>
      <Button color="primary" startIcon={<LocationOnIcon />} onClick={handleViewLocation}>
        {t("viewLocation")}
      </Button>
    </EditToolbar>
  );
};

export default CarEditToolbar;