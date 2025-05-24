import React, { useState, useEffect } from "react";
import {
  Input,
  Select,
  DatePicker,
  Checkbox,
  Button,
  Form,
  message,
  Typography,
  Switch,
  InputNumber,
} from "antd";
import { PlusOutlined } from "@ant-design/icons";
import { clinicasAPI } from "../../api/sistema/clinicasAPI";
import { IMaskInput } from "react-imask";

// const estadosBrasil = [
//   { value: 'AC', label: 'Acre' },
//   { value: 'AL', label: 'Alagoas' },
//   { value: 'AP', label: 'Amapá' },
//   { value: 'AM', label: 'Amazonas' },
//   { value: 'BA', label: 'Bahia' },
//   { value: 'CE', label: 'Ceará' },
//   { value: 'DF', label: 'Distrito Federal' },
//   { value: 'ES', label: 'Espírito Santo' },
//   { value: 'GO', label: 'Goiás' },
//   { value: 'MA', label: 'Maranhão' },
//   { value: 'MT', label: 'Mato Grosso' },
//   { value: 'MS', label: 'Mato Grosso do Sul' },
//   { value: 'MG', label: 'Minas Gerais' },
//   { value: 'PA', label: 'Pará' },
//   { value: 'PB', label: 'Paraíba' },
//   { value: 'PR', label: 'Paraná' },
//   { value: 'PE', label: 'Pernambuco' },
//   { value: 'PI', label: 'Piauí' },
//   { value: 'RJ', label: 'Rio de Janeiro' },
//   { value: 'RN', label: 'Rio Grande do Norte' },
//   { value: 'RS', label: 'Rio Grande do Sul' },
//   { value: 'RO', label: 'Rondônia' },
//   { value: 'RR', label: 'Roraima' },
//   { value: 'SC', label: 'Santa Catarina' },
//   { value: 'SP', label: 'São Paulo' },
//   { value: 'SE', label: 'Sergipe' },
//   { value: 'TO', label: 'Tocantins' }
// ];

const perfisSistema = [
  { label: "Administrador", value: "ADMIN" },
  { label: "Gestor", value: "GESTOR" },
  { label: "Técnico", value: "TECNICO" },
  { label: "Secretário", value: "SECRETARIO" },
  { label: "Veterinário", value: "VETERINARIO" },
  { label: "Inativo", value: "INATIVO" },
];

const perfisClinica = [
  { label: "Gestor", value: "GESTOR" },
  { label: "Técnico", value: "TECNICO" },
  { label: "Secretário", value: "SECRETARIO" },
  { label: "Veterinário", value: "VETERINARIO" },
  { label: "Inativo", value: "INATIVO" },
];

// Listas de raças de cães e gatos
const dogBreeds = [
  "Affenpinscher",
  "Afghan Hound",
  "Airedale Terrier",
  "Akita",
  "Alaskan Malamute",
  "American Bulldog",
  "American Pit Bull Terrier",
  "American Staffordshire Terrier",
  "Anatolian Shepherd Dog",
  "Australian Cattle Dog",
  "Australian Shepherd",
  "Australian Terrier",
  "Basenji",
  "Basset Hound",
  "Beagle",
  "Bearded Collie",
  "Bedlington Terrier",
  "Belgian Malinois",
  "Belgian Sheepdog",
  "Belgian Tervuren",
  "Bernese Mountain Dog",
  "Bichon Frise",
  "Black and Tan Coonhound",
  "Bloodhound",
  "Border Collie",
  "Border Terrier",
  "Borzoi",
  "Boston Terrier",
  "Bouvier des Flandres",
  "Boxer",
  "Boykin Spaniel",
  "Briard",
  "Brittany",
  "Brussels Griffon",
  "Bull Terrier",
  "Bulldog",
  "Bullmastiff",
  "Cairn Terrier",
  "Canaan Dog",
  "Cane Corso",
  "Cardigan Welsh Corgi",
  "Cavalier King Charles Spaniel",
  "Chesapeake Bay Retriever",
  "Chihuahua",
  "Chinese Crested",
  "Chinese Shar-Pei",
  "Chow Chow",
  "Clumber Spaniel",
  "Cocker Spaniel",
  "Collie",
  "Coonhound",
  "Coton de Tulear",
  "Curly-Coated Retriever",
  "Dachshund",
  "Dalmatian",
  "Dandie Dinmont Terrier",
  "Doberman Pinscher",
  "Dogue de Bordeaux",
  "English Bulldog",
  "English Cocker Spaniel",
  "English Foxhound",
  "English Setter",
  "English Springer Spaniel",
  "English Toy Spaniel",
  "Eskimo Dog",
  "Finnish Spitz",
  "Flat-Coated Retriever",
  "French Bulldog",
  "German Pinscher",
  "German Shepherd Dog",
  "German Shorthaired Pointer",
  "German Wirehaired Pointer",
  "Giant Schnauzer",
  "Glen of Imaal Terrier",
  "Golden Retriever",
  "Gordon Setter",
  "Great Dane",
  "Great Pyrenees",
  "Greater Swiss Mountain Dog",
  "Greyhound",
  "Harrier",
  "Havanese",
  "Ibizan Hound",
  "Icelandic Sheepdog",
  "Irish Setter",
  "Irish Terrier",
  "Irish Water Spaniel",
  "Irish Wolfhound",
  "Italian Greyhound",
  "Jack Russell Terrier",
  "Japanese Chin",
  "Japanese Spitz",
  "Keeshond",
  "Kerry Blue Terrier",
  "Komondor",
  "Kuvasz",
  "Labrador Retriever",
  "Lakeland Terrier",
  "Lhasa Apso",
  "Lowchen",
  "Maltese",
  "Manchester Terrier",
  "Mastiff",
  "Miniature Bull Terrier",
  "Miniature Pinscher",
  "Miniature Schnauzer",
  "Neapolitan Mastiff",
  "Newfoundland",
  "Norfolk Terrier",
  "Norwegian Elkhound",
  "Norwich Terrier",
  "Nova Scotia Duck Tolling Retriever",
  "Old English Sheepdog",
  "Otterhound",
  "Papillon",
  "Pekingese",
  "Pembroke Welsh Corgi",
  "Petit Basset Griffon Vendeen",
  "Pharaoh Hound",
  "Plott",
  "Pointer",
  "Polish Lowland Sheepdog",
  "Pomeranian",
  "Poodle",
  "Portuguese Water Dog",
  "Pug",
  "Puli",
  "Pumi",
  "Rat Terrier",
  "Redbone Coonhound",
  "Rhodesian Ridgeback",
  "Rottweiler",
  "Saint Bernard",
  "Saluki",
  "Samoyed",
  "Schipperke",
  "Scottish Deerhound",
  "Scottish Terrier",
  "Sealyham Terrier",
  "Shetland Sheepdog",
  "Shiba Inu",
  "Shih Tzu",
  "Siberian Husky",
  "Silky Terrier",
  "Skye Terrier",
  "Sloughi",
  "Smooth Fox Terrier",
  "Soft Coated Wheaten Terrier",
  "Spanish Water Dog",
  "Spinone Italiano",
  "Staffordshire Bull Terrier",
  "Standard Schnauzer",
  "Sussex Spaniel",
  "Swedish Vallhund",
  "Tibetan Mastiff",
  "Tibetan Spaniel",
  "Tibetan Terrier",
  "Toy Fox Terrier",
  "Treeing Walker Coonhound",
  "Vizsla",
  "Weimaraner",
  "Welsh Springer Spaniel",
  "Welsh Terrier",
  "West Highland White Terrier",
  "Whippet",
  "Wire Fox Terrier",
  "Wirehaired Pointing Griffon",
  "Yorkshire Terrier",
];

const catBreeds = [
  "Abyssinian",
  "American Bobtail",
  "American Curl",
  "American Shorthair",
  "American Wirehair",
  "Balinese",
  "Bengal",
  "Birman",
  "Bombay",
  "British Shorthair",
  "Burmese",
  "Burmilla",
  "Chartreux",
  "Cornish Rex",
  "Devon Rex",
  "Egyptian Mau",
  "European Burmese",
  "Exotic",
  "Havana Brown",
  "Himalayan",
  "Japanese Bobtail",
  "Khao Manee",
  "Korat",
  "LaPerm",
  "Lykoi",
  "Maine Coon",
  "Manx",
  "Norwegian Forest Cat",
  "Ocicat",
  "Oriental",
  "Persian",
  "Ragamuffin",
  "Ragdoll",
  "Russian Blue",
  "Savannah",
  "Scottish Fold",
  "Selkirk Rex",
  "Siamese",
  "Siberian",
  "Singapura",
  "Snowshoe",
  "Somali",
  "Sphynx",
  "Tonkinese",
  "Toyger",
  "Turkish Angora",
  "Turkish Van",
];

const FormCadastrarEntidade = ({
  entidade,
  nomeEntidade,
  onSubmit,
  onSuccess,
  setModalVisivel,
  tituloModal,
  camposSimplesCadastrar,
}) => {
  const campos = camposSimplesCadastrar[entidade] || [];
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [perfisSelecionados, setPerfisSelecionados] = useState([]);
  const [clinicas, setClinicas] = useState([]);
    const [especieSelecionada, setEspecieSelecionada] = useState(null);
  const [sexoSelecionado, setSexoSelecionado] = useState(null);
  const [gestacaoSelecionada, setGestacaoSelecionada] = useState(false);
  const [lactacaoSelecionada, setLactacaoSelecionada] = useState(0);

  useEffect(() => {
    if (entidade === "usuariosDoSistema") {
      clinicasAPI
        .listar()
        .then((data) => {
          // Aqui preparamos os dados no formato que o Select espera
          const clinicasFormatadas = data.map((clinica) => ({
            label: clinica.nomeClinica,
            value: clinica.id,
          }));
          setClinicas(clinicasFormatadas);
        })
        .catch((err) => console.error("Erro ao carregar clínicas:", err));
    }
  }, [entidade]);

  // Efeito específico para animais
  useEffect(() => {
    if (entidade === "animaisDaClinica") {
      // Observa mudanças nos campos relevantes
      const valores = form.getFieldsValue();
      setEspecieSelecionada(valores.especie);
      setSexoSelecionado(valores.sexo);
      setGestacaoSelecionada(valores.gestacao);
      setLactacaoSelecionada(valores.lactacao || 0);

      // Resetar campos dependentes quando mudar para macho
      if (valores.sexo === '1') {
        form.setFieldsValue({
          gestacao: false,
          semanaGestacao: undefined,
          lactacao: 0,
          semanaLactacao: undefined
        });
      }
    }
  }, [entidade, form]);

  const sanitize = (value) => value?.replace(/\D/g, "");

  const handleFinish = async (values) => {
    try {
      setLoading(true);

      if (entidade === "usuariosDoSistema" || "usuariosDaClinica") {
        // Verifica se o array de perfis contém "INATIVO"
        const isInativo = values.perfis?.includes("INATIVO") || false;

        // Atualiza o valor de IsInativo
        values.isAtivo = !isInativo;
      }

      if (values.cpf) values.cpf = sanitize(values.cpf);
      if (values.cep) values.cep = sanitize(values.cep);
      if (values.cnpj) values.cnpj = sanitize(values.cnpj);
      if (values.telefone1) values.telefone1 = sanitize(values.telefone1);
      if (values.telefone2) values.telefone2 = sanitize(values.telefone2);
      if (values.whatsapp) values.whatsapp = sanitize(values.whatsapp);

      const response = await onSubmit(values);
      message.success(`${nomeEntidade} criado com sucesso.`);
      form.resetFields();
      setPerfisSelecionados([]);
      if (onSuccess) onSuccess(response);
    } catch (error) {
      console.error(`Erro ao cadastrar ${nomeEntidade}:`, error);
      message.error(`Falha ao cadastrar ${nomeEntidade}.`);
    } finally {
      setLoading(false);
    }
  };

  const renderCampo = (campo) => {
    // Adicionando casos específicos para animais
    if (entidade === "animaisDaClinica") {
      if (campo.name === "raca") {
        return (
          <Select placeholder={campo.placeholder}>
            {especieSelecionada === '1' && 
              dogBreeds.map(breed => (
                <Select.Option key={breed} value={breed}>
                  {breed}
                </Select.Option>
              ))
            }
            {especieSelecionada === '2' && 
              catBreeds.map(breed => (
                <Select.Option key={breed} value={breed}>
                  {breed}
                </Select.Option>
              ))
            }
          </Select>
        );
      }
    }
    switch (campo.type) {
      case "text":
      case "email":
        return <Input type={campo.type} placeholder={campo.placeholder} />;
      case "password":
        return <Input.Password placeholder={campo.placeholder} />;
      case "select":
        return (
          <Select 
            placeholder={campo.placeholder}
            onChange={(value) => {
              // Captura mudanças nos campos importantes para animais
              if (entidade === "animaisDaClinica") {
                if (campo.name === "especie") setEspecieSelecionada(value);
                if (campo.name === "sexo") setSexoSelecionado(value);
              }
            }}
          >
            {campo.options?.map((opt) => (
              <Select.Option key={opt.value} value={opt.value}>
                {opt.label}
              </Select.Option>
            ))}
          </Select>
        );
      case "date":
        return <DatePicker style={{ width: "100%" }} />;
      case "boolean":
        return (
          <Switch 
            checkedChildren="Sim" 
            unCheckedChildren="Não"
            onChange={(checked) => {
              if (entidade === "animais" && campo.name === "gestacao") {
                setGestacaoSelecionada(checked);
              }
            }}
          />
        );
      case "number":
        return (
          <InputNumber 
            style={{ width: "100%" }}
            onChange={(value) => {
              if (entidade === "animaisDaClinica" && campo.name === "lactacao") {
                setLactacaoSelecionada(value || 0);
              }
            }}
          />
        );
      default:
        return null;
    }
  };

  const adminSelecionado = perfisSelecionados.includes("ADMIN");
  const outrosSelecionados = perfisSelecionados.some((val) =>
    ["GESTOR", "TECNICO", "SECRETARIO", "VETERINARIO"].includes(val)
  );

  return (
    <div className="p-2">
      <Form
        title={`${tituloModal}`}
        form={form}
        layout="vertical"
        onFinish={handleFinish}
        className="space-y-3"
        onValuesChange={(changedValues) => {
          if (entidade === "animaisDaClinica") {
            if('especie' in changedValues) setEspecieSelecionada(changedValues.especie);
            if('sexo' in changedValues) setSexoSelecionado(changedValues.sexo);
            if('gestacao' in changedValues) setGestacaoSelecionada(changedValues.gestacao);
            if('lactacao' in changedValues) setLactacaoSelecionada(changedValues.lactacao);
          }
        }}
      >
        <Typography.Title level={4}>
          {tituloModal || "Cadastro de Entidade"}
        </Typography.Title>
        {campos.map((campo) => (
          <Form.Item
            key={campo.name}
            name={campo.name}
            label={campo.label}
            rules={campo.rules || []}
          >
            {renderCampo(campo)}
          </Form.Item>
        ))}

        {entidade === "usuariosDoSistema" && (
          <>
            <Form.Item
              label="Perfis"
              name="perfis"
              rules={[
                {
                  required: true,
                  message: "Pelo menos um perfil deve ser selecionado",
                },
              ]}
            >
              <Checkbox.Group
                options={perfisSistema.map((perfil) => ({
                  ...perfil,
                  disabled:
                    (adminSelecionado &&
                      !["ADMIN", "INATIVO"].includes(perfil.value)) ||
                    (outrosSelecionados && perfil.value === "ADMIN"),
                }))}
                style={{ display: "flex", flexDirection: "column", gap: 8 }}
                value={perfisSelecionados}
                onChange={(values) => {
                  let newValues = values;

                  // Se selecionou ADMIN, mantém apenas ADMIN e INATIVO
                  if (values.includes("ADMIN")) {
                    newValues = values.filter(
                      (val) => val === "ADMIN" || val === "INATIVO"
                    );
                  }
                  // Se selecionou outros perfis, remove ADMIN se estiver presente
                  else if (
                    values.some((val) =>
                      [
                        "GESTOR",
                        "TECNICO",
                        "SECRETARIO",
                        "VETERINARIO",
                      ].includes(val)
                    )
                  ) {
                    newValues = values.filter((val) => val !== "ADMIN");
                  }

                  setPerfisSelecionados(newValues);
                  form.setFieldValue("perfis", newValues);
                }}
              />
            </Form.Item>

            {!adminSelecionado && (
              <>
                <Form.Item
                  label="Clínicas"
                  name="clinicaIds"
                  rules={[
                    {
                      required: true,
                      message: "Selecione pelo menos uma clínica",
                    },
                  ]}
                >
                  <Select
                    mode="multiple"
                    showSearch
                    placeholder="Selecione uma ou mais clínicas"
                    optionFilterProp="label"
                    filterOption={(input, option) =>
                      option.label.toLowerCase().includes(input.toLowerCase())
                    }
                    options={clinicas}
                  >
                    {clinicas.map((clinica) => (
                      <Select.Option key={clinica.value} value={clinica.value}>
                        {clinica.label}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>

                {/* <Alert
                  message="Atenção"
                  description="Após associar o usuário a uma ou mais clínicas, **não será possível remover essa associação.** Certifique-se antes de continuar."
                  type="warning"
                  showIcon
                  className="mb-4"
                /> */}
              </>
            )}
          </>
        )}

        {entidade === "usuariosDaClinica" && (
          <>
            <Form.Item
              label="Perfis"
              name="perfis"
              rules={[
                {
                  required: true,
                  message: "Pelo menos um perfil deve ser selecionado",
                },
              ]}
            >
              <Checkbox.Group
                options={perfisClinica.map((perfil) => ({
                  ...perfil,
                }))}
                style={{ display: "flex", flexDirection: "column", gap: 8 }}
                value={perfisSelecionados}
                onChange={(values) => {
                  let newValues = values;
                  setPerfisSelecionados(newValues);
                  form.setFieldValue("perfis", newValues);
                }}
              />
            </Form.Item>
          </>
        )}

        {entidade === "clinicas" && (
          <>
            {/* Switch Ativo/Inativo */}
            <Form.Item
              label="Status da Clínica"
              name="isAtiva"
              valuePropName="checked"
            >
              <Switch
                checkedChildren="ATIVA"
                unCheckedChildren="INATIVA"
                defaultUnChecked
              />
            </Form.Item>
          </>
        )}

        {entidade === "tutoresDaClinica" && (
          <>
            {/* Campo CNPJ com máscara */}
            <Form.Item label="CPF" name="cpf">
              <IMaskInput
                mask="000.000.000-00"
                definitions={{ 0: /[0-9]/ }}
                overwrite
                unmask={false}
                placeholder="000.000.000-00"
                style={{
                  width: "100%",
                  padding: "4px 11px",
                  border: "1px solid #d9d9d9",
                  borderRadius: "6px",
                  transition: "all 0.3s",
                }}
              />
            </Form.Item>

            {/* Campo Telefone com máscara dinâmica */}
            <Form.Item label="Telefone/Whatsapp" name="telefone1">
              <IMaskInput
                mask="(00) 90000-0000"
                definitions={{ 0: /[0-9]/ }}
                overwrite
                unmask={false}
                placeholder="(00) 90000-0000"
                style={{
                  width: "100%",
                  padding: "4px 11px",
                  border: "1px solid #d9d9d9",
                  borderRadius: "6px",
                  transition: "all 0.3s",
                }}
              />
            </Form.Item>

            {/* Campo WhatsApp com máscara */}
            {/* <Form.Item
              label="WhatsApp"
              name="whatsapp"
            >
              <IMaskInput
                mask="(00) 90000-0000"
                definitions={{ '0': /[0-9]/ }}
                overwrite
                unmask={false}
                placeholder="(00) 90000-0000"
                style={{
                  width: '100%',
                  padding: '4px 11px',
                  border: '1px solid #d9d9d9',
                  borderRadius: '6px',
                  transition: 'all 0.3s',
                }}
              />
            </Form.Item> */}

            {/* Campo CEP com máscara */}
            {/* <Form.Item
              label="CEP"
              name="cep"
              rules={[
                // { required: true, message: "CEP é obrigatório" },
                // {
                //   validator: (_, value) =>
                //     value && value.replace(/\D/g, '').length === 8
                //       ? Promise.resolve()
                //       : Promise.reject(new Error('CEP inválido'))
                // }
              ]}
            >
              <IMaskInput
                mask="00000-000"
                definitions={{ '0': /[0-9]/ }}
                overwrite
                unmask={false}
                placeholder="00000-000"
                style={{
                  width: '100%',
                  padding: '4px 11px',
                  border: '1px solid #d9d9d9',
                  borderRadius: '6px',
                  transition: 'all 0.3s',
                }}
              />
            </Form.Item> */}

            {/* Campo Estado */}
            {/* <Form.Item
              label="Estado"
              name="endEstado"
              // rules={[{ required: true, message: "Estado é obrigatório" }]}
            >
              <Select
                options={estadosBrasil}
                placeholder="Selecione o estado"
                disabled={form.getFieldValue('isAtiva') === false}
              />
            </Form.Item> */}
          </>
        )}

        {/* Campos condicionais específicos para animais */}
        {entidade === "animaisDaClinica" && sexoSelecionado === '2' && (
          <>
            {gestacaoSelecionada && (
              <Form.Item name="semanaGestacao" label="Semana de Gestação">
                <InputNumber min={1} max={12} style={{ width: "100%" }} />
              </Form.Item>
            )}
            
            {lactacaoSelecionada > 0 && (
              <Form.Item name="semanaLactacao" label="Semana de Lactação">
                <InputNumber min={1} style={{ width: "100%" }} />
              </Form.Item>
            )}
          </>
        )}
        
        {/* <Button onClick={() => {
          console.log(form.getFieldsValue());
        }}>
          Ver campos do formulário
        </Button> */}
        <div className="flex gap-4">
          <Button
            onClick={() => setModalVisivel(false)}
            className="w-1/2 mt-2"
            style={{ backgroundColor: "#d9d9d9", color: "#000" }}
          >
            Cancelar
          </Button>

          <Button
            type="primary"
            icon={<PlusOutlined />}
            loading={loading}
            className="w-1/2 mt-2 bg-[var(--verde-medio)] hover:bg-[var(--verde-realce)] text-white"
            onClick={() => form.submit()}
          >
            Cadastrar
          </Button>
        </div>
      </Form>
    </div>
  );
};

export default FormCadastrarEntidade;
