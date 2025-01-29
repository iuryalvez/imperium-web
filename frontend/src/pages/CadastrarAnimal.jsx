import React, { useState } from 'react';
import axios from 'axios';
import Navsidebar from '../components/organisms/Navsidebar';

import '../App.css';
import '../components/styles/CadastrarAnimal.css';

function CadastrarAnimal() {
  // Estado para armazenar os dados do formulário
  const [formData, setFormData] = useState({
    tutorId: '',
    nome: '',
    especie: 0,
    nascimento: '',
    idade: '',
    sexo: 0,
    castrado: false,
    peso: '',
    condicao_corporal: 1,
    grau_atividade: 0,
    gestacao: false,
    lactacao: false,
    semana_lactacao: '',
    semana_gestacao: '',
  });

  // Estado para indicar o carregamento
  const [isLoading, setIsLoading] = useState(false);
  // Estado para armazenar mensagens de erro
  const [error, setError] = useState('');

  // Função para lidar com mudanças nos campos do formulário
  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData({
      ...formData,
      [name]: type === 'checkbox' ? checked : value,
    });
  };

  // Função para lidar com o envio do formulário
  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    try {
      await axios.post('http://localhost:5432/api/Animais', formData);
      alert('Animal cadastrado com sucesso!');
      setFormData({
        tutorId: 0,
        nome: '',
        especie: 0,
        nascimento: '',
        idade: '',
        sexo: 0,
        castrado: false,
        peso: '',
        condicao_corporal: 1,
        grau_atividade: 0,
        gestacao: false,
        lactacao: false,
        semana_lactacao: '',
        semana_gestacao: '',
      });
    } catch (error) {
      if (error.response) {
        // Erro de resposta do servidor
        console.error('Erro ao cadastrar o animal:', error.response.data);
        setError(`Erro: ${error.response.data.message}`);
      } else if (error.request) {
        // Erro de requisição
        console.error('Erro ao cadastrar o animal:', error.request);
        setError('Erro na comunicação com o servidor.');
      } else {
        // Outro tipo de erro
        console.error('Erro ao cadastrar o animal:', error.message);
        setError('Erro desconhecido. Tente novamente.');
      }
    } finally {
      setIsLoading(false);
    }
  };

  // Listas de raças de cães e gatos
  const dogBreeds = [
    'Affenpinscher', 'Afghan Hound', 'Airedale Terrier', 'Akita', 'Alaskan Malamute',
    'American Bulldog', 'American Pit Bull Terrier', 'American Staffordshire Terrier',
    'Anatolian Shepherd Dog', 'Australian Cattle Dog', 'Australian Shepherd',
    'Australian Terrier', 'Basenji', 'Basset Hound', 'Beagle', 'Bearded Collie',
    'Bedlington Terrier', 'Belgian Malinois', 'Belgian Sheepdog', 'Belgian Tervuren',
    'Bernese Mountain Dog', 'Bichon Frise', 'Black and Tan Coonhound', 'Bloodhound',
    'Border Collie', 'Border Terrier', 'Borzoi', 'Boston Terrier', 'Bouvier des Flandres',
    'Boxer', 'Boykin Spaniel', 'Briard', 'Brittany', 'Brussels Griffon', 'Bull Terrier',
    'Bulldog', 'Bullmastiff', 'Cairn Terrier', 'Canaan Dog', 'Cane Corso', 'Cardigan Welsh Corgi',
    'Cavalier King Charles Spaniel', 'Chesapeake Bay Retriever', 'Chihuahua', 'Chinese Crested',
    'Chinese Shar-Pei', 'Chow Chow', 'Clumber Spaniel', 'Cocker Spaniel', 'Collie', 'Coonhound',
    'Coton de Tulear', 'Curly-Coated Retriever', 'Dachshund', 'Dalmatian', 'Dandie Dinmont Terrier',
    'Doberman Pinscher', 'Dogue de Bordeaux', 'English Bulldog', 'English Cocker Spaniel',
    'English Foxhound', 'English Setter', 'English Springer Spaniel', 'English Toy Spaniel',
    'Eskimo Dog', 'Finnish Spitz', 'Flat-Coated Retriever', 'French Bulldog', 'German Pinscher',
    'German Shepherd Dog', 'German Shorthaired Pointer', 'German Wirehaired Pointer', 'Giant Schnauzer',
    'Glen of Imaal Terrier', 'Golden Retriever', 'Gordon Setter', 'Great Dane', 'Great Pyrenees',
    'Greater Swiss Mountain Dog', 'Greyhound', 'Harrier', 'Havanese', 'Ibizan Hound', 'Icelandic Sheepdog',
    'Irish Setter', 'Irish Terrier', 'Irish Water Spaniel', 'Irish Wolfhound', 'Italian Greyhound',
    'Jack Russell Terrier', 'Japanese Chin', 'Japanese Spitz', 'Keeshond', 'Kerry Blue Terrier',
    'Komondor', 'Kuvasz', 'Labrador Retriever', 'Lakeland Terrier', 'Lhasa Apso', 'Lowchen', 'Maltese',
    'Manchester Terrier', 'Mastiff', 'Miniature Bull Terrier', 'Miniature Pinscher', 'Miniature Schnauzer',
    'Neapolitan Mastiff', 'Newfoundland', 'Norfolk Terrier', 'Norwegian Elkhound', 'Norwich Terrier',
    'Nova Scotia Duck Tolling Retriever', 'Old English Sheepdog', 'Otterhound', 'Papillon', 'Pekingese',
    'Pembroke Welsh Corgi', 'Petit Basset Griffon Vendeen', 'Pharaoh Hound', 'Plott', 'Pointer', 'Polish Lowland Sheepdog',
    'Pomeranian', 'Poodle', 'Portuguese Water Dog', 'Pug', 'Puli', 'Pumi', 'Rat Terrier', 'Redbone Coonhound',
    'Rhodesian Ridgeback', 'Rottweiler', 'Saint Bernard', 'Saluki', 'Samoyed', 'Schipperke', 'Scottish Deerhound',
    'Scottish Terrier', 'Sealyham Terrier', 'Shetland Sheepdog', 'Shiba Inu', 'Shih Tzu', 'Siberian Husky',
    'Silky Terrier', 'Skye Terrier', 'Sloughi', 'Smooth Fox Terrier', 'Soft Coated Wheaten Terrier',
    'Spanish Water Dog', 'Spinone Italiano', 'Staffordshire Bull Terrier', 'Standard Schnauzer',
    'Sussex Spaniel', 'Swedish Vallhund', 'Tibetan Mastiff', 'Tibetan Spaniel', 'Tibetan Terrier', 'Toy Fox Terrier',
    'Treeing Walker Coonhound', 'Vizsla', 'Weimaraner', 'Welsh Springer Spaniel', 'Welsh Terrier',
    'West Highland White Terrier', 'Whippet', 'Wire Fox Terrier', 'Wirehaired Pointing Griffon', 'Yorkshire Terrier'
  ];

  const catBreeds = [
    'Abyssinian', 'American Bobtail', 'American Curl', 'American Shorthair', 'American Wirehair',
    'Balinese', 'Bengal', 'Birman', 'Bombay', 'British Shorthair', 'Burmese', 'Burmilla', 'Chartreux',
    'Cornish Rex', 'Devon Rex', 'Egyptian Mau', 'European Burmese', 'Exotic', 'Havana Brown', 'Himalayan',
    'Japanese Bobtail', 'Khao Manee', 'Korat', 'LaPerm', 'Lykoi', 'Maine Coon', 'Manx', 'Norwegian Forest Cat',
    'Ocicat', 'Oriental', 'Persian', 'Ragamuffin', 'Ragdoll', 'Russian Blue', 'Savannah', 'Scottish Fold',
    'Selkirk Rex', 'Siamese', 'Siberian', 'Singapura', 'Snowshoe', 'Somali', 'Sphynx', 'Tonkinese', 'Toyger',
    'Turkish Angora', 'Turkish Van'
  ];

  return (
    <div>
      <div>
          <Navsidebar />
      </div>
      <div className='main-content'>
        {isLoading && <p>Loading...</p>}
        {error && <p style={{ color: 'red' }}>{error}</p>}
        <form className='form-animal' onSubmit={handleSubmit}>
          <h1>Cadastro de Animal</h1>
          <label>
            Nome do animal:
            <input type="text" name="nome" value={formData.nome} onChange={handleChange} required />
          </label>
          <label>
            Espécie:
            <select name="especie" value={formData.especie} onChange={handleChange} required>
              <option value={0}>Cão</option>
              <option value={1}>Gato</option>
            </select>
          </label>
          {/* Campo de seleção de raça condicional baseado na espécie */}
          {formData.especie === 0 ? (
            <label>
              Raça:
              <input 
                list="dogBreeds" 
                name="raca" 
                value={formData.raca} 
                onChange={handleChange} 
                required 
                />
              <datalist id="dogBreeds">
                {dogBreeds.map((breed) => (
                  <option key={breed} value={breed}>
                    {breed}
                  </option>
                ))}
              </datalist>
            </label>
          ) : (
            <label>
              Raça:
              <input 
                list="catBreeds" 
                name="raca" 
                value={formData.raca} 
                onChange={handleChange} 
                required 
              />
              <datalist id="catBreeds">
                {catBreeds.map((breed) => (
                  <option key={breed} value={breed}>
                    {breed}
                  </option>
                ))}
              </datalist>
            </label>
          )}
          <label>
            Nascimento:
            <input type="date" name="nascimento" value={formData.nascimento} onChange={handleChange} required />
          </label>
          <label>
            Idade:
            <input type="number" name="idade" value={formData.idade} onChange={handleChange} required min="1" />
          </label>
          <label>
            Sexo:
            <select name="sexo" value={formData.sexo} onChange={handleChange} required>
              <option value={0}>Macho</option>
              <option value={1}>Fêmea</option>
            </select>
          </label>
          <label>
            Castrado:
            <select name="castrado" value={formData.castrado} onChange={handleChange} required>
              <option value={true}>Sim</option>
              <option value={false}>Não</option>
            </select>
          </label>
          <label>
            Peso (kg):
            <input type="number" step="0.1" name="peso" value={formData.peso} onChange={handleChange} required min="0.1" />
          </label>
          <label>
            Condição Corporal:
            <select name="condicao_corporal" value={formData.condicao_corporal} onChange={handleChange} required>
              <option value={1}>Muito abaixo do ideal</option>
              <option value={3}>Pouco abaixo do ideal</option>
              <option value={5}>Ideal</option>
              <option value={7}>Pouco acima do ideal</option>
              <option value={9}>Muito acima do ideal</option>
            </select>
          </label>
          <label>
            Grau de Atividade:
            <select name="grau_atividade" value={formData.grau_atividade} onChange={handleChange} required>
              <option value={0}>Inativo</option>
              <option value={1}>Moderado</option>
              <option value={2}>Ativo</option>
            </select>
          </label>
          {/* Mostrar os campos de semanas apenas se o sexo for 'Fêmea' */}
          {formData.sexo == 1 && (
            <>
              <label>
                Gestação:
                <select name="gestacao" value={formData.gestacao} onChange={handleChange} required>
                  <option value={true}>Sim</option>
                  <option value={false}>Não</option>
                </select>
              </label>
              <label>
                Lactação:
                <select name="lactacao" value={formData.lactacao} onChange={handleChange} required>
                  <option value={true}>Sim</option>
                  <option value={false}>Não</option>
                </select>
              </label>
              <label>
                Semanas de Lactação:
                <input type="number" name="semana_lactacao" value={formData.semana_lactacao} onChange={handleChange} min="1" />
              </label>
              <label>
                Semanas de Gestação:
                <input type="number" name="semana_gestacao" value={formData.semana_gestacao} onChange={handleChange} min="1" />
              </label>
            </>
          )}
          <button className='button-padrao' type="submit">Cadastrar</button>
        </form>
      </div>
    </div>
  );
}

export default CadastrarAnimal;
