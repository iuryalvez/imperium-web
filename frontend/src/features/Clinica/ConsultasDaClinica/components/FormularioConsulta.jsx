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
        setAnimais(
          animaisData.map((a) => ({
            label: `${a.nome} (${a.especie === 1 ? "Cão" : "Gato"})`,
            value: a.id,
          })),
        );
        setTutores(tutoresData.map((t) => ({ label: t.nome, value: t.id })));
      } catch (error) {
        message.error("Erro ao carregar animais e tutores.");
      } finally {
        setLoading(false);
      }
    };
    fetchData();
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
        label="Tutor Responsável"
        rules={[{ required: true, message: "Selecione o tutor." }]}
      >
        <Select
          showSearch
          placeholder="Selecione o tutor"
          options={tutores}
          loading={loading}
          optionFilterProp="label"
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
