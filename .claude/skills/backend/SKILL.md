---
name: backend
description: Use sempre que for criar, alterar, revisar ou refatorar o backend do Imperium (.NET 10, EF Core, PostgreSQL) — endpoints, controllers, services, repositories, DTOs, entidades, enums, mappers, migrations, policies ou o AppDbContext — mesmo que o pedido não diga "backend" (ex.: "cria o cadastro de vacinas", "adiciona um campo no animal").
---

# Backend do Imperium

Antes de começar, leia a skill `contexto` (o que é o sistema e o que nunca pode quebrar).

Stack: ASP.NET Core em .NET 10, EF Core 10 com Npgsql (PostgreSQL), AutoMapper 16, JWT em cookie e Swagger.
A raiz é `imperium-web/backend/` e o namespace raiz é `backend`.

## 1. Onde cada coisa fica

Cada domínio (bounded context) fica em `Domain/<Dominio>/`. Os domínios atuais são `AtendimentoAnimal`, `SaudeAnimal`, `GestaoClinica`, `Usuarios` e `Auth`, além de `General` e `Utils` para código compartilhado.

Dentro de um domínio, as pastas são sempre estas:

- `Controllers/` com `<Entidade>Controller.cs` (e `<Entidade>SistemaController.cs` quando houver visão do admin).
- `DTOs/<Entidade>/` com `Cadastrar<Entidade>Dto`, `Editar<Entidade>Dto`, `Get<Entidade>Dto` e outros DTOs da entidade.
- `Entities/` com as entidades do EF.
- `Enums/` com os enums do domínio.
- `Interfaces/` com `I<Entidade>Service` e `I<Entidade>Repository`.
- `Mappers/` com `<Entidade>MappingProfile`.
- `Repository/` com `<Entidade>Repository`.
- `Services/` com `<Entidade>Service`, onde ficam as regras de negócio.
- `Extensions/` ou `Attributes/` só quando o domínio precisar.

Arquivos compartilhados que você vai usar com frequência:

- `Domain/Utils/ServiceResult.cs`: retorno padrão dos services.
- `Domain/Utils/MappingExtensions.cs`: `IgnorarNulosDaOrigem()` para mapas de edição.
- `Domain/Utils/Paginacao/`: `FiltroBase`, `PaginaResultado`, `OpcaoDto` e `QueryablePaginacaoExtensions`.
- `Domain/Auth/Extensions/AuthExtension.cs`: `User.GetClaimValue<T>`, `TemPerfil` e `TemAlgumPerfil`.
- `Domain/General/Extensions/`: `GetDisplayName()` para enums.
- `Infra/Data/AppDbContext.cs`: DbSets e `OnModelCreating`.
- `Infra/Data/DatabaseSeeder.cs`: seeds com `HasData`, só com valores estáticos.
- `Program.cs`: injeção de dependência, autenticação e policies.

Regras de pastas e namespaces, para todo código novo ou alterado:

- Interfaces ficam sempre em `Interfaces/` (plural). Nunca crie uma pasta `Interface/`.
- Controllers ficam sempre em `Controllers/` dentro do domínio, nunca soltos na raiz do domínio.
- O namespace espelha a pasta: `backend.Domain.<Dominio>.<Pasta>`. Assim o `using` fica previsível.
- Existe legado fora do padrão: `backend/Services/ValidateService.cs` e `backend/Interfaces/IValidateService.cs` na raiz do projeto. Não copie esse padrão. Se mexer num arquivo fora do padrão, avise o usuário e sugira a correção.

Antes de criar um domínio novo, confirme se a entidade não cabe num existente. Na dúvida, pergunte.

## 2. Fluxo de uma requisição

A requisição passa por `Controller`, depois `I<Entidade>Service`, depois `I<Entidade>Repository` e chega ao `AppDbContext`. Cada camada só conhece a de baixo.

### Controller

- Usa `[ApiController]`, herda `ControllerBase` e injeta só interfaces de service (mais `ILogger<T>`).
- Rotas sempre sob `/api`, porque o Nginx do `https-server` só encaminha `/api/` e `/dev/api/`.
- Rotas do Sistema (admin) usam `[Route("api")]` com `[HttpGet("sistema/<entidades>")]` e a policy `GerenciaSistema`.
- Rotas da Clínica (tenant) usam `[Route("api/clinica/<entidades>")]` (ou `[Route("api/clinica")]` em controllers antigos).
- As rotas padrão de um CRUD são: `GET <entidades>` para a lista paginada, `GET <entidades>/opcoes?busca=` para selects, `GET <entidades>/{id}` para detalhes, `POST <entidades>/cadastrar`, `PUT <entidades>/editar` (o `Id` vai no corpo), `DELETE <entidades>/{id}` e `PUT <entidades>/{id}/status?ativo=true|false`.
- Ação de negócio que não é CRUD nem ativo/inativo usa `PUT <entidades>/{id}/<acao>` com um DTO próprio no corpo quando precisar (exemplo vivo: `PUT consultas/{id}/cancelar` com `CancelarConsultaDto { Motivo? }` e `PUT consultas/{id}/reabrir`). As regras ficam no service, que responde `ServiceResult`.
- Quando a mesma entidade tem visão da clínica e do admin, use dois controllers no mesmo domínio: `<Entidade>Controller` em `api/clinica/<entidades>` e `<Entidade>SistemaController` em `api/sistema/<entidades>`. Nunca sirva tela de clínica por rota `sistema/` nem o contrário. Exemplo vivo: `RacaController` e `RacaSistemaController`.
- Autorização é feita com as policies de `Program.cs` (perfis entre parênteses):
  - `VisualizaTudo` (GESTOR, VETERINARIO, SECRETARIO, TECNICO): leitura de animais, raças, rações, acompanhamento, `tutores/opcoes` e Meu Painel.
  - `VisualizaAtendimento` (GESTOR, VETERINARIO, SECRETARIO): leitura de tutores, consultas e avaliações.
  - `GerenciaClinica` (GESTOR): dashboard, dados e usuários da clínica.
  - `GerenciaAnimais` (GESTOR, VETERINARIO, TECNICO): escrita de animais e raças.
  - `GerenciaRacoes` (GESTOR, VETERINARIO, TECNICO): escrita de rações.
  - `GerenciaAcompanhamento` (GESTOR, VETERINARIO): escrita de avaliações e dietas.
  - `GerenciaTutores` e `GerenciaConsultas` (GESTOR, VETERINARIO, SECRETARIO).
  - `GerenciaSistema` (claim `isAdmin`), `AcessoLoginPolicy` e `AcessoLoginOuTodosUsuarios`.
- `TemPerfil(...)` olha só a claim `Perfis`; não coloque `"isAdmin"` nas policies da clínica (o admin simulando acesso tem só os perfis escolhidos). Uma policy nova entra em `AddAuthorization` com `TemPerfil(...)` e no espelho do frontend (`src/utils/permissoes.js`).
- Dados pessoais de terceiros por perfil: quando um perfil pode ver a entidade mas não todos os dados (TECNICO vê só o nome do tutor), o controller passa um booleano ao service (ex.: `User.PodeVerDadosDoTutor()`) e o service omite os campos. Não crie DTO paralelo.
- Multi-tenant: em rotas de clínica o `clinicaId` vem sempre do token, com `var clinicaId = User.GetClaimValue<int>("clinicaId"); if (clinicaId <= 0) return Forbid();`, e é passado ao service. Nunca aceite `clinicaId` do body ou da query.
- Para gravar quem fez a ação use `User.GetUsuarioId()` (claim `usuarioId`, o id do usuário). Nunca use o `NameIdentifier` do token de vínculo como id de usuário: ele é o id do vínculo.
- Respostas de falha: sempre `if (!result.Succeeded) return this.Falha(result);` (`Domain/Utils/ServiceResultHttpExtensions.cs`). O status sai do tipo da falha que o service marcou, e o corpo é sempre o `ServiceResult`:
  - 404: registro da rota inexistente ou de outra clínica (`ServiceResult.NaoEncontrado(msg)`).
  - 409: conflito com dado existente, como duplicado (CPF, e-mail, CNPJ) ou item em uso que bloqueia a exclusão (`ServiceResult.Conflito(msg)`).
  - 400: as demais regras e validações (`ServiceResult.Invalido(msg)` ou `new ServiceResult { Succeeded = false, ... }`). Um id inválido que vem no corpo (raça do animal, ração da dieta, tutor da consulta) é 400, não 404: 404 é só para o registro da própria rota.
  - Nunca escolha o status comparando a mensagem (`Message.Contains("não encontrado")`).
- Respostas de sucesso: `Ok(result)`, `Ok(result.Data)`, `CreatedAtAction` ou `NoContent()`. Mantenha o formato que o frontend já consome naquela entidade.
- Sem regra de negócio e sem `AppDbContext` no controller. Há legado assim em `UsuarioController`: não copie.

### Service

- Implementa `I<Entidade>Service` e recebe repositórios, `IMapper` e `ILogger<T>`. Recebe `AppDbContext` só para transações.
- Retorna `ServiceResult` (`Succeeded`, `Message`, `Data`, `Errors`) nas operações de escrita. Leituras simples podem devolver o DTO.
- Falhas de não encontrado e de conflito usam os atalhos `ServiceResult.NaoEncontrado(msg)` e `ServiceResult.Conflito(msg)`, que preenchem `Falha` (`TipoFalha`, com `[JsonIgnore]`, então não muda o JSON). É isso que o controller usa para responder 404 ou 409.
- Validações de negócio ficam aqui, com mensagens em português: unicidade, vínculo com a clínica, item inativo ou oculto, dependentes antes de excluir.
- Métodos assíncronos terminam em `Async`. Métodos públicos não triviais têm `/// <summary>`. `#region` é opcional.
- Operações em várias tabelas usam `await using var transaction = await _context.Database.BeginTransactionAsync();` com `CommitAsync` e `RollbackAsync`.

### Repository

- Só acesso a dados: `GetByIdAsync`, `GetAllAsync`, `GetByIdWithDetailsAsync` (com `Include`), `ExistsAsync` ou `...ExistsForAnother...Async`, `AddAsync`, `Delete` e `SaveChangesAsync`.
- Leituras usam `AsNoTracking()`. Dashboards e contagens projetam direto para DTO com `Select`.
- Consultas de tenant filtram direto pelo `ClinicaId` da entidade (`t.ClinicaId == clinicaId`). Usuário é a exceção: é global e chega à clínica pelo vínculo `UsuarioPerfilClinica`.
- Listagens vêm com os mais recentes primeiro (`OrderByDescending(DataCadastro).ThenByDescending(Id)`), salvo outra ordenação pedida pelo filtro.

### DTOs

- Nomes: `Cadastrar<Entidade>Dto`, `Editar<Entidade>Dto` (no admin, `Editar<Entidade>AdminDto`), `Get<Entidade>Dto` e `<Entidade>InfoSimplesDto`.
- Nenhum DTO de clínica tem `ClinicaId` vindo do cliente.
- DataAnnotations com mensagens em português, como `[Required(ErrorMessage = "...")]` e `[StringLength(...)]`.
- Documentos e telefones são guardados só com dígitos: CPF com 11, CNPJ com 14, CEP com 8 e telefone com até 11.

### Mappers

- Cada entidade tem `<Entidade>MappingProfile : Profile` em `Mappers/`. O registro é automático (`AddAutoMapper(..., typeof(Program).Assembly)`).
- Mapas de edição terminam com `.IgnorarNulosDaOrigem();`: campo nulo no DTO mantém o valor atual. É isso que permite edição parcial sem zerar campos.
- Nos DTOs de edição, `bool`, `int`, enum e `DateTime` são sempre anuláveis (`bool?`). Um `bool` não-nulo chega como `false` quando o campo não vem e sobrescreve a entidade (já aconteceu com `IsPorcentagem` da ração e `IsAtiva` da clínica).
- Ao copiar um item do sistema para a clínica, a cópia parte dos dados do original e só depois recebe o DTO (`_mapper.Map(dto, copia)`); assim a cópia não perde o que não veio no DTO.
- Não use `.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null))`. Com `int?`, `bool?`, enum ou `DateTime?` nulo, o valor chega como 0, false ou MinValue e sobrescreve a entidade (bug já visto em animal e ração).
- Campos que nunca vêm do DTO (`ClinicaId`, `IsAtivo`, `DataCadastro`, `DataDesativacao` e `Id` na edição) ficam com `.ForMember(..., opt => opt.Ignore())`.

### Entities e Enums

- Chave com `[Key]` e `[DatabaseGenerated(DatabaseGeneratedOption.Identity)] int Id`. Navegações como `virtual ICollection<>?`.
- Entidade do tenant pertence a uma clínica: tem `int ClinicaId` obrigatório e `Clinica?` (exemplos: `Tutor`, `Animal`, `Consulta`). Unicidade é por clínica, como o índice único `(ClinicaId, Cpf)` do tutor.
- Vínculo explícito só para relação N:N: `UsuarioPerfilClinica`, `TutorAnimal`, `RacaOcultaClinica` e `RacaoOcultaClinica`. Não crie tabela de vínculo Entidade×Clínica para algo que pertence a uma clínica só.
- Cadastros que podem sair de uso têm `bool IsAtivo` (padrão `true`), `DateTime? DataDesativacao` e `DateTime DataCadastro`.
- Enums começam em 1 e podem ter `[Display(Name = "...")]`, lido com `GetDisplayName()`.

## 3. Regras de dados

### Ativo, inativo e exclusão

- Desativar é o caminho normal. Excluir só quando o registro não está em uso: o service confere antes e responde algo como "... está em uso ... Desative-o.".
- Relacionamentos de histórico usam `DeleteBehavior.Restrict`, nunca `Cascade` que apague dados de outra entidade (exemplo: `Racao` para `Dieta`).
- Inativo continua aparecendo nas listagens (com tag e filtro) e no histórico, mas o service recusa vínculo ou registro novo com ele (exemplo: consulta nova com animal inativo). Na edição, só valide os IDs novos: manter o que já estava vinculado é sempre permitido.

### Catálogo do sistema e itens da clínica (Raça e Ração)

- `ClinicaId` nulo significa item do sistema, compartilhado. Preenchido significa item próprio da clínica.
- A clínica vê os itens do sistema mais os próprios, nunca os de outra clínica. Pedir um item de outra clínica responde "não encontrado" (`GetDisponivel...(id, clinicaId)`).
- Quando a clínica edita um item do sistema, o service cria uma cópia própria da clínica com as alterações.
- A clínica não exclui nem desativa item do sistema. Ela pode ocultá-lo só para ela com `PUT {id}/ocultar?oculta=`, gravando em `<Entidade>OcultaClinica`.
- O admin mantém o catálogo em `api/sistema/<entidades>` (CRUD e status) e vê as cópias das clínicas em `GET sistema/<entidades>/copias`.
- Item oculto ou inativo não pode ser escolhido em registro novo (`ValidarSelecionavelAsync` e `ObterSelecionavelAsync`).

### Paginação, filtros e selects

- Listas paginam no banco. O controller recebe `[FromQuery] <Entidade>Filtro`, que herda `FiltroBase` (`Pagina`, `TamanhoPagina` com padrão 10 e máximo 100, `Busca`, `OrdenarPor` e `Ordem` `asc` ou `desc`), e devolve `PaginaResultado<T>` no formato `{ itens, total, pagina, tamanhoPagina }`.
- No repository, a busca usa `EF.Functions.ILike(..., QueryablePaginacaoExtensions.PadraoBusca(busca))`; depois vem `.Ordenar(filtro, mapaDeColunas, "dataCadastro", x => x.Id)`, com lista branca de colunas, e `.PaginarAsync(filtro)`. O service converte para DTO com `pagina.Converter(...)`.
- Coleções no `Include` de lista paginada usam `.AsSplitQuery()` para o `Skip/Take` não multiplicar linhas.
- Selects usam `GET <entidades>/opcoes?busca=`: devolvem até `OpcaoDto.LimiteOpcoes` (20) itens `{ id, nome, isAtivo }`, ativos primeiro e depois por nome. Inativos vêm junto (o frontend mostra desabilitado); ocultos pela clínica não vêm.
- Já seguem o contrato as listas da clínica (animais, tutores, consultas, usuários, raças e rações) e do sistema (clínicas, raças, rações e cópias). A única pendência é `GET sistema/usuarios`.
- Rotas `/opcoes` existentes: `clinica/animais`, `clinica/tutores`, `clinica/racas?especie=`, `clinica/racoes?especie=` e `sistema/clinicas`.

## 4. Registrar e migrar

1. Em `Program.cs`, registre service e repository com `builder.Services.AddScoped<I<Entidade>Service, <Entidade>Service>();`, junto do bloco do domínio.
2. Em `Infra/Data/AppDbContext.cs`, adicione o `DbSet` e as configurações em `OnModelCreating` (índices únicos, chaves compostas, comportamento de delete, conversões).
3. Rode `cd backend && dotnet build` e confirme 0 erros e nenhum warning novo.
4. Migration só com permissão do usuário. Diga que o modelo mudou, o que muda no banco e o nome sugerido, e espere o sim. Depois use sempre o Makefile, que carrega o `.env`: `make dev-add-migration` (pede o nome em PascalCase) e, com nova permissão, `make dev-database-update`. Nunca rode `dotnet ef` direto.
5. Revise o arquivo gerado antes de aplicar: confira se não há `DropColumn` ou `DropTable` inesperado. Seeds com `HasData` só com valores estáticos, porque o EF 10 rejeita `DateTime.UtcNow`, `Guid.NewGuid()` e hash com salt.

## 5. Passo a passo: CRUD de entidade nova

Use Raça (`RacaController`, `RacaSistemaController`, `RacaService`, `RacaRepository`) e Ração (`RacaoController`, `RacaoSistemaController`) como modelo vivo. Para uma entidade só da clínica, `Tutor` é o exemplo mais simples.

1. Escolha o domínio. Veja em qual pasta de `Domain/` a entidade cabe. Se não estiver claro, pergunte antes de criar arquivos.
2. Crie a entidade em `Entities/` com `Id`, `int ClinicaId` e `Clinica?` (ou `int? ClinicaId` se for catálogo do sistema), `bool IsAtivo = true`, `DateTime? DataDesativacao`, `DateTime DataCadastro` e os campos do negócio.
3. Crie os enums em `Enums/`, começando em 1, com `[Display(Name = "...")]` quando forem exibidos.
4. No `AppDbContext`, adicione o `DbSet` e configure índices únicos por clínica, relacionamentos e `DeleteBehavior.Restrict` onde houver histórico.
5. Crie os DTOs em `DTOs/<Entidade>/`: `Cadastrar`, `Editar` (com `Id`) e `Get`, sem `ClinicaId`, com mensagens de validação em português. Crie também o `<Entidade>Filtro : FiltroBase` se a lista tiver filtros próprios.
6. Crie o mapper. O mapa de edição ignora `Id`, `ClinicaId`, `IsAtivo`, `DataCadastro` e `DataDesativacao` e termina com `.IgnorarNulosDaOrigem()`, para que a edição parcial não zere campos.
7. Crie as interfaces `I<Entidade>Service` e `I<Entidade>Repository` em `Interfaces/`.
8. Crie o repository: toda consulta filtra por `ClinicaId`, leituras com `AsNoTracking()`, lista paginada com `ILike`, `Ordenar` e `PaginarAsync`, mais recentes primeiro, e o método de opções para `/opcoes`.
9. Crie o service: valida unicidade, vínculos e itens inativos; ao desativar grava `DataDesativacao`; antes de excluir confere se está em uso; devolve `ServiceResult` com mensagens em português.
10. Crie o controller fino: lê o `clinicaId` do token, aplica a policy certa e expõe as rotas padrão (lista, `opcoes`, `{id}`, `cadastrar`, `editar`, `{id}` no DELETE e `{id}/status`).
11. Registre service e repository em `Program.cs` com `AddScoped`.
12. Rode `dotnet build` com 0 erros e sem warnings novos.
13. Peça permissão para criar a migration (seção 4).
14. Avise que o frontend precisa das rotas novas em `frontend/src/services/Endpoints.js` (ou faça isso, se a tarefa incluir o frontend).
15. Cubra a entidade com testes de integração em `backend.Tests/<Entidade>/` (skill `testes`): caminho feliz, isolamento entre clínicas, perfis, validação e regras. Rode `make test`.

## 6. Quando não houver exemplo no projeto

Se a tarefa pede algo que o projeto ainda não faz (upload de arquivo, envio agendado, relatório em PDF, cache etc.):

- Siga a prática de mercado da stack (.NET 10, EF Core, ASP.NET Core) e a documentação oficial.
- Mantenha a estrutura do projeto: Controller fino, Service com as regras, Repository com o acesso a dados, `ServiceResult` e as regras de tenant (`clinicaId` do token, filtro por `ClinicaId`).
- Explique ao usuário, em poucas linhas, a abordagem e por que ela foi escolhida, e espere aprovação antes de tratá-la como padrão do projeto. Se for aprovada, sugira atualizar esta skill.

## 7. Checklist final

- Arquivos no domínio certo, dentro de `Controllers/` e `Interfaces/`, com namespace igual à pasta.
- Controller fino, policy correta e `clinicaId` lido do token.
- Service com `ServiceResult` e mensagens em português.
- Entidade do tenant com `ClinicaId` direto; nada de outra clínica é lido ou alterado.
- Status por `PUT {id}/status?ativo=`; exclusão bloqueada se em uso; inativo recusado em registro novo.
- Lista nova paginada (`FiltroBase` e `PaginaResultado`), mais recentes primeiro, e select por `/opcoes`.
- Edição parcial não zera campos (`IgnorarNulosDaOrigem`).
- DI em `Program.cs` e `DbSet` no `AppDbContext`.
- Falhas com `this.Falha(result)`: 404 não encontrado, 409 conflito, 400 regra/validação.
- `dotnet build` com 0 erros e sem warnings novos, e `make test` passando.
- Se o modelo mudou, migration pedida ao usuário (não criada sem permissão).
- Rotas conferidas com `frontend/src/services/Endpoints.js` (skill `frontend`).
- Nada alterado em `bris-web` ou `https-server`; rotas novas continuam sob `/api/`.
