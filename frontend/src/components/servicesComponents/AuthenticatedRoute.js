import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../../services/AuthContext';

import LoadingSpinner from '../atoms/LoadingSpinner';
import LayoutLogged from '../layout/LayoutLogged';

const AuthenticatedRoute = () => {
  const { isAuthenticated, isAdmin, isLogged, loading } = useAuth();

  if (loading) return (
    <LayoutLogged>
      <div className="flex justify-center items-center h-screen">
        <LoadingSpinner/>
      </div>
    </LayoutLogged>
  );

  if (!isLogged) {
    return <Navigate to="/login" replace />;
  }

  if (!isAuthenticated && !isAdmin) {
    return <Navigate to="/sistema/vinculos" replace />;
  }
  
  if (!isAuthenticated && isAdmin) {
    return <Navigate to="/sistema" replace />;
  }

  return <Outlet />;

};

export default AuthenticatedRoute;
