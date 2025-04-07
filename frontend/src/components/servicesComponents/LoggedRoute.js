import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../../services/AuthContext';
import LoadingSpinner from '../atoms/LoadingSpinner';

const LoggedRoute = () => {
  const { isLogged, loading } = useAuth();

  if (loading) return <LoadingSpinner />;

  return isLogged ? <Outlet /> : <Navigate to="/login" />;
};

export default LoggedRoute;