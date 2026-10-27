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
  - Verbos usados: `GET <entidades>`, `GET <entidades>/{id}`, `POST <entidades>/cadastrar`, `PUT <entidades>/editar`, `DELETE <entidades>/{id}`, `PUT <entidades>/{id}/status`.
- Autorização com policies de `Program.cs`: `VisualizaTudo`, `GerenciaClinica`, `GerenciaAnimais`, `GerenciaTutores`, `GerenciaConsultas`, `GerenciaSistema`, `AcessoLoginPolicy`, `AcessoLoginOuTodosUsuarios`. Nova policy → adicionar em `AddAuthorization` usando `TemPerfil(...)`.
- **Multi-tenant:** em rotas de clínica, o `clinicaId` vem **sempre** do token: `var clinicaId = User.GetClaimValue<int>("clinicaId"); if (clinicaId <= 0) return Forbid();` e é passado ao service. Nunca aceite `clinicaId` vindo do body/query.
- Resposta: `if (!result.Succeeded) return BadRequest(result);` / `NotFound(...)`; sucesso com `Ok(result)` / `Ok(result.Data)` / `CreatedAtAction` / `NoContent()`. Mantenha o formato que o frontend já consome naquela entidade.
- Sem regra de negócio e sem `AppDbContext` no controller (há legado assim em `AnimalController`: não copie).

### Service
- Implementa `I<Entidade>Service`; recebe repositórios, `IMapper`, `ILogger<T>` (e `AppDbContext` só para transações).
- Retorna `ServiceResult { Succeeded, Message, Data, Errors }` em operações de escrita; leituras simples podem retornar DTO.
- Validações de negócio (unicidade, vínculo com a clínica, dependentes antes de excluir) ficam aqui, com mensagens em português.
- Métodos assíncronos com sufixo `Async`; `XML doc` (`/// <summary>`) em métodos públicos não triviais; `#region` opcional para agrupar.
- Operações em várias tabelas: `await using var transaction = await _context.Database.BeginTransactionAsync();` + `CommitAsync`/`RollbackAsync`.

### Repository
- Acesso a dados puro: `GetByIdAsync`, `GetAllAsync`, `GetByIdWithDetailsAsync` (com `Include`), `ExistsAsync`/`...ExistsForAnother...Async`, `AddAsync`, `Delete`, `SaveChangesAsync`.
- Leituras com `AsNoTracking()`; dashboards/contagens projetam direto para DTO com `Select`.
- Consultas de tenant filtram por `ClinicaId` (via tabelas de vínculo como `AnimalClinica`, `TutorClinica`, `UsuarioPerfilClinica`).

### DTOs
- `Cadastrar<Entidade>Dto`, `Editar<Entidade>Dto` (Sistema admin: `Editar<Entidade>AdminDto`), `Get<Entidade>Dto`, `<Entidade>InfoSimplesDto`.
- DataAnnotations com mensagens em pt-BR (`[Required(ErrorMessage = "...")]`, `[StringLength(...)]`).
- Documentos e telefones guardados só com dígitos (CPF 11, CNPJ 14, CEP 8, telefone 11).

### Mappers
- `<Entidade>MappingProfile : Profile` em `Mappers/`. DTOs de edição usam
  `.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));` para atualização parcial.
- Registro já é automático: `AddAutoMapper(config => { }, typeof(Program).Assembly)`.

### Entities e Enums
- `[Key]` + `[DatabaseGenerated(Identity)] int Id`, navegações `virtual ICollection<>?`.
- Relação N:N por entidade de vínculo explícita (padrão `AnimalClinica`, `TutorClinica`).
- Enums começam em 1 e podem ter `[Display(Name = "...")]` (use `GetDisplayName()`).

## 3. Registrar e migrar

1. `Program.cs`: `builder.Services.AddScoped<I<Entidade>Service, <Entidade>Service>();` e o mesmo para o repository, junto do bloco do domínio.
2. `Infra/Data/AppDbContext.cs`: `public DbSet<<Entidade>> <Entidades> { get; set; }` e configurações em `OnModelCreating` (chaves compostas, deletes, conversões).
3. Migration (rodar em `backend/`):
   ```bash
   dotnet ef migrations add <NomeEmPascalCaseDescritivo>
   dotnet ef database update
   ```
   Revise o arquivo gerado antes de aplicar. Seeds (`HasData`) só com valores estáticos (EF 10 rejeita `DateTime.UtcNow`, `Guid.NewGuid()`, hashes com salt).
4. `dotnet build` sem erros novos.

## 4. Checklist final
- [ ] Arquivos no domínio certo, com `Controllers/` e `Interfaces/` e namespace igual à pasta.
- [ ] Controller fino, policy correta, `clinicaId` do token.
- [ ] Service com `ServiceResult` e mensagens em pt-BR.
- [ ] DI em `Program.cs`, `DbSet` e migration revisada.
- [ ] Rota confere com `frontend/src/services/Endpoints.js` (ver skill `frontend`).
- [ ] Nada alterado em `bris-web` ou `https-server`; rotas novas continuam sob `/api/`.
