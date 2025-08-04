import React from 'react';
import { Modal, Button, Skeleton } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { useConsultasManager } from './useConsultasManager';
import TabelaConsultas from './components/TabelaConsultas';
import FormularioConsulta from './components/FormularioConsulta';

const ConsultasDaClinica = () => {
    const {
        consultas,
        loading,
        isModalOpen,
        editingConsulta,
        handleOpenModal,
        handleCloseModal,
        handleSubmit,
        handleDelete,
    } = useConsultasManager();

    // Placeholder para a lógica do relatório
    const handleOpenRelatorio = (consulta) => {
        alert(`Abrir modal de relatório para a consulta ID: ${consulta.id}`);
    };

    if (loading) {
        return <Skeleton active />;
    }

    return (
        <div>
            <div className="flex justify-between items-center mb-4">
                <h1 className="text-2xl font-bold">Gerenciamento de Consultas</h1>
                <Button type="primary" icon={<PlusOutlined />} onClick={() => handleOpenModal()}>
                    Agendar Consulta
                </Button>
            </div>
            
            <TabelaConsultas 
                consultas={consultas} 
                onEdit={handleOpenModal} 
                onDelete={handleDelete} 
                onOpenRelatorio={handleOpenRelatorio}
                loading={loading}
            />

            <Modal
                title={editingConsulta ? "Editar Consulta" : "Agendar Nova Consulta"}
                open={isModalOpen}
                onCancel={handleCloseModal}
                footer={null}
                destroyOnClose
            >
                <FormularioConsulta
                    initialValues={editingConsulta}
                    onFinish={handleSubmit}
                    onCancel={handleCloseModal}
                />
            </Modal>
        </div>
    );
};

export default ConsultasDaClinica;