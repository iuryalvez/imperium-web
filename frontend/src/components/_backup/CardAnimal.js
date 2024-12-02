import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import './CardListagem.css'

const CardAnimal = ({ id, info }) => {
  const navigate = useNavigate();

  const handleAcessar = (id) => {
    navigate(`/animais/${id}`);
  };
  
  return (
    <div title={<Link to={`/animais/${id}`}>Animal de id: {id}</Link>} className='card' extra={<Link to={`/animais/${id}/editar`}>Editar</Link>}>
      <p>{info}</p>
      <button className='button-menu' onClick={() => handleAcessar(id)}>Acessar</button>
    </div>
  );
};

export default CardAnimal;
