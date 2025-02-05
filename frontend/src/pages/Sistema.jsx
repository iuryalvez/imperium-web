import React from 'react';
import { useAuth } from '../services/AuthContext.js';   
import TemplateSistema from '../components/template/TemplateSistema';

const Sistema = () => {
  const { isAdmin } = useAuth();
  return <TemplateSistema isAdmin={isAdmin} />;
};

export default Sistema;
