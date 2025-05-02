import React from 'react';
import LoggedHeader from './Header/LoggedHeader.jsx';
import { Outlet } from 'react-router-dom';

const LayoutLogged = () => {
  return (
    <div className="flex flex-col min-h-screen">
      <LoggedHeader />
      <main>
        <Outlet></Outlet>
      </main>
    </div>
  );
};

export default LayoutLogged;