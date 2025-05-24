import React from "react";
import { Link } from "react-router-dom";
import logo from "../../../assets/logo-unificada.png";
import { useAuth } from "../../../services/AuthContext";

const AuthenticatedHeader = () => {
  const { logout, trocarVinculo} = useAuth();
  return (
    <header className="fixed top-0 left-0 w-full flex justify-between items-center px-8 py-2 border-b border-gray-300 bg-[var(--cinza-background)] shadow-md z-50">
      <div className="flex items-center">
        <img src={logo} alt="Logo" className="h-12" />
      </div>
      <nav>
        <ul className="flex space-x-6">
          <li>
            <Link
              to="/clinica/perfil"
              className="text-[var(--verde-medio)] font-bold hover:text-[var(--verde-realce)]"
            >
              Perfil
            </Link>
          </li>
          <li className="text-[var(--verde-medio)]">|</li>
          <li>
            <button
              onClick={trocarVinculo}
              className="text-[var(--verde-medio)] font-bold hover:text-[var(--verde-realce)]"
            >
              Trocar vínculo
            </button>
          </li>
          <li className="text-[var(--verde-medio)]">|</li>
          <li>
            <button
              onClick={logout}
              className="text-[var(--verde-medio)] font-bold hover:text-[var(--verde-realce)]"
            >
              Sair
            </button>
          </li>
        </ul>
      </nav>
    </header>
  );
};

export default AuthenticatedHeader;