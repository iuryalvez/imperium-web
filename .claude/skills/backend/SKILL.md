---
name: backend
description: Use sempre que for criar, alterar, revisar ou refatorar o backend do Imperium (.NET 10, EF Core, PostgreSQL) — endpoints, controllers, services, repositories, DTOs, entidades, enums, mappers, migrations, policies ou o AppDbContext — mesmo que o pedido não diga "backend" (ex.: "cria o cadastro de vacinas", "adiciona um campo no animal").
---

# Backend do Imperium

Stack: ASP.NET Core / .NET 10, EF Core 10 + Npgsql (PostgreSQL), AutoMapper 16, JWT em cookie, Swagger.
Raiz: `imperium-web/backend/` (namespace raiz `backend`).

## 1. Onde cada coisa fica (padrão obrigatório)

```
backend/
├── Domain/<Dominio>/                 # bounded context: GestaoClinica, AtendimentoAnimal, SaudeAnimal, Usuarios, Auth
│   ├── Controllers/<Entidade>Controller.cs
│   ├── DTOs/<Entidade>/              # Cadastrar<Entidade>Dto, Editar<Entidade>Dto, Get<Entidade>Dto, Upsert...Dto
│   ├── Entities/
│   ├── Enums/
│   ├── Interfaces/                   # I<Entidade>Service, I<Entidade>Repository
│   ├── Mappers/                      # <Entidade>MappingProfile : Profile
│   ├── Repository/                   # <Entidade>Repository
│   ├── Services/                     # <Entidade>Service (regras de negócio)
│   └── Extensions/ | Attributes/     # só quando o domínio precisar
├── Domain/Utils/ServiceResult.cs              # retorno padrão dos services
├── Domain/Auth/Extensions/AuthExtension.cs    # User.GetClaimValue<T>, TemPerfil
├── Domain/General/Extensions/                 # EnumExtensions.GetDisplayName
├── Infra/Data/AppDbContext.cs                 # DbSets + OnModelCreating
├── Infra/Data/DatabaseSeeder.cs               # HasData (só valores estáticos!)
├── Middlewares/  Migrations/  Program.cs
```

Regras de pastas e namespaces (valem para todo código novo ou alterado):
- Interfaces ficam **sempre** em `Interfaces/` (plural). Nunca crie uma pasta `Interface/`.
- Controllers ficam **sempre** em `Controllers/` dentro do domínio, nunca soltos na raiz do domínio.
- O namespace espelha a pasta: `backend.Domain.<Dominio>.<Pasta>` (ex.: `backend.Domain.GestaoClinica.Controllers`, `backend.Domain.GestaoClinica.Interfaces`). Isso deixa o `using` previsível e evita classes "perdidas" em namespaces de outra pasta.
- Se encontrar código legado fora desse padrão no arquivo que você está mexendo, avise o usuário e sugira a correção em vez de copiar o padrão errado.

Antes de criar um domínio novo, confirme se a entidade não cabe num existente (pergunte se tiver dúvida).

## 2. Fluxo de uma requisição

`Controller` → `I<Entidade>Service` → `I<Entidade>Repository` → `AppDbContext`

### Controller
- `[ApiController]`, herda `ControllerBase`, injeta só interfaces de service (+ `ILogger<T>`).
- Rotas sempre sob `/api` (o Nginx do `https-server` só encaminha `/api/` e `/dev/api/`):
  - Sistema (admin): `[Route("api")]` + `[HttpGet("sistema/<entidades>")]`, policy `GerenciaSistema`.
  - Clínica (tenant): `[Route("api/clinica")]` ou `[Route("api/clinica/<entidades>")]`.
  - Verbos usados: `GET <entidades>` (lista paginada), `GET <entidades>/opcoes?busca=` (selects), `GET <entidades>/{id}`, `POST <entidades>/cadastrar`, `PUT <entidades>/editar` (o `Id` vai no corpo), `DELETE <entidades>/{id}`, `PUT <entidades>/{id}/status?ativo=true|false`.
  - A mesma entidade pode ter as duas visões em controllers separados no mesmo domínio: `<Entidade>Controller` (`api/clinica/<entidades>`, policies de clínica) e `<Entidade>SistemaController` (`api/sistema/<entidades>`, `GerenciaSistema`). Nunca sirva tela de clínica por rota `sistema/` nem o contrário (ex.: `RacaController` x `RacaSistemaController`).
- Autorização com policies de `Program.cs`: `VisualizaTudo`, `GerenciaClinica`, `GerenciaAnimais`, `GerenciaTutores`, `GerenciaConsultas`, `GerenciaSistema`, `AcessoLoginPolicy`, `AcessoLoginOuTodosUsuarios`. Nova policy → adicionar em `AddAuthorization` usando `TemPerfil(...)`.
- **Multi-tenant:** em rotas de clínica, o `clinicaId` vem **sempre** do token: `var clinicaId = User.GetClaimValue<int>("clinicaId"); if (clinicaId <= 0) return Forbid();` e é passado ao service. Nunca aceite `clinicaId` vindo do body/query.
- Resposta: `if (!result.Succeeded) return BadRequest(result);` / `NotFound(...)`; sucesso com `Ok(result)` / `Ok(result.Data)` / `CreatedAtAction` / `NoContent()`. Mantenha o formato que o frontend já consome naquela entidade.
- Sem regra de negócio e sem `AppDbContext` no controller (há legado assim em `UsuarioController`: não copie).

### Service
- Implementa `I<Entidade>Service`; recebe repositórios, `IMapper`, `ILogger<T>` (e `AppDbContext` só para transações).
- Retorna `ServiceResult { Succeeded, Message, Data, Errors }` em operações de escrita; leituras simples podem retornar DTO.
- Validações de negócio (unicidade, vínculo com a clínica, dependentes antes de excluir) ficam aqui, com mensagens em português.
- Métodos assíncronos com sufixo `Async`; `XML doc` (`/// <summary>`) em métodos públicos não triviais; `#region` opcional para agrupar.
- Operações em várias tabelas: `await using var transaction = await _context.Database.BeginTransactionAsync();` + `CommitAsync`/`RollbackAsync`.

### Repository
- Acesso a dados puro: `GetByIdAsync`, `GetAllAsync`, `GetByIdWithDetailsAsync` (com `Include`), `ExistsAsync`/`...ExistsForAnother...Async`, `AddAsync`, `Delete`, `SaveChangesAsync`.
- Leituras com `AsNoTracking()`; dashboards/contagens projetam direto para DTO com `Select`.
- Consultas de tenant filtram direto por `ClinicaId` da entidade (`t.ClinicaId == clinicaId`). Usuário é a exceção: é global e chega à clínica pelo vínculo `UsuarioPerfilClinica`.
- Listagens vêm **mais recentes primeiro** (`OrderByDescending(DataCadastro).ThenByDescending(Id)`), salvo outra ordenação pedida pelo filtro.

### DTOs
- `Cadastrar<Entidade>Dto`, `Editar<Entidade>Dto` (Sistema admin: `Editar<Entidade>AdminDto`), `Get<Entidade>Dto`, `<Entidade>InfoSimplesDto`.
- DataAnnotations com mensagens em pt-BR (`[Required(ErrorMessage = "...")]`, `[StringLength(...)]`).
- Documentos e telefones guardados só com dígitos (CPF 11, CNPJ 14, CEP 8, telefone 11).

### Mappers
- `<Entidade>MappingProfile : Profile` em `Mappers/`. DTOs de edição terminam o mapa com `.IgnorarNulosDaOrigem();` (`Domain/Utils/MappingExtensions.cs`): campo nulo no DTO mantém o valor atual.
- Não use `.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null))`: com `int?`/`bool?`/enum/`DateTime?` nulo, o valor chega convertido para 0/false/MinValue e sobrescreve a entidade (bug já visto em animal e ração).
- Campos que nunca vêm do DTO (`ClinicaId`, `IsAtivo`, `DataCadastro`, `Id` na edição) ficam com `.ForMember(..., opt => opt.Ignore())`.
- Registro já é automático: `AddAutoMapper(config => { }, typeof(Program).Assembly)`.

### Entities e Enums
- `[Key]` + `[DatabaseGenerated(Identity)] int Id`, navegações `virtual ICollection<>?`.
- **Entidade do tenant pertence a uma clínica**: `int ClinicaId` obrigatório + `Clinica?` (ex.: `Tutor`, `Animal`, `Consulta`). Unicidade é por clínica (ex.: índice único `(ClinicaId, Cpf)` no tutor).
- N:N só com entidade de vínculo explícita: `UsuarioPerfilClinica`, `TutorAnimal`, `RacaOcultaClinica`, `RacaoOcultaClinica`. Não crie vínculo Entidade×Clínica para algo que pertence a uma clínica só.
- Cadastros que podem sair de uso têm `bool IsAtivo` (padrão `true`), `DateTime? DataDesativacao` e `DateTime DataCadastro`.
- Enums começam em 1 e podem ter `[Display(Name = "...")]` (use `GetDisplayName()`).

## 3. Regras de dados (padrão do projeto)

### Ativo/inativo e exclusão
- Desativar é o normal; excluir só quando o registro **não está em uso** (o service confere antes e responde "... está em uso ... Desative-o."). Relacionamentos de histórico usam `DeleteBehavior.Restrict`, nunca `Cascade` que apague dados de outra entidade (ex.: `Racao → Dieta`).
- Inativo continua aparecendo nas listagens (com tag/filtro) e no histórico, mas o **service recusa vínculo ou registro novo** com ele (ex.: consulta nova com animal inativo). Ao editar, só valide os IDs **novos**: manter o que já estava vinculado é sempre permitido.

### Catálogo do sistema x itens da clínica (Raça, Ração)
- `ClinicaId` nulo = item do sistema, compartilhado; preenchido = item próprio da clínica.
- A clínica vê sistema + próprios, nunca os de outra clínica (`GetDisponivel...(id, clinicaId)` responde "não encontrado").
- Clínica editando item do sistema → cria **cópia** própria com as alterações. Clínica não exclui nem desativa item do sistema; pode **ocultá-lo só para ela** (`PUT {id}/ocultar?oculta=`, tabela `<Entidade>OcultaClinica`).
- Admin (`api/sistema/<entidades>`) mantém o catálogo (CRUD + status) e vê as cópias das clínicas em `GET sistema/<entidades>/copias`.
- Item oculto ou inativo não pode ser escolhido em registro novo (`ValidarSelecionavelAsync` / `ObterSelecionavelAsync`).

### Paginação, filtros e selects (`Domain/Utils/Paginacao/`)
- Listas paginam no banco: o controller recebe `[FromQuery] <Entidade>Filtro` (herda `FiltroBase`: `Pagina`, `TamanhoPagina` padrão 10 e máximo 100, `Busca`, `OrdenarPor`, `Ordem` `asc|desc`) e devolve `PaginaResultado<T>` = `{ itens, total, pagina, tamanhoPagina }`.
- No repository: filtros com `EF.Functions.ILike(..., QueryablePaginacaoExtensions.PadraoBusca(busca))`, depois `.Ordenar(filtro, mapaDeColunas, "dataCadastro", x => x.Id)` (lista branca de colunas) e `.PaginarAsync(filtro)`. O service converte com `pagina.Converter(...)` para DTO.
- Selects usam `GET <entidades>/opcoes?busca=`: até `OpcaoDto.LimiteOpcoes` (20) itens `{ id, nome, isAtivo }`, ativos primeiro e depois por nome; inativos vêm junto (o frontend mostra desabilitado), ocultos pela clínica não vêm.
- Listas que já seguem o contrato: clínica (animais, tutores, consultas, usuários, raças, rações) e sistema (clínicas, raças, rações e cópias). Única pendência: `GET sistema/usuarios` (junta vínculos e admins; fica para depois).
- `/opcoes` existentes: `clinica/animais`, `clinica/tutores`, `clinica/racas?especie=`, `clinica/racoes?especie=`, `sistema/clinicas`.
- Coleções no `Include` de lista paginada usam `.AsSplitQuery()` para o `Skip/Take` não multiplicar linhas.

## 4. Registrar e migrar

1. `Program.cs`: `builder.Services.AddScoped<I<Entidade>Service, <Entidade>Service>();` e o mesmo para o repository, junto do bloco do domínio.
2. `Infra/Data/AppDbContext.cs`: `public DbSet<<Entidade>> <Entidades> { get; set; }` e configurações em `OnModelCreating` (chaves compostas, deletes, conversões).
3. Migration (sempre pelo Makefile, que carrega o `.env`; só com permissão do usuário):
   ```bash
   make dev-add-migration      # pede o nome em PascalCase
   make dev-database-update
   ```
   Revise o arquivo gerado antes de aplicar. Seeds (`HasData`) só com valores estáticos (EF 10 rejeita `DateTime.UtcNow`, `Guid.NewGuid()`, hashes com salt).
4. `dotnet build` sem erros novos.

## 5. Checklist final
- [ ] Arquivos no domínio certo, com `Controllers/` e `Interfaces/` e namespace igual à pasta.
- [ ] Controller fino, policy correta, `clinicaId` do token.
- [ ] Service com `ServiceResult` e mensagens em pt-BR.
- [ ] Entidade do tenant com `ClinicaId` direto; nada de outra clínica é lido ou alterado.
- [ ] Status por `PUT {id}/status?ativo=`; excluir bloqueado se em uso; inativo recusado em registro novo.
- [ ] Lista nova paginada (`FiltroBase`/`PaginaResultado`), mais recentes primeiro; select por `/opcoes`.
- [ ] DI em `Program.cs`, `DbSet` e migration revisada.
- [ ] Rota confere com `frontend/src/services/Endpoints.js` (ver skill `frontend`).
- [ ] Nada alterado em `bris-web` ou `https-server`; rotas novas continuam sob `/api/`.
