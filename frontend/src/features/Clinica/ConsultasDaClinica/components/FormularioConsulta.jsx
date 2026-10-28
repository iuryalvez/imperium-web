import React, { useState, useEffect } from "react";
import { Form, Select, DatePicker, message } from "antd";
import dayjs from "dayjs";
import { animaisDaClinicaAPI } from "../../../../api/clinica/animaisDaClinicaAPI";
import { tutoresDaClinicaAPI } from "../../../../api/clinica/tutoresDaClinicaAPI";

const FormularioConsulta = ({ initialValues, onFinish }) => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [animais, setAnimais] = useState([]);
  const [tutores, setTutores] = useState([]);

  // Busca os dados para os selects
  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      try {
        const [animaisData, tutoresData] = await Promise.all([
          animaisDaClinicaAPI.listar(),
          tutoresDaClinicaAPI.listar(),
        ]);
        // Inativos aparecem desabilitados, exceto o que já está na consulta em edição
        const sufixoInativo = (item) => (item.isAtivo === false ? " (inativo)" : "");
        setAnimais(
          animaisData.map((a) => ({
            label: `${a.nome} (${a.especie === 1 ? "Cão" : "Gato"})${sufixoInativo(a)}`,
            value: a.id,
            tutorIds: a.tutorIds || [],
            disabled: a.isAtivo === false && a.id !== initialValues?.animalId,
          })),
        );
        setTutores(
          tutoresData.map((t) => ({
            label: `${t.nome}${sufixoInativo(t)}`,
            value: t.id,
            disabled: t.isAtivo === false && t.id !== initialValues?.tutorId,
          })),
        );
      } catch (error) {
        message.error("Erro ao carregar animais e tutores.");
      } finally {
        setLoading(false);
      }
    };
    fetchData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

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

  const selectedAnimalId = Form.useWatch("animalId", form);
  const animalSelecionado = animais.find(a => a.value === selectedAnimalId);
  const isSobTutelaClinica = animalSelecionado && animalSelecionado.tutorIds.length === 0;

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
        <Select
          showSearch
          placeholder="Selecione o paciente"
          options={animais}
          loading={loading}
          optionFilterProp="label"
        />
      </Form.Item>

      <Form.Item
        name="tutorId"
        label="Tutor Responsável (Opcional)"
        rules={[{ required: false }]}
      >
        <Select
          showSearch
          allowClear
          placeholder={isSobTutelaClinica ? "Animal sob tutela da clínica" : "Selecione o tutor (Opcional)"}
          options={tutores}
          loading={loading}
          optionFilterProp="label"
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
