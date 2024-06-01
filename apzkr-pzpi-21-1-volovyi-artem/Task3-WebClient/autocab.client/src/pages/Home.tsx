import ArrowForwardIcon from '@mui/icons-material/ArrowForward';
import DirectionsCarIcon from '@mui/icons-material/DirectionsCar';
import ExploreIcon from '@mui/icons-material/Explore';
import SensorsIcon from '@mui/icons-material/Sensors';
import { Box, Button, Container, IconButton, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const Home = () => {
  const { t } = useTranslation();

 return (
   <Container>
     <Box textAlign="center" mt={10}>
       <Typography variant="h4" gutterBottom>
         {t('homePageTitle')}
       </Typography>
       <Typography variant="subtitle1" color="textSecondary" paragraph>
         {t('homePageSubtitle1')}
       </Typography>

       <Box mt={4}>
         <IconButton color="primary">
           <DirectionsCarIcon fontSize="large" />
         </IconButton>
         <IconButton color="primary">
           <SensorsIcon fontSize="large" />
         </IconButton>
         <IconButton color="primary">
           <ExploreIcon fontSize="large" />
         </IconButton>
       </Box>

       <Box mt={5}>
         <Typography variant="h6" gutterBottom>
           {t('homePageSubtitle2')}
         </Typography>
         <Typography variant="body1" paragraph>
           {t('homePageSubtitle3')}
         </Typography>
       </Box>

       <Box mt={5}>
         <Button
           variant="contained"
           color="primary"
           size="large"
           endIcon={<ArrowForwardIcon />}
           component={Link}
           to="/sign-up"
         >
           {t('getStarted')}
         </Button>
       </Box>
     </Box>
   </Container>
 );
};

export default Home;
