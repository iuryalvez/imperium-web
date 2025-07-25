import React, { useState, useEffect, useCallback } from 'react';
import { List, Button, Space, Modal, Popconfirm, message, Tooltip, Tag, Input, Divider } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined, AppstoreOutlined } from '@ant-design/icons';
import { racasAPI } from '../../api/sistema/racasAPI';
import FormularioCadastroRaca from '../molecules/Cadastro/CadastroRacas';
import FormularioEdicaoRaca from '../molecules/Edicao/EdicaoRaca';

const GerenciadorDeRacas = () => {
    // --- State Management ---
    const [loading, setLoading] = useState(true);
    const [modalCadastroVisivel, setModalCadastroVisivel] = useState(false);
    const [modalEdicaoVisivel, setModalEdicaoVisivel] = useState(false);
    const [racaSelecionada, setRacaSelecionada] = useState(null);
    
    // NOVO: Estados para a lógica de busca
    const [searchTerm, setSearchTerm] = useState(''); // O que o usuário digita
    const [allRacas, setAllRacas] = useState([]); // A lista mestra, nunca modificada
    const [filteredRacas, setFilteredRacas] = useState([]); // A lista exibida, resultado da busca

    // --- Data Fetching ---
    const buscarRacas = useCallback(async () => {
        setLoading(true);
        try {
            const data = await racasAPI.listar();
            setAllRacas(data); // Guarda a lista completa
        } catch (error) {
            message.error("Erro ao carregar lista de raças.");
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        buscarRacas();
    }, [buscarRacas]);

    // --- Handlers ---
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

    // NOVO: Handler para a busca
    const handleSearch = (e) => {
        const term = e.target.value.toLowerCase();
        setSearchTerm(term);
        if (term.length > 0) {
            const filtered = allRacas.filter(raca => 
                raca.nome.toLowerCase().includes(term.toLowerCase())
            );
            setFilteredRacas(filtered);
        } else {
            setFilteredRacas([]);
        }
    };

    const handleViewAll = () => {
        setFilteredRacas(allRacas); // Preenche a lista de resultados com a lista completa.
        setSearchTerm(''); // Limpa o campo de busca para consistência.
    };

    return (
        <div>
            {/* --- Cabeçalho com Busca e Botão de Cadastrar --- */}
            <div style={{ display: 'flex', gap: 16, marginBottom: 16, alignItems: 'center' }}>
                <Input.Search
                    placeholder="Buscar raça pelo nome..."
                    value={searchTerm}
                    onChange={handleSearch}
                    style={{ flex: 1 }}
                    allowClear
                />
                <Space>
                    {/* 3. MUDANÇA: Novo botão adicionado */}
                    <Button icon={<AppstoreOutlined />} onClick={handleViewAll}>
                        Visualizar Todas
                    </Button>
                    <Button type="primary" icon={<PlusOutlined />} onClick={() => setModalCadastroVisivel(true)}>
                        Cadastrar Nova Raça
                    </Button>
                </Space>
            </div>

            <Divider />

            {/* --- Lista de Resultados --- */}
            <List
                loading={loading}
                itemLayout="horizontal"
                dataSource={filteredRacas}
                renderItem={(raca) => (
                    <List.Item
                        actions={[
                            <Tooltip title="Editar">
                                <Button type="text" icon={<EditOutlined />} onClick={() => abrirModalEdicao(raca)} />
                            </Tooltip>,
                            <Tooltip title="Excluir">
                                <Popconfirm
                                    title="Tem certeza que deseja excluir esta raça?"
                                    onConfirm={() => handleDeletar(raca.id)}
                                    okText="Sim" cancelText="Não"
                                >
                                    <Button type="text" icon={<DeleteOutlined />} danger />
                                </Popconfirm>
                            </Tooltip>
                        ]}
                    >
                        <List.Item.Meta
                            title={raca.nome}
                            description={<Tag color={raca.especie === 'Cao' ? 'blue' : 'green'}>{raca.especie}</Tag>}
                        />
                    </List.Item>
                )}
                locale={{ emptyText: 'Nenhuma raça encontrada.' }}
            />

            {/* --- Modais (lógica inalterada) --- */}
            <Modal title="Cadastrar Nova Raça" open={modalCadastroVisivel} onCancel={() => setModalCadastroVisivel(false)} footer={null} destroyOnClose>
                <FormularioCadastroRaca 
                    onSuccess={() => { setModalCadastroVisivel(false); buscarRacas(); }}
                    onCancel={() => setModalCadastroVisivel(false)}
                />
            </Modal>
            <Modal title="Editar Raça" open={modalEdicaoVisivel} onCancel={() => setModalEdicaoVisivel(false)} footer={null} destroyOnClose>
                {racaSelecionada && (
                    <FormularioEdicaoRaca
                        valoresIniciais={racaSelecionada}
                        onSuccess={() => { setModalEdicaoVisivel(false); buscarRacas(); }}
                        onCancel={() => setModalEdicaoVisivel(false)}
                    />
                )}
            </Modal>
        </div>
    );
};

export default GerenciadorDeRacas;