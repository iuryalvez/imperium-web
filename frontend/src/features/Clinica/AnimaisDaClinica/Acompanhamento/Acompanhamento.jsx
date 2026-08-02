import React from "react";
import { Skeleton, Result, Button, Divider } from "antd";
import { Link } from "react-router-dom";
import { FormOutlined, EditOutlined, ProfileOutlined } from "@ant-design/icons";
import dayjs from "dayjs";
import { useAcompanhamentoManager } from "./useAcompanhamentoManager";
import ListaAvaliacoes from "./components/ListaAvaliacoes";
import FormularioDieta from "./components/FormularioDieta";
import DetalhesDieta from "./components/DetalhesDieta";
import FichaGeralAnimal from "./components/FichaGeralAnimal";
import ModalPadrao from "../../../../components/modal/ModalPadrao";

const Acompanhamento = () => {
  const {
    historico,
    loading,
    racoes,
    isModalCriarDietaOpen,
    isModalVisualizarDietaOpen,
    isModalEditarDietaOpen,
    avaliacaoSelecionado,
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
  } = useAcompanhamentoManager();

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
  const avaliacoesOrdenados = [
    ...(historico.acompanhamento?.avaliacoes || []),
  ].sort((a, b) => new Date(b.dataConsulta) - new Date(a.dataConsulta));

  const avaliacaoMaisRecente = avaliacoesOrdenados[0];
  const dietaMaisRecente = avaliacoesOrdenados.find((rel) => rel.dieta)?.dieta;

  return (
    <div>
      {/* Título externo removido, a FichaGeralAnimal agora possui seu próprio título */}
      <FichaGeralAnimal
        animal={historico}
        avaliacaoRecente={avaliacaoMaisRecente}
        dietaRecente={dietaMaisRecente}
      />

      <Divider orientation="left">Histórico</Divider>

      <ListaAvaliacoes
        avaliacoes={historico.acompanhamento.avaliacoes}
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
        title={`Dieta - Paciente: ${historico?.nome || "Desconhecido"} (${dayjs(avaliacaoSelecionado?.dataConsulta).format("DD/MM/YYYY")})`}
        icon={<FormOutlined />}
        formId="form-dieta-animal"
        isSubmitting={isSubmitting}
        width="min(700px, 95vw)"
      >
        {avaliacaoSelecionado && (
          <FormularioDieta
            onFinish={handleCriarDieta}
            avaliacaoId={avaliacaoSelecionado.id}
            racoes={racoes}
          />
        )}
      </ModalPadrao>

      {/* Modal Padrao: Editar Dieta */}
      <ModalPadrao
        modo="edicao"
        open={isModalEditarDietaOpen}
        onCancel={handleCloseModalEditarDieta}
        title={`Editar Dieta - Paciente: ${historico?.nome || "Desconhecido"} (${dayjs(dietaParaEditar?.dataConsulta).format("DD/MM/YYYY")})`}
        icon={<EditOutlined />}
        formId="form-dieta-animal"
        isSubmitting={isSubmitting}
        width="min(700px, 95vw)"
      >
        {dietaParaEditar && (
          <FormularioDieta
            onFinish={handleEditarDieta}
            initialValues={dietaParaEditar}
            avaliacaoId={dietaParaEditar.avaliacaoId}
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

export default Acompanhamento;
