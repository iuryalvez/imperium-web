import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { CpfMaskInput, TelefoneMaskInput, CepMaskInput, CnpjMaskInput } from "./masks";

// Os inputs com máscara mostram o valor formatado, mas entregam ao formulário só os dígitos (unmask)
describe("inputs com máscara", () => {
  test("CPF mostra a máscara e envia só os dígitos", () => {
    const onChange = jest.fn();
    render(<CpfMaskInput onChange={onChange} />);
    const input = screen.getByPlaceholderText("000.000.000-00");

    userEvent.type(input, "52998224725");

    expect(input).toHaveValue("529.982.247-25");
    expect(onChange).toHaveBeenLastCalledWith("52998224725");
  });

  test("CPF ignora letras e passa do limite", () => {
    const onChange = jest.fn();
    render(<CpfMaskInput onChange={onChange} />);
    const input = screen.getByPlaceholderText("000.000.000-00");

    userEvent.type(input, "529a98224725999");

    expect(input).toHaveValue("529.982.247-25");
    expect(onChange).toHaveBeenLastCalledWith("52998224725");
  });

  test("telefone formata celular", () => {
    const onChange = jest.fn();
    render(<TelefoneMaskInput onChange={onChange} />);
    const input = screen.getByPlaceholderText("(00) 99999-9999");

    userEvent.type(input, "62999990000");

    expect(input).toHaveValue("(62) 99999-0000");
    expect(onChange).toHaveBeenLastCalledWith("62999990000");
  });

  test("CEP e CNPJ entregam só os dígitos", () => {
    const onCep = jest.fn();
    const onCnpj = jest.fn();
    render(
      <>
        <CepMaskInput onChange={onCep} />
        <CnpjMaskInput onChange={onCnpj} />
      </>
    );

    userEvent.type(screen.getByPlaceholderText("00000-000"), "74000000");
    userEvent.type(screen.getByPlaceholderText("00.000.000/0000-00"), "11222333000181");

    expect(onCep).toHaveBeenLastCalledWith("74000000");
    expect(onCnpj).toHaveBeenLastCalledWith("11222333000181");
  });

  test("CPF desabilitado não aceita digitação", () => {
    const onChange = jest.fn();
    render(<CpfMaskInput onChange={onChange} disabled />);

    expect(screen.getByPlaceholderText("000.000.000-00")).toBeDisabled();
  });
});
