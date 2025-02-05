import React from 'react';
import { useAuth } from '../services/AuthContext.js';   
import TemplateHome from '../components/template/TemplateSistema';

const Sistema = () => {
  const { isAdmin } = useAuth();
  return <TemplateHome isAdmin={isAdmin} />;
};

export default Sistema;
