import React from "react";
import { useAuth } from "../../services/AuthContext";
import BotaoSecundario from "../atoms/BotaoSecundario";

const CardInfo = ({ tipoEntidade, entidade }) => {
  const { escolherVinculo } = useAuth();

  if (!entidade) {
    return <div className="text-red-500">Erro: Dados não encontrados.</div>;
  }

  const handleRedirect = async () => {
    if (tipoEntidade === "Vínculo") {
      try {
        await escolherVinculo(entidade.id);
      } catch (error) {
        console.error("Erro ao escolher vínculo:", error);
        alert("Não foi possível selecionar o vínculo. Tente novamente.");
      }
    } else {
        alert("Tipo de entidade não reconhecido.");
    }
  };

  return (
    <div className="flex items-center justify-between p-4 mb-4 bg-white shadow-lg rounded-lg w-full">
      <div className="flex flex-col space-y-2">
        <h3 className="font-bold text-lg text-[var(--verde-medio)]">
          {tipoEntidade} - {entidade.nomeClinica}
        </h3>

        <div className="text-[var(--verde-medio)]">
          Clínica: {entidade.nomeClinica || "N/A"}
        </div>
        <div className="text-[var(--verde-medio)]">
          Tipo de Usuário: {entidade.nomePerfil}
        </div>
      </div>

      <BotaoSecundario
        texto={tipoEntidade === "Vínculo" ? "Selecionar" : "Acessar"}
        onClick={handleRedirect}
      />
    </div>
  );
};

export default CardInfo;
