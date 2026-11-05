import React from "react";
import { Button, Input } from "antd";
import {
  PlusOutlined,
  EditOutlined,
  FormOutlined,
  ProfileOutlined,
  UnorderedListOutlined,
} from "@ant-design/icons";
import { useAnimaisDaClinicaManager } from "./useAnimaisDaClinicaManager";
import { usePermissao } from "../../../utils/usePermissao";
import FormCadastroAnimal from "./components/FormularioCadastroAnimaisDaClinica/FormCadastroAnimais";
import FormEdicaoAnimal from "./components/FormularioEdicaoAnimaisDaClinica/FormEdicaoAnimais";
import GerenciadorDeRacas from "./components/GerenciadorDeRacas/GerenciadorDeRacas";
import TabelaAnimais from "./components/TabelaAnimaisDaClinica/TabelaAnimaisDaClinica";
import FormDetalhesAnimal from "./components/FormularioDetalhesAnimaisDaClinica/FormDetalhesAnimais";
import ModalPadrao from "../../../components/modal/ModalPadrao";
import PaginaPadrao from "../../../components/layout/PaginaPadrao";

const AnimaisDaClinica = () => {
  const {
    animais,
    loading,
    loadingRowKey,
    // searchTerm,
    isSubmitting,
    isModalAnimalOpen,
    isModalRacasOpen,
    isModalDetalhesOpen,
    editingAnimal,
    viewingAnimal,
    // handleSearch,
    handleOpenModal,
    handleCloseModal,
    handleOpenModalRacas,
    handleCloseModalRacas,
    handleNavigateToAcompanhamento,
    handleSubmit,
    handleDelete,
    handleToggleStatus,
    handleOpenModalDetalhes,
    handleCloseModalDetalhes,
    paginacao,
    setBusca,
    handleTableChange,
  } = useAnimaisDaClinicaManager();

  // Cadastrar/editar/excluir animais: GESTOR, VETERINARIO e TECNICO (o SECRETARIO só visualiza)
  const pode = usePermissao();
  const podeEditar = pode("gerenciaAnimais");


  return (
    <>
      <PaginaPadrao
        titulo="Gerenciamento de Animais"
        acoes={
          <>
            <Button onClick={handleOpenModalRacas}>Gerenciar Raças</Button>
            {podeEditar && (
              <Button
                type="primary"
                icon={<PlusOutlined />}
                onClick={() => handleOpenModal()}
              >
                Cadastrar Animal
              </Button>
            )}
          </>
        }
      >
        <TabelaAnimais
          barra={
            // Busca no servidor (com debounce); ordenação e filtros ficam nas colunas da tabela
            <Input.Search
              placeholder="Buscar por animal ou tutor..."
              onChange={(e) => setBusca(e.target.value)}
              allowClear
              className="w-full sm:w-80"
            />
          }
          loading={loading}
          paginacao={paginacao}
          onChange={handleTableChange}
          data={animais}
          onEdit={handleOpenModal}
          onDelete={handleDelete}
          onToggleStatus={handleToggleStatus}
          podeEditar={podeEditar}
          onNavigateToAcompanhamento={handleNavigateToAcompanhamento}
          onViewDetails={handleOpenModalDetalhes}
          loadingRowKey={loadingRowKey}
        />
      </PaginaPadrao>

      {/* Modal de Criação / Edição de Animal */}
      <ModalPadrao
        modo={editingAnimal ? "edicao" : "criacao"}
        open={isModalAnimalOpen}
        onCancel={handleCloseModal}
        title={editingAnimal ? "Editar Animal" : "Cadastrar Novo Animal"}
        icon={editingAnimal ? <EditOutlined /> : <FormOutlined />}
        formId={
          editingAnimal
            ? "form-edicao-animal-clinica"
            : "form-cadastro-animal-clinica"
        }
        isSubmitting={isSubmitting}
      >
        {editingAnimal ? (
          <FormEdicaoAnimal
            initialValues={editingAnimal}
            onFinish={handleSubmit}
          />
        ) : (
          <FormCadastroAnimal onFinish={handleSubmit} />
        )}
      </ModalPadrao>

      {/* Modal de Gerenciamento de Raças */}
      <ModalPadrao
        modo="detalhes"
        open={isModalRacasOpen}
        onCancel={handleCloseModalRacas}
        title="Gerenciamento de Raças"
        icon={<UnorderedListOutlined />}
        tamanho="lg"
      >
        <GerenciadorDeRacas />
      </ModalPadrao>

      {/* Modal de Detalhes do Animal */}
      <ModalPadrao
        modo="detalhes"
        open={isModalDetalhesOpen}
        onCancel={handleCloseModalDetalhes}
        title={`Detalhes de ${viewingAnimal?.nome || "Animal"}`}
        icon={<ProfileOutlined />}
      >
        {viewingAnimal && <FormDetalhesAnimal valores={viewingAnimal} />}
      </ModalPadrao>
    </>
  );
};

export default AnimaisDaClinica;
