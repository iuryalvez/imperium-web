import React from 'react';
import { Navigate } from 'react-router-dom';
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

  if (isLogged ) {
    return isAdmin ? <Navigate to="/sistema" /> 
    : <Navigate to="/sistema/vinculos" />
  }

  if (!isLogged && !isAuthenticated) return <Navigate to="/login" />;

  if (isAuthenticated) return <Navigate to="/clinica/inicio" />;

};

export default LoggedRoute;