import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../../services/AuthContext';
import LoadingSpinner from '../atoms/LoadingSpinner';
import LayoutPublic from '../layout/LayoutPublic';

const LoggedRoute = () => {
  const { isLogged, isAuthenticated, isAdmin, loading } = useAuth();

  if (loading) return (
    <LayoutPublic>
      <div className="flex justify-center items-center h-screen">
        <LoadingSpinner />
      </div>
    </LayoutPublic>
  );
  
  if (isLogged && isAdmin) {
    return <Navigate to="/sistema" replace />;
  }

  if (!isLogged) {
    return <Navigate to="/login" replace />;
  }
  
  if (isAuthenticated) {
    return <Navigate to="/clinica/inicio" replace />;
  }
  
  // if (isLogged && !isAuthenticated && !isAdmin) {
  //   return <Navigate to="/sistema/vinculos" replace />;
  // }
  
  return <Outlet />

};

export default LoggedRoute;