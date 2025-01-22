import React, { useEffect, useState } from 'react';
import axios from 'axios';

import ListaCards from '../components/organisms/ListaCards';
import FormUsuario from '../components/organisms/FormUsuarios';

import { Modal, Button } from 'antd'; // Usando Ant Design
import Sidebar from '../components/molecules/Sidebar';

function Usuarios() {
  const [usuarios, setUsuarios] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [usuarioSelecionado, setUsuarioSelecionado] = useState(null);

  const fetchUsuarios = async () => {
    try {
      const response = await axios.get('http://localhost:5050/api/clinica/usuarios', {
        withCredentials: true,
      });
      setUsuarios(response.data);
    } catch (error) {
      console.error('Erro ao buscar usuários:', error);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchUsuarios();
  }, []);

  const handleOpenModal = (usuario = null) => {
    setUsuarioSelecionado(usuario);
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setUsuarioSelecionado(null);
    setIsModalOpen(false);
  };

  return (
    <div>
      <Sidebar />
        <Button type="primary" onClick={() => handleOpenModal()}>
          Criar Novo Usuário
        </Button>
        {isLoading ? (
          <p>Carregando...</p>
        ) : (
          <ListaCards
            tipoEntidade="usuario"
            lista={usuarios}
            onEdit={handleOpenModal} // Passa a função para editar
          />
        )}
        <Modal
          title={usuarioSelecionado ? 'Editar Usuário' : 'Criar Usuário'}
          open={isModalOpen}
          onCancel={handleCloseModal}
          footer={null}
        >
          <FormUsuario
            usuario={usuarioSelecionado}
            onSubmitSuccess={fetchUsuarios}
            closeModal={handleCloseModal}
          />
        </Modal>
    </div>
  );
}

export default Usuarios;
