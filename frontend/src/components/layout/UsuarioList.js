import React, { useState, useEffect } from 'react';
import axios from 'axios';

const UsuarioList = () => {
    const [usuarios, setUsuarios] = useState([]);

    useEffect(() => {
        axios.get('/api/usuarios')
            .then(response => setUsuarios(response.data))
            .catch(error => console.error('Error fetching usuarios:', error));
    }, []);

    return (
        <div>
            <h2>Pet List</h2>
            <ul>
                {usuarios.map(pet => (
                    <li key={pet.id}>{pet.name} - {pet.type}</li>
                ))}
            </ul>
        </div>
    );
};

export default UsuarioList;
