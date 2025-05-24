import React from "react";
import { Link } from "react-router-dom";
import logo from "../../../assets/logo-unificada.png";

const PublicHeader = () => {
  return (
    <header className="fixed top-0 left-0 w-full flex justify-between items-center px-8 py-2 border-b border-gray-300 bg-[var(--cinza-background)] shadow-md">
      <div className="flex items-center">
        <img src={logo} alt="Logo" className="h-12" />
      </div>
      <nav>
        <ul className="flex space-x-6">
          <li>
            <Link
              to="/sobre"
              className="text-[var(--verde-medio)] font-bold hover:text-[var(--verde-realce)]"
            >
              Sobre
            </Link>
          </li>
          <li className="text-[var(--verde-medio)]">|</li>
          <li>
            <Link
              to="/login"
              className="text-[var(--verde-medio)] font-bold hover:text-[var(--verde-realce)]"
            >
              Login
            </Link>
          </li>
        </ul>
      </nav>
    </header>
  );
};

export default PublicHeader;