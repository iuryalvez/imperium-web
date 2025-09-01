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
import FormularioCadastroRaca from "../molecules/Cadastro/FormCadastroRacas";
import FormularioEdicaoRaca from "../molecules/Edicao/FormEdicaoRaca";

const GerenciadorDeRacas = () => {
  const [loading, setLoading] = useState(true);
  const [isModalCadastroOpen, setIsModalCadastroOpen] = useState(false);
  const [modalEdicaoVisivel, setModalEdicaoVisivel] = useState(false);
  const [editingRaca, setEditingRaca] = useState(null);

  const [searchTerm, setSearchTerm] = useState(""); // O que o usuário digita
  const [allRacas, setAllRacas] = useState([]); // A lista mestra, nunca modificada
  const [filteredRacas, setFilteredRacas] = useState([]); // A lista exibida, resultado da busca
  const [isSearched, setIsSearched] = useState(false);

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
    setIsModalCadastroOpen(false);
    setModalEdicaoVisivel(false);
    // 2. Limpa o campo de busca
    setSearchTerm("");
  };

  const handleDeletar = async (id) => {
    try {
      await racasAPI.deletar(id);
      message.success("Raça excluída com sucesso!");
      await buscarRacas(); // Re-busca todos os dados para garantir consistência
    } catch (error) {
      message.error("Erro ao excluir raça.");
    }
  };

  const abrirModalEdicao = (raca) => {
    setEditingRaca(raca);
    setModalEdicaoVisivel(true);
  };

  const handleSearch = (e) => {
    const term = e.target.value;
    setSearchTerm(term);
    setIsSearched(false); // Qualquer busca desativa o modo "Visualizar Todas"

    if (term.length > 0) {
      const filtered = allRacas.filter((raca) =>
        raca.nome.toLowerCase().includes(term.toLowerCase())
      );
      setFilteredRacas(filtered);
    } else {
      setIsSearched(false); // Qualquer busca desativa o modo "Visualizar Todas"
      setFilteredRacas([]);
    }
    setIsSearched(true); // Qualquer busca desativa o modo "Visualizar Todas"
  };

  const handleViewAll = () => {
    setFilteredRacas(allRacas); // Preenche a lista de resultados com a lista completa.
    setIsSearched(true);
    setSearchTerm(""); // Limpa o campo de busca para consistência.
  };

  const handleFilter = (especie) => {
    setFilteredRacas(allRacas.filter((r) => r.especie === especie));
    setSearchTerm("");
    setIsSearched(true);
  };

  const handleClearResults = () => {
    setFilteredRacas([]);
    setSearchTerm("");
    setIsSearched(false);
  };

  return (
    <div>
      <div
        style={{
          display: "flex",
          flexDirection: "column", // Empilha os filhos (as linhas) um abaixo do outro
          gap: 16, // Espaço entre as linhas
          marginBottom: 16,
        }}
      >
        <div
          style={{
            display: "flex",
            gap: 16,
            alignItems: "center",
            flexWrap: "wrap",
          }}
        >
          <Input.Search
            placeholder="Ex: Golden Retriever"
            value={searchTerm}
            onChange={handleSearch}
            style={{ flex: 1, minWidth: 200 }}
            allowClear
          />
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={() => setIsModalCadastroOpen(true)}
          >
            Nova Raça
          </Button>
        </div>
        <div>
          <Space wrap>
            {isSearched && (
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
          </Space>
        </div>
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
        open={isModalCadastroOpen}
        onCancel={() => setIsModalCadastroOpen(false)}
        footer={null}
        destroyOnClose
        style={{ minWidth: "40vw", maxWidth: "80vw" }} // importante
        styles={{
          body: { maxHeight: "80vh", overflowY: "auto" }, // importante
        }}
      >
        <FormularioCadastroRaca
          onSuccess={handleOperationSuccess}
          onCancel={() => setIsModalCadastroOpen(false)}
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
        {editingRaca && (
          <FormularioEdicaoRaca
            valoresIniciais={editingRaca}
            onSuccess={handleOperationSuccess}
            onCancel={() => setModalEdicaoVisivel(false)}
          />
        )}
      </Modal>
    </div>
  );
};

export default GerenciadorDeRacas;
