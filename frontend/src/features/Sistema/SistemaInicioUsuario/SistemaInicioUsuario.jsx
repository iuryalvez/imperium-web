import React, { useEffect, useState } from "react";

import { Card, Typography, Spin, Alert, Row, Col, Button } from "antd";
import { autenticacaoAPI } from "../../../api/autenticacaoAPI";
import { useAuth } from "../../../services/AuthContext";

import "./SistemaInicioUsuario.css";

const { Title, Text } = Typography;

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
      <div className="flex justify-center items-center h-screen">
        <Spin size="large" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mx-auto my-8">
        <Title level={2}>Lista de Vínculos</Title>
        <Alert message={error} type="error" showIcon />
      </div>
    );
  }

  return (
    <div className="vinculos-container">
      <Title level={2} className="vinculos-title">
        Lista de Vínculos
      </Title>

      <Row gutter={[24, 24]}>
        {vinculos.map((vinculo) => (
          <Col xs={24} md={12} lg={8} key={vinculo.id}>
            <Card
              title={vinculo.nomeClinica || "Clínica Desconhecida"}
              bordered={false}
              className="vinculo-card"
              actions={[
                <Button
                  type="primary" // Usará a cor do seu themeConfig
                  onClick={() => escolherVinculo(vinculo.id)}
                  loading={isLoading} // Usa a prop 'loading' para um UX melhor
                >
                  Selecionar
                </Button>,
              ]}
            >
              <Text strong>Perfis:</Text>
              <br />
              <Text>
                {vinculo.perfis?.length > 0
                  ? vinculo.perfis.join(", ")
                  : "Nenhum perfil vinculado"}
              </Text>
            </Card>
          </Col>
        ))}
      </Row>
    </div>
  );
};

export default SistemaInicioUsuario;
