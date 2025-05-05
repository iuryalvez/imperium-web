import React from "react";
import logo from "../../../assets/logo-unificada.png";

const LoadingHeader = () => {
  return (
    <header className="fixed top-0 left-0 w-full flex justify-between items-center px-8 py-2 border-b border-gray-300 bg-[var(--cinza-background)] shadow-md z-50">
      <div className="flex items-center">
        <img src={logo} alt="Logo" className="h-12" />
      </div>
    </header>
  );
};

export default LoadingHeader;