// src/pages/AcompanhamentoAnimal/index.jsx
import React from "react";
import {
  Skeleton,
  Result,
  Button,
  Typography,
  Divider,
  Modal,
} from "antd";
import { Link } from "react-router-dom";
import { useAcompanhamentoAnimalManager } from "./useAcompanhamentoAnimalManager";
import ListaRelatorios from "./components/ListaRelatorios";
import dayjs from "dayjs";
import FormularioDieta from "./components/FormularioDieta";
import DetalhesDieta from "./components/DetalhesDieta";
import FichaGeralAnimal from "./components/FichaGeralAnimal";

const { Title } = Typography;

const AcompanhamentoAnimal = () => {
  const {
    historico,
    loading,
    racoes,
    isModalCriarDietaOpen,
    isModalVisualizarDietaOpen,
    isModalEditarDietaOpen,
    relatorioSelecionado,
    dietaParaVisualizar,
    dietaParaEditar,
    isSubmitting,
    handleOpenModalCriarDieta,
    handleCloseModalCriarDieta,
    handleOpenModalVisualizarDieta,
    handleCloseModalVisualizarDieta,
    handleOpenModalEditarDieta,
    handleCloseModalEditarDieta,
    handleCriarDieta,
    handleEditarDieta,
    handleExcluirDieta,
  } = useAcompanhamentoAnimalManager();

  if (loading) {
    return <Skeleton active paragraph={{ rows: 8 }} />;
  }

  if (!historico) {
    return (
      <Result
        status="404"
        title="Histórico não encontrado"
        subTitle="Não foi possível carregar os dados do acompanhamento."
        extra={
          <Button type="primary">
            <Link to="/clinica/animais">Voltar para a Lista</Link>
          </Button>
        }
      />
    );
  }

  if (loading && !historico) {
    // Mostra Skeleton apenas no load inicial
    return <Skeleton active paragraph={{ rows: 8 }} />;
  }

  const dietaMaisRecente = historico.acompanhamento.relatorios
    .filter((rel) => rel.dieta) // Filtra apenas relatórios com dieta
    .sort((a, b) => new Date(b.dataConsulta) - new Date(a.dataConsulta))[0] // Ordena por data e pega o mais recente
    ?.dieta;

  return (
    <div>
      <Title level={2}>Ficha Geral do Animal</Title>

      <FichaGeralAnimal animal={historico} dieta={dietaMaisRecente} />

      <Divider orientation="left">Histórico de Relatórios</Divider>

      <ListaRelatorios
        relatorios={historico.acompanhamento.relatorios}
        onOpenModalCriarDieta={handleOpenModalCriarDieta}
        onOpenModalVisualizarDieta={handleOpenModalVisualizarDieta}
        onOpenModalEditarDieta={handleOpenModalEditarDieta}
        onExcluirDieta={handleExcluirDieta}
      />

      <Modal
        title={`Calcular Dieta para Consulta de ${dayjs(
          relatorioSelecionado?.dataConsulta
        ).format("DD/MM/YYYY")}`}
        open={isModalCriarDietaOpen}
        onCancel={handleCloseModalCriarDieta}
        footer={null} // O formulário terá seus próprios botões
        destroyOnClose
        width="min(700px, 95vw)"
      >
        {/* Renderiza o formulário apenas quando o modal estiver aberto */}
        {relatorioSelecionado && (
          <FormularioDieta
            onFinish={handleCriarDieta}
            onCancel={handleCloseModalCriarDieta}
            relatorioId={relatorioSelecionado.id}
            racoes={racoes}
            isSubmitting={isSubmitting}
          />
        )}
      </Modal>

      <Modal
        title="Detalhes da Dieta Calculada"
        open={isModalVisualizarDietaOpen}
        onCancel={handleCloseModalVisualizarDieta}
        footer={[
          <Button key="close" onClick={handleCloseModalVisualizarDieta}>
            Fechar
          </Button>,
        ]}
        destroyOnClose
        width="min(600px, 95vw)"
      >
        {/* Renderiza o componente de detalhes com a dieta selecionada */}
        {dietaParaVisualizar && <DetalhesDieta dieta={dietaParaVisualizar} />}
      </Modal>

      <Modal
        title={`Editar Dieta da Consulta de ${dayjs(
          dietaParaEditar?.dataConsulta
        ).format("DD/MM/YYYY")}`}
        open={isModalEditarDietaOpen}
        onCancel={handleCloseModalEditarDieta}
        footer={null}
        destroyOnClose
        width="min(700px, 90vw)"
      >
        {dietaParaEditar && (
          <FormularioDieta
            onFinish={handleEditarDieta}
            onCancel={handleCloseModalEditarDieta}
            // Passamos o initialValues para preencher o formulário
            initialValues={dietaParaEditar}
            relatorioId={dietaParaEditar.relatorioId}
            racoes={racoes}
            isSubmitting={isSubmitting}
          />
        )}
      </Modal>
    </div>
  );
};

export default AcompanhamentoAnimal;
