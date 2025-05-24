import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../../services/AuthContext';

import LayoutPublic from '../layout/LayoutPublic';
import LoadingSpinner from '../atoms/LoadingSpinner';

const PublicRoute = () => {
  const { loading, isLogged, isAuthenticated, isAdmin } = useAuth();

  if (loading) {
    return (
      <LayoutPublic>
        <div className="flex justify-center items-center h-screen">
          <LoadingSpinner/>
        </div>
      </LayoutPublic>
    );
  }

  if (isLogged && !isAdmin) {
    return <Navigate to="/sistema/vinculos" />;
  }
  
  if (isLogged && isAdmin) {
    return <Navigate to="/sistema" />;
  }
  
  if (isAuthenticated) {
    return <Navigate to="/clinica/inicio" />;
  }

  return <Outlet />;
}

export default PublicRoute;
