import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../../services/AuthContext';

import '../../App.css';
import LayoutLogged from '../layout/LayoutLogged';
import LoadingSpinner from '../atoms/LoadingSpinner';

const AdminRoute = () => {
  const { isAdmin, loading, isLogged, isAuthenticated } = useAuth();

  if (loading) {
    return (
      <LayoutLogged>
        <div className="flex justify-center items-center h-screen">
          <LoadingSpinner />
        </div>
      </LayoutLogged>
    );
  }

  if (isLogged && isAdmin) return <Outlet />;

  if (!isLogged) {
    return <Navigate to="/login" replace />;
  }

  if (isLogged && !isAdmin) {
    return <Navigate to="/sistema/vinculos" replace />;
  }
  
  if (isAuthenticated) {
    return <Navigate to="/clinica/inicio" replace />;
  }

  return <Outlet />
}

export default AdminRoute;
