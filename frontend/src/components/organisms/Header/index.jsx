import React from "react";
import { Link } from "react-router-dom";
import logo from "../../../assets/logo-unificada.png"; // Mantendo o logo atual
import { useAuth } from "../../../services/AuthContext"; // Contexto de autenticação (simulado aqui)

const Header = () => {
  const { isLogged, isAuthenticated, logout, trocarVinculo } = useAuth(); // isAuthenticated indica se o usuário está logado

  return (
    <header className="fixed top-0 left-0 w-full flex justify-between items-center px-8 py-2 border-b border-gray-300 bg-[var(--cinza-background)] shadow-md z-50">
      <div className="flex items-center">
        <img src={logo} alt="Logo" className="h-12" />
      </div>
      <nav>
        <ul className="flex space-x-6">
          {isAuthenticated ? (
            <>
              {/* <li>
                            <Link to="/clinica/home" className="text-green-dark font-bold hover:text-green-light">Início</Link>
                        </li>
                        <li className="text-green-dark">|</li> */}
              <li>
                <Link
                  to="/clinica/perfil"
                  className="text-green-dark font-bold hover:text-green-light"
                >
                  Perfil
                </Link>
              </li>
              <li className="text-green-dark">|</li>
              <li>
                <button
                  onClick={trocarVinculo}
                  className="text-green-dark font-bold hover:text-green-light"
                >
                  Trocar vínculo
                </button>
              </li>
              <li className="text-green-dark">|</li>
              <li>
                <button
                  onClick={logout}
                  className="text-green-dark font-bold hover:text-green-light"
                >
                  Sair
                </button>
              </li>
            </>
          ) : isLogged ? (
            <>
              <li>
                <button
                  onClick={logout}
                  className="text-green-dark font-bold hover:text-green-light"
                >
                  Sair
                </button>
              </li>
            </>
          ) : (
            <>
              <li>
                <Link
                  to="/sobre"
                  className="text-green-dark font-bold hover:text-green-light"
                >
                  Sobre
                </Link>
              </li>
              <li className="text-green-dark">|</li>
              {/* <li>
                            <Link to="/cadastro" className="text-green-dark font-bold hover:text-green-light">Cadastro</Link>
                        </li> */}
              {/* <li className="text-green-dark">|</li> */}
              <li>
                <Link
                  to="/login"
                  className="text-green-dark font-bold hover:text-green-light"
                >
                  Login
                </Link>
              </li>
            </>
          )}
        </ul>
      </nav>
    </header>
  );
};

export default Header;
