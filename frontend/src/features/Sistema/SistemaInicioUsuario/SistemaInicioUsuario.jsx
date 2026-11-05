import React, { useEffect, useState } from "react";

import { Card, Typography, Spin, Alert, Row, Col, Button } from "antd";
import PaginaPadrao from "../../../components/layout/PaginaPadrao.jsx";
import { renderPerfis } from "../../../utils/renderers";
import { autenticacaoAPI } from "../../../api/autenticacaoAPI";
import { useAuth } from "../../../services/AuthContext";

import "./SistemaInicioUsuario.css";

const { Text } = Typography;

const SistemaInicioUsuario = () => {
  const { escolherVinculo, isLoading } = useAuth();
  const [vinculos, setVinculos] = useState([]);
  const [pageLoading, setPageLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchVinculos = async () => {
      try {
        const response = await autenticacaoAPI.listarVinculos();
        setVinculos(response);
      } catch (err) {
        setError("Erro ao carregar os vínculos.");
      } finally {
        setPageLoading(false);
      }
    };

    fetchVinculos();
  }, []);

  if (pageLoading) {
    return (
      <div className="flex justify-center items-center flex-1 py-16">
        <Spin size="large" />
      </div>
    );
  }

  if (error) {
    return (
      <PaginaPadrao preencher={false} titulo="Lista de Vínculos">
        <Alert message={error} type="error" showIcon />
      </PaginaPadrao>
    );
  }

  return (
    <PaginaPadrao
      preencher={false}
      titulo="Lista de Vínculos"
      descricao="Escolha a clínica em que deseja trabalhar."
    >

      <Row gutter={[24, 24]}>
        {vinculos.map((vinculo) => (
          <Col xs={24} sm={12} lg={8} key={vinculo.id}>
            <Card
              title={vinculo.nomeClinica || "Clínica Desconhecida"}
              bordered={false}
              className="vinculo-card"
              actions={[
                <Button
                  type="primary"
                  block
                  onClick={() => escolherVinculo(vinculo.id)}
                  loading={isLoading} // Usa a prop 'loading' para um UX melhor
                >
                  Selecionar
                </Button>,
              ]}
            >
              <Text strong className="block mb-2">Perfis</Text>
              {renderPerfis(vinculo.perfis)}
            </Card>
          </Col>
        ))}
      </Row>
    </PaginaPadrao>
  );
};

export default SistemaInicioUsuario;
