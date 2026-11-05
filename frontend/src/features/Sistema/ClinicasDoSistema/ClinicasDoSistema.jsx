import React from "react";
import { Button, Input } from "antd";
import {
  PlusOutlined,
  DesktopOutlined,
  EditOutlined,
  FormOutlined,
  ProfileOutlined,
} from "@ant-design/icons";
import { useClinicasDoSistemaManager } from "./useClinicasDoSistemaManager";
import TabelaClinicasDoSistema from "./components/TabelaClinicasDoSistema/TabelaClinicasDoSistema.jsx";
import FormularioEdicaoClinicaDoSistema from "./components/FormularioEdicaoClinicaDoSistema/FormEdicaoClinicaDoSistema.jsx";
import FormularioCadastroClinicaDoSistema from "./components/FormularioCadastroClinicaDoSistema/FormCadastroClinicaDoSistema.jsx";
import FormularioDetalhesClinicaDoSistema from "./components/FormularioDetalhesClinicaDoSistema/FormDetalhesClinicaDoSistema.jsx";
import FormularioAcessoClinicaDoSistema from "./components/FormularioAcessoClinicaDoSistema/FormAcessoClinicaDoSistema.jsx";
import PaginaPadrao from "../../../components/layout/PaginaPadrao.jsx";
import ModalPadrao from "../../../components/modal/ModalPadrao.jsx";

// Breadcrumb: Início do admin > esta tela
const TRILHA = [{ titulo: "Início", para: "/sistema" }];

const ClinicasDoSistema = () => {
  const {
    clinicas,
    loading,
    loadingRowKey,
    isSubmitting,
    isModalOpen,
    isModalDetalhesOpen,
    isModalAcessoOpen,
    editingClinica,
    viewingClinica,
    accessingClinica,
    handleOpenModalCadastro,
    handleEditarClinica,
    handleOpenModalDetalhes,
    handleOpenModalAcesso,
    handleCloseModals,
    handleSubmit,
    handleSimularAcesso,
    handleDeleteClinica,
    paginacao,
    setBusca,
    handleTableChange,
  } = useClinicasDoSistemaManager();


  return (
    <>
      <PaginaPadrao
        titulo="Gerenciamento de Clínicas do Sistema"
        trilha={TRILHA}
        acoes={
          <Button type="primary" icon={<PlusOutlined />} onClick={handleOpenModalCadastro}>
            Cadastrar Clínica
          </Button>
        }
      >
        <TabelaClinicasDoSistema
          barra={
            // Busca no servidor (com debounce); ordenação e filtros ficam nas colunas da tabela
            <Input.Search
              placeholder="Buscar por nome ou CNPJ..."
              onChange={(e) => setBusca(e.target.value)}
              allowClear
              className="w-full sm:w-80"
            />
          }
          loading={loading}
          loadingRowKey={loadingRowKey}
          paginacao={paginacao}
          onChange={handleTableChange}
          clinicas={clinicas}
          onEdit={handleEditarClinica}
          onViewDetails={handleOpenModalDetalhes}
          onAccess={handleOpenModalAcesso}
          onDelete={handleDeleteClinica}
        />
      </PaginaPadrao>

      {/* MODAL PADRÃO: Cadastro / Edição */}
      <ModalPadrao
        modo={editingClinica ? "edicao" : "criacao"}
        open={isModalOpen}
        onCancel={handleCloseModals}
        title={editingClinica ? "Editar Clínica" : "Cadastrar Nova Clínica"}
        icon={editingClinica ? <EditOutlined /> : <FormOutlined />}
        formId={
          editingClinica
            ? "form-edicao-clinica-sistema"
            : "form-cadastro-clinica-sistema"
        }
        isSubmitting={isSubmitting}
      >
        {editingClinica ? (
          <FormularioEdicaoClinicaDoSistema
            initialValues={editingClinica}
            onFinish={handleSubmit}
          />
        ) : (
          <FormularioCadastroClinicaDoSistema onFinish={handleSubmit} />
        )}
      </ModalPadrao>

      {/* MODAL PADRÃO: Detalhes */}
      <ModalPadrao
        modo="detalhes"
        open={isModalDetalhesOpen}
        onCancel={handleCloseModals}
        title={`Detalhes de ${viewingClinica?.nomeClinica || "Clínica"}`}
        icon={<ProfileOutlined />}
      >
        {viewingClinica && (
          <FormularioDetalhesClinicaDoSistema valores={viewingClinica} />
        )}
      </ModalPadrao>

      {/* Acesso (simulação): rodapé próprio, com o botão que envia o formulário de perfis */}
      <ModalPadrao
        open={isModalAcessoOpen}
        onCancel={handleCloseModals}
        title={`Simular Acesso na clínica ${accessingClinica?.nomeClinica ?? ""}`}
        icon={<DesktopOutlined />}
        tamanho="sm"
        rodape={
          <>
            <Button onClick={handleCloseModals}>Cancelar</Button>
            <Button form="form-acesso-clinica" type="primary" htmlType="submit">
              Acessar {accessingClinica?.nomeClinica}
            </Button>
          </>
        }
      >
        {accessingClinica && (
          <FormularioAcessoClinicaDoSistema
            clinica={accessingClinica}
            onFinish={handleSimularAcesso}
          />
        )}
      </ModalPadrao>
    </>
  );
};

export default ClinicasDoSistema;
