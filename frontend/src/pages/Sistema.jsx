import React from 'react';
import { useAuth } from '../services/AuthContext.js';   
import TemplateHome from '../components/template/TemplateSistema';

const Sistema = () => {
  const { userType } = useAuth();
  return <TemplateHome userType={userType} />;
};

export default Sistema;
