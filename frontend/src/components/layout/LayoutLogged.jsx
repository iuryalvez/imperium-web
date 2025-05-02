import React from 'react';
import LoggedHeader from './Header/LoggedHeader.jsx';
import { Outlet } from 'react-router-dom';

const LayoutLogged = () => {
  return (
    <div className="flex flex-col min-h-screen">
      <LoggedHeader />
      <main className="flex flex-col mt-8 px-4 md:px-8 lg:px-16 text-left w-full min-w-screen-lg mx-auto space-y-4">
        <Outlet></Outlet>
      </main>
    </div>
  );
};

export default LayoutLogged;