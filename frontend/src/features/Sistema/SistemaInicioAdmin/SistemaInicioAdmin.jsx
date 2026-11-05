import React from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../../services/AuthContext.js";
import PaginaPadrao from "../../../components/layout/PaginaPadrao.jsx";
import CardAtalho from "../../../components/cards/CardAtalho.jsx";

// Mesmos ícones da sidebar da clínica
import ClinicasIcon from "../../../assets/animal/pet-care-service-pet-grooming-service-svgrepo-com.png";
import UsuariosIcon from "../../../assets/user/user-group-svgrepo-com.png";
import RacasIcon from "../../../assets/animal/pet-14-svgrepo-com.png";
import RacoesIcon from "../../../assets/animal/dog-food-pet-svgrepo-com.png";

const ATALHOS = [
  {
    titulo: "Clínicas",
    descricao: "Cadastre clínicas, edite os dados e simule o acesso como um perfil da clínica.",
    rota: "/sistema/clinicas",
    icone: ClinicasIcon,
  },
  {
    titulo: "Usuários",
    descricao: "Gerencie os usuários do sistema, seus perfis e vínculos com as clínicas.",
    rota: "/sistema/usuarios",
    icone: UsuariosIcon,
  },
  {
    titulo: "Raças",
    descricao: "Mantenha as raças padrão de cães e gatos e veja as cópias feitas pelas clínicas.",
    rota: "/sistema/racas",
    icone: RacasIcon,
  },
  {
    titulo: "Rações",
    descricao: "Mantenha as rações padrão e veja as rações cadastradas pelas clínicas.",
    rota: "/sistema/racoes",
    icone: RacoesIcon,
  },
];

/** Início do admin do sistema: atalhos para as telas de administração. */
const SistemaInicio = () => {
  const { userName } = useAuth();
  const navigate = useNavigate();

  return (
    <PaginaPadrao
      preencher={false}
      titulo={`Bem-vindo, ${userName || "Usuário"}!`}
      descricao="Administração do Imperium: escolha o que deseja gerenciar."
    >
      <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-4">
        {ATALHOS.map((atalho) => (
          <CardAtalho
            key={atalho.rota}
            titulo={atalho.titulo}
            descricao={atalho.descricao}
            icone={atalho.icone}
            rotulo={`Acessar ${atalho.titulo.toLowerCase()}`}
            onClick={() => navigate(atalho.rota)}
          />
        ))}
      </div>
    </PaginaPadrao>
  );
};

export default SistemaInicio;
