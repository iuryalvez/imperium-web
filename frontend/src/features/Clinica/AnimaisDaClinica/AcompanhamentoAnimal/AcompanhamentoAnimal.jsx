import React from "react";
import { Skeleton, Result, Button, Divider } from "antd";
import { Link } from "react-router-dom";
import { FormOutlined, EditOutlined, ProfileOutlined } from "@ant-design/icons";
import dayjs from "dayjs";
import { useAcompanhamentoAnimalManager } from "./useAcompanhamentoAnimalManager";
import ListaRelatorios from "./components/ListaRelatorios";
import FormularioDieta from "./components/FormularioDieta";
import DetalhesDieta from "./components/DetalhesDieta";
import FichaGeralAnimal from "./components/FichaGeralAnimal";
import ModalPadrao from "../../../../components/modal/ModalPadrao";

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

  // Ordena os relatórios pela data para extrair o mais recente de forma segura
  const relatoriosOrdenados = [
    ...(historico.acompanhamento?.relatorios || []),
  ].sort((a, b) => new Date(b.dataConsulta) - new Date(a.dataConsulta));

  const relatorioMaisRecente = relatoriosOrdenados[0];
  const dietaMaisRecente = relatoriosOrdenados.find((rel) => rel.dieta)?.dieta;

  return (
    <div>
      {/* Título externo removido, a FichaGeralAnimal agora possui seu próprio título */}
      <FichaGeralAnimal
        animal={historico}
        relatorioRecente={relatorioMaisRecente}
        dietaRecente={dietaMaisRecente}
      />

      <Divider orientation="left">Histórico de Relatórios</Divider>

      <ListaRelatorios
        relatorios={historico.acompanhamento.relatorios}
        onOpenModalCriarDieta={handleOpenModalCriarDieta}
        onOpenModalVisualizarDieta={handleOpenModalVisualizarDieta}
        onOpenModalEditarDieta={handleOpenModalEditarDieta}
        onExcluirDieta={handleExcluirDieta}
      />

      {/* Modal Padrao: Criar Dieta */}
      <ModalPadrao
        modo="criacao"
        open={isModalCriarDietaOpen}
        onCancel={handleCloseModalCriarDieta}
        title={`Calcular Dieta (${dayjs(relatorioSelecionado?.dataConsulta).format("DD/MM/YYYY")})`}
        icon={<FormOutlined />}
        formId="form-dieta-animal"
        isSubmitting={isSubmitting}
        width="min(700px, 95vw)"
      >
        {relatorioSelecionado && (
          <FormularioDieta
            onFinish={handleCriarDieta}
            relatorioId={relatorioSelecionado.id}
            racoes={racoes}
          />
        )}
      </ModalPadrao>

      {/* Modal Padrao: Editar Dieta */}
      <ModalPadrao
        modo="edicao"
        open={isModalEditarDietaOpen}
        onCancel={handleCloseModalEditarDieta}
        title={`Editar Dieta (${dayjs(dietaParaEditar?.dataConsulta).format("DD/MM/YYYY")})`}
        icon={<EditOutlined />}
        formId="form-dieta-animal"
        isSubmitting={isSubmitting}
        width="min(700px, 95vw)"
      >
        {dietaParaEditar && (
          <FormularioDieta
            onFinish={handleEditarDieta}
            initialValues={dietaParaEditar}
            relatorioId={dietaParaEditar.relatorioId}
            racoes={racoes}
          />
        )}
      </ModalPadrao>

      {/* Modal Padrao: Visualizar Dieta (Detalhes) */}
      <ModalPadrao
        modo="detalhes"
        open={isModalVisualizarDietaOpen}
        onCancel={handleCloseModalVisualizarDieta}
        title="Detalhes da Dieta Calculada"
        icon={<ProfileOutlined />}
        width="min(600px, 95vw)"
      >
        {dietaParaVisualizar && <DetalhesDieta dieta={dietaParaVisualizar} />}
      </ModalPadrao>
    </div>
  );
};

export default AcompanhamentoAnimal;
