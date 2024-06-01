import { Route, Routes } from 'react-router-dom';
import Layout from './components/Layout';
import RequireAuth from './components/RequireAuth';
import { Roles } from './interfaces/enums';
import AdminDashboard from './pages/AdminDashboard';
import CreateTrip from './pages/CreateTrip';
import CustomerTrips from './pages/CustomerTrips';
import Home from './pages/Home';
import Login from './pages/Login';
import SignUp from './pages/SignUp';

function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route path="/" element={<Home />} />
        <Route path="/sign-up" element={<SignUp />} />
        <Route path="/login" element={<Login />} />

        <Route element={<RequireAuth allowedRoles={[Roles.Administrator]} />}>
          <Route path="/admin-dashboard" element={<AdminDashboard />} />
        </Route>

        <Route element={<RequireAuth allowedRoles={[Roles.Customer]} />}>
          <Route path="/customer-trips" element={<CustomerTrips />} />
          <Route path="/create-trip" element={<CreateTrip />} />
        </Route>
      </Route>
    </Routes>
  );
}

export default App;