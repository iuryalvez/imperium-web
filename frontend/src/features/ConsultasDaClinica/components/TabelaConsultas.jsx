import React from 'react';
import { Table, Space, Tooltip, Button, Popconfirm, Tag } from 'antd';
import { EditOutlined, DeleteOutlined, FileTextOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';

const TabelaConsultas = ({ consultas, onEdit, onDelete, onOpenRelatorio, loading }) => {
    const colunas = [
        {
            title: 'Data da Consulta',
            dataIndex: 'dataConsulta',
            render: (data) => dayjs(data).format('DD/MM/YYYY HH:mm'),
            sorter: (a, b) => dayjs(a.dataConsulta).unix() - dayjs(b.dataConsulta).unix(),
        },
        { title: 'Paciente', dataIndex: 'nomeAnimal', sorter: (a, b) => a.nomeAnimal.localeCompare(b.nomeAnimal) },
        { title: 'Tutor', dataIndex: 'nomeTutor', sorter: (a, b) => a.nomeTutor.localeCompare(b.nomeTutor) },
        { title: 'Veterinário', dataIndex: 'nomeVeterinario' },
        {
            title: 'Relatório',
            dataIndex: 'relatorioPreenchido',
            render: (preenchido) => (
                <Tag color={preenchido ? 'green' : 'orange'}>{preenchido ? 'Preenchido' : 'Pendente'}</Tag>
            ),
            filters: [
                { text: 'Preenchido', value: true },
                { text: 'Pendente', value: false },
            ],
            onFilter: (value, record) => record.relatorioPreenchido === value,
        },
        {
            title: 'Ações',
            key: 'acoes',
            render: (_, record) => (
                <Space>
                    <Tooltip title={record.relatorioPreenchido ? "Ver/Editar Relatório" : "Preencher Relatório"}>
                        <Button icon={<FileTextOutlined />} type="primary" onClick={() => onOpenRelatorio(record)} />
                    </Tooltip>
                    <Tooltip title="Editar Consulta"><Button icon={<EditOutlined />} onClick={() => onEdit(record)} /></Tooltip>
                    <Tooltip title="Excluir Consulta">
                        <Popconfirm title="Tem certeza?" onConfirm={() => onDelete(record.id)}>
                            <Button icon={<DeleteOutlined />} danger />
                        </Popconfirm>
                    </Tooltip>
                </Space>
            ),
        },
    ];

    return <Table dataSource={consultas} columns={colunas} rowKey="id" loading={loading} />;
};

export default TabelaConsultas;