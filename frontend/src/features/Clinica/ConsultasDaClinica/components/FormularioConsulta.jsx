import React, { useState, useEffect } from "react";
import { Form, DatePicker } from "antd";
import dayjs from "dayjs";
import { animaisDaClinicaAPI } from "../../../../api/clinica/animaisDaClinicaAPI";
import SelectBuscaRemota from "../../../../components/atoms/SelectBuscaRemota";
import { tutoresDaClinicaAPI } from "../../../../api/clinica/tutoresDaClinicaAPI";

const FormularioConsulta = ({ initialValues, onFinish }) => {
  const [form] = Form.useForm();
  // Popula o formulário para edição
  useEffect(() => {
    if (initialValues) {
      form.setFieldsValue({
        ...initialValues,
        dataConsulta: initialValues.dataConsulta
          ? dayjs(initialValues.dataConsulta)
          : null,
      });
    } else {
      form.resetFields();
    }
  }, [initialValues, form]);

  // Tutores do animal escolhido: sem tutor = animal sob tutela da clínica
  const selectedAnimalId = Form.useWatch("animalId", form);
  const [tutorIdsDoAnimal, setTutorIdsDoAnimal] = useState(null);

  useEffect(() => {
    if (!selectedAnimalId) {
      setTutorIdsDoAnimal(null);
      return;
    }
    let ativo = true;
    animaisDaClinicaAPI
      .detalhes(selectedAnimalId)
      .then((resultado) => ativo && setTutorIdsDoAnimal(resultado?.data?.tutorIds || []))
      .catch(() => ativo && setTutorIdsDoAnimal(null));
    return () => {
      ativo = false;
    };
  }, [selectedAnimalId]);

  const isSobTutelaClinica = tutorIdsDoAnimal !== null && tutorIdsDoAnimal.length === 0;

  // Rótulos do animal/tutor da consulta em edição (podem estar inativos ou fora da busca)
  const animalAtual = initialValues?.animalId ? [{ id: initialValues.animalId, nome: initialValues.nomeAnimal }] : [];
  const tutorAtual = initialValues?.tutorId ? [{ id: initialValues.tutorId, nome: initialValues.nomeTutor }] : [];

  useEffect(() => {
    if (isSobTutelaClinica) {
      form.setFieldsValue({ tutorId: null });
    }
  }, [isSobTutelaClinica, form]);

  const handleFormFinish = async (values) => {
    try {
      await onFinish(values);
    } catch (error) {
      // Se onFinish lançar um erro, o modal não fecha
      console.error("Falha na submissão:", error);
    }
  };

  return (
    <Form
      id="form-cadastro-edicao-consulta"
      form={form}
      layout="vertical"
      onFinish={handleFormFinish}
      autoComplete="off"
    >
      <Form.Item
        name="animalId"
        label="Paciente (Animal)"
        rules={[{ required: true, message: "Selecione o animal." }]}
      >
        {/* Animais da clínica com busca no servidor; inativos aparecem desabilitados */}
        <SelectBuscaRemota
          placeholder="Busque e selecione o paciente"
          buscarOpcoes={animaisDaClinicaAPI.opcoes}
          opcoesAtuais={animalAtual}
        />
      </Form.Item>

      <Form.Item
        name="tutorId"
        label="Tutor Responsável (Opcional)"
        rules={[{ required: false }]}
      >
        {/* Tutores da clínica com busca no servidor; inativos aparecem desabilitados */}
        <SelectBuscaRemota
          allowClear
          placeholder={isSobTutelaClinica ? "Animal sob tutela da clínica" : "Busque e selecione o tutor (Opcional)"}
          buscarOpcoes={tutoresDaClinicaAPI.opcoes}
          opcoesAtuais={tutorAtual}
          disabled={isSobTutelaClinica}
        />
      </Form.Item>

      <Form.Item
        name="dataConsulta"
        label="Data e Hora da Consulta"
        rules={[{ required: true, message: "Selecione a data e hora." }]}
      >
        <DatePicker
          style={{ width: "100%" }}
          format="DD/MM/YYYY HH:mm"
          showTime={{ format: "HH:mm" }}
          placeholder="Selecione a data e hora"
        />
      </Form.Item>
    </Form>
  );
};

export default FormularioConsulta;
