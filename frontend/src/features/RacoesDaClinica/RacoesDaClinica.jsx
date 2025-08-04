import React from 'react';
import { Modal, Button, Skeleton } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { useRacoesManager } from './useRacoesManager';
import TabelaRacoes from './components/TabelaRacoes';
import FormularioRacao from './components/FormularioRacao';

const RacoesDaClinica = () => {
    const {
        racoes,
        loading,
        isModalOpen,
        editingRacao,
        handleOpenModal,
        handleCloseModal,
        handleSubmit,
        handleDelete,
    } = useRacoesManager();

    if (loading) {
        return <Skeleton active />;
    }

    return (
        <div>
            <div className="flex justify-between items-center mb-4">
                <h1 className="text-2xl font-bold">Gerenciamento de Rações</h1>
                <Button type="primary" icon={<PlusOutlined />} onClick={() => handleOpenModal()}>
                    Cadastrar Ração
                </Button>
            </div>
            
            <TabelaRacoes 
                racoes={racoes} 
                onEdit={handleOpenModal} 
                onDelete={handleDelete} 
                loading={loading}
            />

            <Modal
                title={editingRacao ? "Editar Ração" : "Cadastrar Nova Ração"}
                open={isModalOpen}
                onCancel={handleCloseModal}
                footer={null}
                destroyOnClose
            >
                <FormularioRacao
                    initialValues={editingRacao}
                    onFinish={handleSubmit}
                    onCancel={handleCloseModal}
                />
            </Modal>
        </div>
    );
};

export default RacoesDaClinica;