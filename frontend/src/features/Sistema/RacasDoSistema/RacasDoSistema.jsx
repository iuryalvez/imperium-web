import React from "react";
import { Button, Input } from "antd";
import {
  PlusOutlined,
  EditOutlined,
  FormOutlined,
  ShopOutlined,
  GlobalOutlined,
} from "@ant-design/icons";
import { useRacasDoSistemaManager } from "./useRacasDoSistemaManager";
import TabelaRacasDoSistema from "./components/TabelaRacasDoSistema/TabelaRacasDoSistema";
// Formulários de raça são os mesmos da clínica (gerenciador de raças)
import PaginaPadrao from "../../../components/layout/PaginaPadrao";
import ModalPadrao from "../../../components/modal/ModalPadrao";
import FormCadastroRaca from "../../Clinica/AnimaisDaClinica/components/GerenciadorDeRacas/FormCadastroRaca";
import FormEdicaoRaca from "../../Clinica/AnimaisDaClinica/components/GerenciadorDeRacas/FormEdicaoRaca";

// Breadcrumb: Início do admin > esta tela
const TRILHA = [{ titulo: "Início", para: "/sistema" }];

const RacasDoSistema = () => {
  const {
    visao,
    racas,
    loading,
    paginacao,
    loadingRowKey,
    isModalOpen,
    isSubmitting,
    editingRaca,
    setBusca,
    handleTableChange,
    handleAlternarVisao,
    handleOpenModalCadastro,
    handleEditarRaca,
    handleCloseModal,
    handleSubmit,
    handleDelete,
    handleToggleStatus,
  } = useRacasDoSistemaManager();

  const isCopias = visao === "copias";

  return (
    <>
      <PaginaPadrao
        titulo={isCopias ? "Raças das Clínicas" : "Raças do Sistema"}
        trilha={TRILHA}
        acoes={
          <>
            <Button
              icon={isCopias ? <GlobalOutlined /> : <ShopOutlined />}
              onClick={handleAlternarVisao}
            >
              {isCopias ? "Ver raças do sistema" : "Cópias das clínicas"}
            </Button>
            {!isCopias && (
              <Button type="primary" icon={<PlusOutlined />} onClick={handleOpenModalCadastro}>
                Cadastrar Raça
              </Button>
            )}
          </>
        }
      >
        <TabelaRacasDoSistema
          key={visao}
          barra={
            <Input.Search
              placeholder={isCopias ? "Buscar por raça ou clínica..." : "Buscar por nome..."}
              onChange={(e) => setBusca(e.target.value)}
              allowClear
              className="w-full sm:w-80"
            />
          }
          racas={racas}
          loading={loading}
          paginacao={paginacao}
          onChange={handleTableChange}
          visao={visao}
          onEdit={handleEditarRaca}
          onDelete={handleDelete}
          onToggleStatus={handleToggleStatus}
          loadingRowKey={loadingRowKey}
        />
      </PaginaPadrao>

      {/* Os formulários de raça já trazem os próprios botões, por isso o modal sem rodapé */}
      <ModalPadrao
        title={editingRaca ? "Editar Raça" : "Cadastrar Raça do Sistema"}
        icon={editingRaca ? <EditOutlined /> : <FormOutlined />}
        open={isModalOpen}
        onCancel={handleCloseModal}
        tamanho="sm"
        rodape={null}
      >
        {editingRaca ? (
          <FormEdicaoRaca
            initialValues={editingRaca}
            onFinish={handleSubmit}
            onCancel={handleCloseModal}
            isSubmitting={isSubmitting}
          />
        ) : (
          <FormCadastroRaca
            onFinish={handleSubmit}
            onCancel={handleCloseModal}
            isSubmitting={isSubmitting}
          />
        )}
      </ModalPadrao>
    </>
  );
};

export default RacasDoSistema;
