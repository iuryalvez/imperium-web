// src/pages/AcompanhamentoAnimal/index.jsx
import React from "react";
import {
  Skeleton,
  Result,
  Button,
  Typography,
  Divider,
  Modal,
  Statistic,
  Col,
  Row,
  Card,
} from "antd";
import { Link } from "react-router-dom";
import { useAcompanhamentoAnimalManager } from "./useAcompanhamentoAnimalManager";
import CabecalhoAnimal from "./components/CabecalhoAnimal";
import ListaRelatorios from "./components/ListaRelatorios";
import dayjs from "dayjs";
import FormularioDieta from "./components/FormularioDieta";
import DetalhesDieta from "./components/DetalhesDieta";

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
      <Title level={2}>Acompanhamento de {historico.nome}</Title>

      <Card style={{ marginBottom: 24 }}>
        <Row gutter={16}>
          <Col span={8}>
            <Statistic
              title="Peso Atual"
              value={historico.pesoAtual}
              suffix="kg"
            />
          </Col>
          <Col span={8}>
            <Statistic
              title="Condição Corporal"
              value={historico.condicaoCorporal}
            />
          </Col>
          <Col span={8}>
            <Statistic
              title="Kcal Diárias (último cálculo)"
              value={
                dietaMaisRecente
                  ? dietaMaisRecente.necessidadeEnergeticaKcal
                  : "N/A"
              }
              precision={2}
            />
          </Col>
        </Row>
      </Card>

      <CabecalhoAnimal animal={historico} />

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
