import React from "react";
import { Card, Statistic, Button, Avatar, Divider } from "antd";
import { EditOutlined, MedicineBoxOutlined } from "@ant-design/icons";

// Importando os ícones em PNG dos assets
// import UsuariosIcon from "../../../assets/";
import TutoresIcon from "../../../assets/client/client-profile-svgrepo-com.png";
import AnimaisIcon from "../../../assets/animal/pet-14-svgrepo-com.png";
import ConsultasIcon from "../../../assets/document/consultation-consulting-laptop-doctor-healthy-medical-care-svgrepo-com.png";
import UsuariosIcon from "../../../assets/user/user-group-svgrepo-com.png";
import AcompanhamentoIcon from "../../../assets/animal/pet-weight-weigh-a-pet-small-pet-svgrepo-com.png";

import LoadingSpinner from "../../../components/atoms/LoadingSpinner.jsx";
import { usePerfilClinicaManager } from "./usePerfilClinicaManager.js";
import FormularioEdicaoClinica from "./components/FormularioEdicaoClinica.jsx";
import ModalPadrao from "../../../components/modal/ModalPadrao.jsx";

// Componente auxiliar para renderizar a máscara CSS do ícone
const CustomIconMask = ({ src }) => (
  <div
    className="w-5 h-5 bg-[var(--verde-medio)]"
    style={{
      WebkitMask: `url(${src}) center/contain no-repeat`,
      mask: `url(${src}) center/contain no-repeat`,
    }}
  />
);

// "120 ativos (8 inativos)": o backend envia os ativos em total* e os inativos à parte
const formatarAtivosInativos = (ativos, inativos) =>
  `${ativos || 0} ativos${inativos ? ` (${inativos} inativos)` : ""}`;

const PerfilClinica = () => {
  const {
    dadosClinica,
    loading,
    isSubmitting,
    isModalOpen,
    handleOpenModal,
    handleCloseModal,
    handleSubmit,
  } = usePerfilClinicaManager();

  if (loading) {
    return <LoadingSpinner />;
  }

  return (
    <div className="max-w-7xl mx-auto">
      <div className="flex flex-col md:flex-row justify-between items-center bg-white p-6 rounded-lg shadow mb-6">
        <div className="flex items-center gap-4">
          <Avatar
            size={80}
            src={dadosClinica?.logoUrl}
            icon={!dadosClinica?.logoUrl && <MedicineBoxOutlined />}
            className="bg-[var(--verde-medio)]"
          />
          <div>
            <h1 className="text-2xl font-bold text-[var(--verde-medio)] m-0">
              {dadosClinica?.nomeFantasia || dadosClinica?.nomeClinica}
            </h1>
            <p className="text-gray-500 m-0">
              CNPJ:{" "}
              {dadosClinica?.cnpj?.replace(
                /^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$/,
                "$1.$2.$3/$4-$5",
              ) || "Não informado"}
            </p>
            <p className="text-xs text-gray-400 mt-1">
              Cadastrada em:{" "}
              {dadosClinica?.dataCadastro
                ? new Date(dadosClinica.dataCadastro).toLocaleDateString(
                    "pt-BR",
                  )
                : "--/--/----"}
            </p>
          </div>
        </div>
        <Button
          type="primary"
          icon={<EditOutlined />}
          onClick={handleOpenModal}
          className="mt-4 md:mt-0"
        >
          Editar Informações
        </Button>
      </div>

      <Divider className="my-2" />
      <h2 className="text-xl font-semibold text-gray-700 mb-4">
        Visão Geral da Clínica
      </h2>

      {/* Grid de Estatísticas com Ícones Customizados */}
      <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
        <Card className="shadow-sm hover:shadow-md transition-shadow">
          <Statistic
            title="Usuários Vinculados"
            value={dadosClinica?.totalUsuarios || 0}
            prefix={<CustomIconMask src={UsuariosIcon} />}
          />
        </Card>
        <Card className="shadow-sm hover:shadow-md transition-shadow">
          <Statistic
            title="Tutores Cadastrados"
            value={formatarAtivosInativos(dadosClinica?.totalTutores, dadosClinica?.totalTutoresInativos)}
            prefix={<CustomIconMask src={TutoresIcon} />}
          />
        </Card>
        <Card className="shadow-sm hover:shadow-md transition-shadow">
          <Statistic
            title="Animais"
            value={formatarAtivosInativos(dadosClinica?.totalAnimais, dadosClinica?.totalAnimaisInativos)}
            prefix={<CustomIconMask src={AnimaisIcon} />}
          />
        </Card>
        <Card className="shadow-sm hover:shadow-md transition-shadow">
          <Statistic
            title="Consultas Realizadas"
            value={dadosClinica?.totalConsultas || 0}
            prefix={<CustomIconMask src={ConsultasIcon} />}
          />
        </Card>
        <Card className="shadow-sm hover:shadow-md transition-shadow">
          <Statistic
            title="Acompanhamentos"
            value={dadosClinica?.totalAcompanhamentos || 0}
            prefix={
              <CustomIconMask src={AcompanhamentoIcon || ConsultasIcon} />
            }
          />
        </Card>
      </div>

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
