import React from "react";
import { Button, Avatar } from "antd";
import { EditOutlined, HomeOutlined, MedicineBoxOutlined } from "@ant-design/icons";

// Importando os ícones em PNG dos assets
// import UsuariosIcon from "../../../assets/";
import TutoresIcon from "../../../assets/client/client-profile-svgrepo-com.png";
import AnimaisIcon from "../../../assets/animal/pet-14-svgrepo-com.png";
import ConsultasIcon from "../../../assets/document/consultation-consulting-laptop-doctor-healthy-medical-care-svgrepo-com.png";
import UsuariosIcon from "../../../assets/user/user-group-svgrepo-com.png";

import LoadingSpinner from "../../../components/atoms/LoadingSpinner.jsx";
import { usePerfilClinicaManager } from "./usePerfilClinicaManager.js";
import FormularioEdicaoClinica from "./components/FormularioEdicaoClinica.jsx";
import ModalPadrao from "../../../components/modal/ModalPadrao.jsx";
import CardAtalho from "../../../components/cards/CardAtalho.jsx";

// "120 ativos (8 inativos)": o backend envia os ativos em total* e os inativos à parte
const formatarAtivosInativos = (ativos, inativos) =>
  `${ativos || 0} ativos${inativos ? ` (${inativos} inativos)` : ""}`;

const PerfilClinica = () => {
  const {
    dadosClinica,
    totalRacas,
    loading,
    isSubmitting,
    isModalOpen,
    handleOpenModal,
    handleCloseModal,
    handleSubmit,
    handleIrParaMeuPainel,
    handleIrPara,
  } = usePerfilClinicaManager();

  if (loading) {
    return <LoadingSpinner />;
  }

  // Visão Geral: cada card leva à tela da entidade (Raças ficam no modal da tela de Animais)
  const cards = [
    {
      titulo: "Usuários vinculados",
      valor: dadosClinica?.totalUsuarios || 0,
      icone: UsuariosIcon,
      rota: "/clinica/usuarios",
      rotulo: "Ver usuários",
    },
    {
      titulo: "Tutores",
      valor: formatarAtivosInativos(dadosClinica?.totalTutores, dadosClinica?.totalTutoresInativos),
      icone: TutoresIcon,
      rota: "/clinica/tutores",
      rotulo: "Ver tutores",
    },
    {
      titulo: "Animais",
      valor: formatarAtivosInativos(dadosClinica?.totalAnimais, dadosClinica?.totalAnimaisInativos),
      detalhe: totalRacas && `Raças: ${totalRacas.caes} de cães · ${totalRacas.gatos} de gatos`,
      icone: AnimaisIcon,
      rota: "/clinica/animais",
      rotulo: "Ver animais",
    },
    {
      titulo: "Consultas realizadas",
      valor: dadosClinica?.totalConsultas || 0,
      icone: ConsultasIcon,
      rota: "/clinica/consultas",
      rotulo: "Ver consultas",
    },
  ];

  const nomeExibido = dadosClinica?.nomeFantasia || dadosClinica?.nomeClinica;

  return (
    <div className="w-full max-w-7xl mx-auto flex flex-col gap-6">
      {/* Cabeçalho: com nome longo, o nome quebra em linhas e os botões descem para baixo dele */}
      <div className="flex flex-wrap items-center justify-between gap-4 bg-white p-6 rounded-lg shadow">
        <div className="flex items-center gap-4 flex-1 min-w-[16rem]">
          <Avatar
            size={80}
            src={dadosClinica?.logoUrl}
            icon={!dadosClinica?.logoUrl && <MedicineBoxOutlined />}
            className="bg-[var(--verde-medio)] shrink-0"
          />
          <div className="min-w-0">
            <h1 className="text-2xl font-bold text-[var(--verde-medio)] m-0 break-words">
              {nomeExibido}
            </h1>
            <p className="text-gray-500 m-0">
              CNPJ:{" "}
              {dadosClinica?.cnpj?.replace(
                /^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$/,
                "$1.$2.$3/$4-$5",
              ) || "Não informado"}
            </p>
            <p className="text-xs text-gray-400 mt-1 mb-0">
              Cadastrada em:{" "}
              {dadosClinica?.dataCadastro
                ? new Date(dadosClinica.dataCadastro).toLocaleDateString(
                    "pt-BR",
                  )
                : "--/--/----"}
            </p>
          </div>
        </div>
        <div className="flex flex-wrap gap-2 shrink-0">
          {/* Volta ao Meu Painel, como o Meu Painel tem o botão "Dashboard da clínica" */}
          <Button icon={<HomeOutlined />} onClick={handleIrParaMeuPainel}>
            Meu painel
          </Button>
          <Button type="primary" icon={<EditOutlined />} onClick={handleOpenModal}>
            Editar Informações
          </Button>
        </div>
      </div>

      <section className="flex flex-col gap-4">
        <h2 className="text-xl font-semibold text-gray-700 m-0">
          Visão Geral da Clínica
        </h2>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          {cards.map((card) => (
            <CardAtalho
              key={card.rota}
              titulo={card.titulo}
              valor={card.valor}
              descricao={card.detalhe}
              icone={card.icone}
              rotulo={card.rotulo}
              onClick={() => handleIrPara(card.rota)}
            />
          ))}
        </div>
      </section>

      {/* MODAL PADRÃO: Edição do Perfil da Clínica */}
      <ModalPadrao
        modo="edicao"
        open={isModalOpen}
        onCancel={handleCloseModal}
        title="Editar Perfil da Clínica"
        icon={<EditOutlined />}
        formId="form-edicao-perfil-clinica"
        isSubmitting={isSubmitting}
      >
        <FormularioEdicaoClinica
          initialValues={dadosClinica}
          onFinish={handleSubmit}
        />
      </ModalPadrao>
    </div>
  );
};

export default PerfilClinica;
