import React, { useState, useEffect } from 'react';
import { Form, Input, Select, DatePicker, Switch, InputNumber, Row, Col, Slider } from 'antd';
import { racasAPI } from '../../../api/sistema/racasAPI'; // Ajuste o caminho
import { tutoresDaClinicaAPI } from '../../../api/clinica/tutoresDaClinicaAPI'; // Ajuste o caminho

// --- Opções para os Selects ---
const opcoesEspecie = [ { value: 1, label: 'Cão' }, { value: 2, label: 'Gato' } ];
const opcoesSexo = [ { value: 1, label: 'Macho' }, { value: 2, label: 'Fêmea' } ];
// NOVO: Opções para os novos campos
const opcoesGrauAtividade = [
    { value: 1, label: 'Baixo' },
    { value: 2, label: 'Moderado' },
    { value: 3, label: 'Alto' },
];
// Usaremos um Slider para Condição Corporal para uma melhor UX, mas um Select também funcionaria
const marcasCondicaoCorporal = {
  1: "1",
  2: "2",
  3: "3",
  4: "4",
  5: "5",
  6: "6",
  7: "7",
  8: "8",
  9: "9",
};

const FormularioCadastroAnimaisDaClinica = ({ form }) => {
    // --- State para dados assíncronos ---
    const [racasDeCaes, setRacasDeCaes] = useState([]);
    const [racasDeGatos, setRacasDeGatos] = useState([]);
    const [loadingRacas, setLoadingRacas] = useState(false);
    const [tutores, setTutores] = useState([]);
    const [loadingTutores, setLoadingTutores] = useState(false);

    // --- Observadores para reatividade do formulário ---
    const especie = Form.useWatch('especie', form);
    const sexo = Form.useWatch('sexo', form);
    const gestacao = Form.useWatch('gestacao', form);
    const estaLactando = Form.useWatch('estaLactando', form);

    const isFemea = sexo === 2;
    const isGestando = isFemea && gestacao;

    // --- Effects para buscar dados e controlar o formulário ---
    useEffect(() => {
        setLoadingRacas(true);
        setLoadingTutores(true);
        Promise.all([racasAPI.listar(1), racasAPI.listar(2), tutoresDaClinicaAPI.listar()])
            .then(([caesData, gatosData, tutoresData]) => {
                setRacasDeCaes(caesData.map(r => ({ label: r.nome, value: r.id })));
                setRacasDeGatos(gatosData.map(r => ({ label: r.nome, value: r.id })));
                setTutores(tutoresData.map(t => ({ 
                    label: `${t.nome} (${t.cpf || 'CPF não informado'})`, 
                    value: t.id 
                })));
            })
            .catch(err => console.error("Erro ao carregar dados:", err))
            .finally(() => {
                setLoadingRacas(false);
                setLoadingTutores(false);
            });
    }, []);

    useEffect(() => { form.setFieldsValue({ raca: undefined }); }, [especie, form]);

    useEffect(() => {
        if (!isFemea) {
            form.setFieldsValue({
                gestacao: false,
                semanaGestacao: undefined,
                estaLactando: false, // Também reseta o novo switch
                lactacao: 0,
                semanaLactacao: undefined,
            });
        }
    }, [isFemea, form]);

    // NOVO: Effect para limpar os dados de lactação quando o switch for desligado
    useEffect(() => {
        if (!estaLactando) {
            form.setFieldsValue({
                lactacao: 0,
                semanaLactacao: undefined,
            });
        }
    }, [estaLactando, form]);

    return (
        <Row gutter={20}>
            {/* --- Coluna 1: Informações Básicas --- */}
            <Col xs={24} sm={12}>
                <Form.Item label="Tutor(es) Responsável(is)" name="tutorId">
                    <Select mode="multiple" showSearch placeholder="Busque por nome ou CPF" loading={loadingTutores} options={tutores} optionFilterProp="label" />
                </Form.Item>
                <Form.Item label="Nome do Animal" name="nome" rules={[{ required: true, message: "O nome é obrigatório" }]}>
                    <Input placeholder="Ex: Bob" />
                </Form.Item>
                <Form.Item label="Espécie" name="especie" rules={[{ required: true, message: "A espécie é obrigatória" }]}>
                    <Select options={opcoesEspecie} placeholder="Selecione a espécie" />
                </Form.Item>
                <Form.Item label="Raça" name="raca" rules={[{ required: true, message: "A raça é obrigatória" }]}>
                    <Select placeholder="Selecione a raça" disabled={!especie} loading={loadingRacas} options={especie === 1 ? racasDeCaes : racasDeGatos} />
                </Form.Item>
                <Form.Item label="Data de Nascimento" name="nascimento" rules={[{ required: true, message: "A data é obrigatória" }]}>
                    <DatePicker style={{ width: '100%' }} format="DD/MM/YYYY" />
                </Form.Item>
                 <Form.Item label="Sexo" name="sexo" rules={[{ required: true, message: "O sexo é obrigatório" }]}>
                    <Select options={opcoesSexo} placeholder="Selecione o sexo" />
                </Form.Item>
                
                <Form.Item label="Condição Corporal" name="condicaoCorporal" initialValue={5} rules={[{ required: true, message: "A condição corporal é obrigatória" }]}>
                    <Slider min={1} max={9} marks={marcasCondicaoCorporal} />
                </Form.Item>
                <Form.Item label="Grau de Atividade" name="grauAtividade" rules={[{ required: true, message: "O grau de atividade é obrigatório" }]}>
                    <Select options={opcoesGrauAtividade} placeholder="Selecione o grau de atividade" />
                </Form.Item>
            </Col>

            {/* --- Coluna 2: Status e Condicionais --- */}
            <Col xs={24} sm={12}>
                <Form.Item label="Peso (kg)" name="peso" rules={[{ required: true, message: "O peso é obrigatório" }]}>
                    <InputNumber style={{ width: '100%' }} min={0} precision={2} placeholder="Ex: 12.5" />
                </Form.Item>
                
                <Form.Item name="castrado" label="Castrado?" valuePropName="checked" initialValue={false}>
                    <Switch checkedChildren="SIM" unCheckedChildren="NÃO" />
                </Form.Item>
                
                {isFemea && (
                    <>
                        <Form.Item name="gestacao" label="Em Gestação?" valuePropName="checked" initialValue={false}>
                            <Switch checkedChildren="SIM" unCheckedChildren="NÃO" />
                        </Form.Item>
                        {isGestando && (
                            <Form.Item name="semanaGestacao" label="Semanas de Gestação">
                                <InputNumber style={{ width: '100%' }} min={1} max={45} placeholder="Nº de semanas" />
                            </Form.Item>
                        )}

                        {/* 2. LÓGICA DE LACTAÇÃO ALTERADA */}
                        <Form.Item name="estaLactando" label="Em Lactação?" valuePropName="checked" initialValue={false}>
                            <Switch checkedChildren="SIM" unCheckedChildren="NÃO" />
                        </Form.Item>
                        {estaLactando && (
                            <>
                                <Form.Item name="lactacao" label="Nº de Filhotes" rules={[{ required: true, message: "Informe o nº de filhotes" }]}>
                                    <InputNumber style={{ width: '100%' }} min={1} placeholder="Quantos filhotes?" />
                                </Form.Item>
                                <Form.Item name="semanaLactacao" label="Semanas de Lactação">
                                    <InputNumber style={{ width: '100%' }} min={1} max={20} placeholder="Nº de semanas" />
                                </Form.Item>
                            </>
                        )}
                    </>
                )}
            </Col>
        </Row>
    );
};

export default FormularioCadastroAnimaisDaClinica;