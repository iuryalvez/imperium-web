import React, { useEffect, useState } from 'react';
import TemplateListaUsuarios from '../components/template/TemplateListaUsuarios';
import { usuario } from '../api/usuariosAPI';
import Navsidebar from '../components/organisms/Navsidebar.jsx';

const ListaUsuarios = () => {
  const [usuarios, setUsuarios] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchUsuarios = async () => {
      try {
        const response = await usuario.listar();
        console.log('Usuários na Page: ', response);
        setUsuarios(response);
      } catch (err) {
        setError('Erro ao carregar os usuários.');
      } finally {
        setLoading(false);
      }
    };

    fetchUsuarios();
  }, []);

  if (loading) {
    return (
      <div className='main-content'>
        <Navsidebar></Navsidebar>
        <div className='usuarios-content'>
          <p>Carregando usuários...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className='main-content'>
        <Navsidebar></Navsidebar>
        <div className='usuarios-content'>
          <p className="text-red-500">{error}</p>
        </div>
      </div>
    );
  }

  return <TemplateListaUsuarios usuarios={usuarios} />;
};

export default ListaUsuarios;
