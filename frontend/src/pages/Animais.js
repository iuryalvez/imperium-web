import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { FaPlus } from "react-icons/fa";

import Navsidebar from "../components/Navsidebar";
import CardAnimal from "../components/CardAnimal";
import api from '../services/api';

import '../App.css';
import './Listas.css'

function Animais() {
    
    const navigate = useNavigate();

    const [animais, setAnimais] = useState([]);

    useEffect(() => {
        const fetchAnimais = async () => {
        try {
            const response = await api.get('http://localhost:5432/api/Animais');
            setAnimais(response.data);
        } catch (error) {
            console.error('Erro ao buscar os dados dos animais:', error);
        }
        };

        fetchAnimais();
    }, []);

    const handleEdit = (animalId) => {
        // Lógica para editar os dados do animal com o ID fornecido
        console.log('Editar dados do animal com ID:', animalId);
    };

    return (
        <div>    
            <div>
                <Navsidebar />
            </div>
            <main className='main-content'>
                <div className="titulo-animais">
                    <h2>Lista de animais</h2>
                    <div>
                        <button onClick={() => navigate('/animais/cadastrar')} className="button-bar">
                            <FaPlus className='icon-button-bar'/>
                            Cadastrar novo animal
                        </button>
                    </div>
                </div>
                {animais.map(animal => (
                    <div key={animal.id}>
                        <CardAnimal
                            id={animal.id}
                            info={animal.info}
                            onEdit={() => handleEdit(animal.id)}
                        />
                    </div>
                ))}
            </main>
        </div>
    )
}

export default Animais;