import React, { useState, useEffect, useCallback } from "react";
import {
  List,
  Button,
  Space,
  Modal,
  Popconfirm,
  message,
  Tooltip,
  Tag,
  Input,
  Divider,
} from "antd";
import {
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  AppstoreOutlined,
  ClearOutlined,
} from "@ant-design/icons";
import { racasAPI } from "../../api/sistema/racasAPI";
import FormularioCadastroRaca from "../molecules/Cadastro/CadastroRacas";
import FormularioEdicaoRaca from "../molecules/Edicao/EdicaoRaca";

const GerenciadorDeRacas = () => {
  const [loading, setLoading] = useState(true);
  const [modalCadastroVisivel, setModalCadastroVisivel] = useState(false);
  const [modalEdicaoVisivel, setModalEdicaoVisivel] = useState(false);
  const [racaSelecionada, setRacaSelecionada] = useState(null);

  const [searchTerm, setSearchTerm] = useState(""); // O que o usuário digita
  const [allRacas, setAllRacas] = useState([]); // A lista mestra, nunca modificada
  const [filteredRacas, setFilteredRacas] = useState([]); // A lista exibida, resultado da busca
  const [isListFull, setIsListFull] = useState(false);

  const buscarRacas = useCallback(async () => {
    setLoading(true);
    try {
      const data = await racasAPI.listar();
      setAllRacas(data);
      return data; // Retorna os dados frescos
    } catch (error) {
      message.error("Erro ao carregar lista de raças.");
      return []; // Retorna array vazio em caso de erro
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    buscarRacas();
  }, [buscarRacas]);

  const handleOperationSuccess = async () => {
    // Fecha qualquer modal que esteja aberto
    setModalCadastroVisivel(false);
    setModalEdicaoVisivel(false);

    // 1. Re-busca os dados da API
    const racasAtualizadas = await buscarRacas();
    // 2. Limpa o campo de busca
    setSearchTerm("");
    // 3. Exibe a lista completa e atualizada
    setFilteredRacas(racasAtualizadas);
    setIsListFull(true);
  };

  const handleDeletar = async (id) => {
    try {
      await racasAPI.deletar(id);
      message.success("Raça excluída com sucesso!");
      buscarRacas(); // Re-busca todos os dados para garantir consistência
    } catch (error) {
      message.error("Erro ao excluir raça.");
    }
  };

  const abrirModalEdicao = (raca) => {
    setRacaSelecionada(raca);
    setModalEdicaoVisivel(true);
  };

  const handleSearch = (e) => {
    const term = e.target.value;
    setSearchTerm(term);
    setIsListFull(false); // Qualquer busca desativa o modo "Visualizar Todas"

    if (term.length > 0) {
      const filtered = allRacas.filter((raca) =>
        raca.nome.toLowerCase().includes(term.toLowerCase())
      );
      setFilteredRacas(filtered);
    } else {
      setFilteredRacas([]);
    }
  };

  const handleViewAll = () => {
    setFilteredRacas(allRacas); // Preenche a lista de resultados com a lista completa.
    setSearchTerm(""); // Limpa o campo de busca para consistência.
  };

  const handleFilter = (especie) => {
    setFilteredRacas(allRacas.filter((r) => r.especie === especie));
    setSearchTerm("");
    setIsListFull(false);
  };

  const handleClearResults = () => {
    setFilteredRacas([]);
    setSearchTerm("");
    setIsListFull(false);
  };

  return (
    <div>
      <div
        style={{
          display: "flex",
          flexWrap: "wrap",
          gap: 16,
          marginBottom: 16,
          alignItems: "center",
        }}
      >
        <Input.Search
          placeholder="Buscar raça pelo nome..."
          value={searchTerm}
          onChange={handleSearch}
          style={{ flex: 1, minWidth: 200 }}
          allowClear
        />
        <Space wrap>
          {/* Botão de Limpar condicional */}
          {isListFull && (
            <Button
              icon={<ClearOutlined />}
              onClick={handleClearResults}
              danger
            >
              Limpar
            </Button>
          )}
          <Button onClick={() => handleFilter("Cao")}>Listar Cães</Button>
          <Button onClick={() => handleFilter("Gato")}>Listar Gatos</Button>
          <Button icon={<AppstoreOutlined />} onClick={handleViewAll}>
            Visualizar Todas
          </Button>
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={() => setModalCadastroVisivel(true)}
          >
            Nova Raça
          </Button>
        </Space>
      </div>

      <Divider />

      <List
        loading={loading}
        itemLayout="horizontal"
        dataSource={filteredRacas}
        renderItem={(raca) => (
          <List.Item
            actions={[
              <Tooltip title="Editar">
                <Button
                  type="text"
                  icon={<EditOutlined />}
                  onClick={() => abrirModalEdicao(raca)}
                />
              </Tooltip>,
              <Tooltip title="Excluir">
                <Popconfirm
                  title="Tem certeza que deseja excluir esta raça?"
                  onConfirm={() => handleDeletar(raca.id)}
                  okText="Sim"
                  cancelText="Não"
                >
                  <Button type="text" icon={<DeleteOutlined />} danger />
                </Popconfirm>
              </Tooltip>,
            ]}
          >
            <List.Item.Meta
              title={raca.nome}
              description={
                <Tag color={raca.especie === "Cao" ? "blue" : "green"}>
                  {raca.especie}
                </Tag>
              }
            />
          </List.Item>
        )}
        locale={{ emptyText: "Nenhuma raça encontrada." }}
      />

      <Modal
        title="Cadastrar Nova Raça"
        open={modalCadastroVisivel}
        onCancel={() => setModalCadastroVisivel(false)}
        footer={null}
        destroyOnClose
        styles={{
          body: { maxHeight: "70vh", overflowY: "auto" },
        }}
      >
        <FormularioCadastroRaca
          onSuccess={handleOperationSuccess}
          onCancel={() => setModalCadastroVisivel(false)}
        />
      </Modal>
      <Modal
        title="Editar Raça"
        open={modalEdicaoVisivel}
        onCancel={() => setModalEdicaoVisivel(false)}
        footer={null}
        destroyOnClose
        styles={{
          body: { maxHeight: "70vh", overflowY: "auto" },
        }}
      >
        {racaSelecionada && (
          <FormularioEdicaoRaca
            valoresIniciais={racaSelecionada}
            onSuccess={handleOperationSuccess}
            onCancel={() => setModalEdicaoVisivel(false)}
          />
        )}
      </Modal>
    </div>
  );
};

export default GerenciadorDeRacas;
