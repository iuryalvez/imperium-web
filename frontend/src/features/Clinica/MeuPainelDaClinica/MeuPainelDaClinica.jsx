import React from "react";
import { Button, Card, Col, Descriptions, Row, Statistic, Tag } from "antd";
import { DashboardOutlined, UserOutlined } from "@ant-design/icons";
import LoadingSpinner from "../../../components/atoms/LoadingSpinner.jsx";
import { renderPerfis } from "../../../utils/renderers";
import { formatarTelefone } from "../../../utils/formatters";
import { useMeuPainelDaClinicaManager } from "./useMeuPainelDaClinicaManager";
import TabelaConsultasDoPainel from "./components/TabelaConsultasDoPainel";
import PaginaPadrao from "../../../components/layout/PaginaPadrao";

const MeuPainelDaClinica = () => {
  const {
    painel,
    loading,
    ehSecretario,
    ehVeterinario,
    podeVerDashboard,
    consultasMarcadas,
    consultasAtendidas,
    consultasPendentes,
    handleIrParaAcompanhamento,
    handleIrParaDashboard,
    handleIrParaMeuPerfil,
  } = useMeuPainelDaClinicaManager();

  if (loading) return <LoadingSpinner />;
  if (!painel) return null;

  const resumo = painel.consultasMarcadas;
  const vet = painel.veterinario;

  return (
    <PaginaPadrao
      preencher={false}
      titulo="Meu Painel" // a clínica já aparece no header
      acoes={
        <>
          {/* Mesmo destino do "Meu perfil" do menu do usuário */}
          <Button icon={<UserOutlined />} onClick={handleIrParaMeuPerfil}>
            Meu perfil
          </Button>
          {podeVerDashboard && (
            <Button type="primary" icon={<DashboardOutlined />} onClick={handleIrParaDashboard}>
              Dashboard da clínica
            </Button>
          )}
        </>
      }
    >
      {/* Meus dados: todos os perfis */}
      <Card title="Meus dados" className="shadow-sm">
        <Descriptions column={{ xs: 1, md: 2 }} size="small">
          <Descriptions.Item label="Nome">{painel.nome}</Descriptions.Item>
          <Descriptions.Item label="E-mail">{painel.email || "-"}</Descriptions.Item>
          <Descriptions.Item label="Formação">{painel.formacao || "-"}</Descriptions.Item>
          <Descriptions.Item label="Telefone">{painel.telefone ? formatarTelefone(painel.telefone) : "-"}</Descriptions.Item>
          <Descriptions.Item label="Perfis nesta clínica">{renderPerfis(painel.perfis)}</Descriptions.Item>
        </Descriptions>
      </Card>

      {/* SECRETARIO: consultas que marquei */}
      {ehSecretario && resumo && (
        <Card title="Consultas que marquei" className="shadow-sm">
          <Row gutter={[16, 16]} className="mb-4">
            <Col xs={12} md={4}><Statistic title="Total" value={resumo.total} /></Col>
            <Col xs={12} md={5}><Statistic title={<Tag color="gold">Pendentes</Tag>} value={resumo.pendentes} /></Col>
            <Col xs={12} md={5}><Statistic title={<Tag color="blue">Em andamento</Tag>} value={resumo.emAndamento} /></Col>
            <Col xs={12} md={5}><Statistic title={<Tag color="green">Concluídas</Tag>} value={resumo.concluidas} /></Col>
            <Col xs={12} md={5}><Statistic title={<Tag color="red">Canceladas</Tag>} value={resumo.canceladas} /></Col>
          </Row>
          <TabelaConsultasDoPainel lista={consultasMarcadas} onIrParaAcompanhamento={handleIrParaAcompanhamento} />
        </Card>
      )}

      {/* VETERINARIO: o que atendi e o que está pendente na clínica */}
      {ehVeterinario && vet && (
        <>
          <Card title="Meus atendimentos" className="shadow-sm">
            <Row gutter={[16, 16]} className="mb-4">
              <Col xs={12} md={8}><Statistic title="Avaliações preenchidas" value={vet.avaliacoesPreenchidas} /></Col>
              <Col xs={12} md={8}><Statistic title="Com dieta (concluídas)" value={vet.consultasConcluidas} /></Col>
            </Row>
            <TabelaConsultasDoPainel lista={consultasAtendidas} onIrParaAcompanhamento={handleIrParaAcompanhamento} />
          </Card>
          <Card title={`Consultas pendentes da clínica (${vet.consultasPendentesNaClinica})`} className="shadow-sm">
            <TabelaConsultasDoPainel lista={consultasPendentes} onIrParaAcompanhamento={handleIrParaAcompanhamento} />
          </Card>
        </>
      )}
    </PaginaPadrao>
  );
};

export default MeuPainelDaClinica;
