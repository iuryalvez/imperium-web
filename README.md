# Imperium  Web

## Instalação com o docker-compose

 Passos:
- Instale o docker-compose (sudo apt install docker-compose)
- Baixe o projeto Imperium Web do Gitlab (git clone ...URL...)
- Entre na pasta do Imperium Web (cd imperium-web)
- Se for a primeira instalação, rode: ./setup.sh
- Crie o container com o Banco de Dados (sudo docker-compose -f docker-compose.db.yml up)
- Crie os containers da aplicação backend e frontend (sudo docker-compose up)


## Instalação para rodar a aplicação no Ubuntu 24.04.1 LTS x86_64

O Runtime do ASP.NET Core permite executar aplicativos feitos com o .NET que não forneceram o runtime. O comando a seguir instala o runtime do ASP.NET Core, que é o runtime mais compatível com o .NET. Em seu terminal, execute os seguintes comandos:

    sudo apt-get update && \
        sudo apt-get install -y aspnetcore-runtime-8.0

## Instalação para desenvolvimento no Ubuntu 24.04.1 LTS x86_64

O SDK do .NET permite que você desenvolva aplicativos com o .NET. Se você instalar o SDK do .NET, não será necessário instalar o runtime correspondente. Para instalar o SDK do .NET, execute os seguintes comandos:

    sudo apt-get update && \
        sudo apt-get install -y dotnet-sdk-8.0
    
Quando você faz a instalação com um gerenciador de pacotes, essas bibliotecas são instaladas automaticamente. Porém, se você instalar o .NET manualmente ou publicar um aplicativo autossuficiente, será necessário verificar se estas bibliotecas estão instaladas:

- ca-certificates
- libc6
- libgcc-s1
- libicu74
- liblttng-ust1
- libssl3
- libstdc++6
- libunwind8
- zlib1g

As dependências podem ser instaladas por meio do comando apt install. O snippet a seguir demonstra a instalação da biblioteca zlib1g:

    sudo apt install zlib1g

## Tecnologias backend

<!-- - [Playlist de criação de uma aplicação ASP.NET Web API .NET 8](https://www.youtube.com/playlist?list=PL82C6-O4XrHfrGOCPmKmwTO7M0avXyQKc) -->
- [.NET 8](https://dotnet.microsoft.com/en-us/download)
- [Documentação do ASP.NET](https://learn.microsoft.com/pt-br/aspnet/core/?view=aspnetcore-8.0)
- [.NET API Web e Controladores](https://learn.microsoft.com/pt-br/training/modules/build-web-api-aspnet-core/)
- [Criar APIs Web com o ASP.NET Core](https://learn.microsoft.com/pt-br/aspnet/core/web-api/?view=aspnetcore-8.0&WT.mc_id=dotnet-35129-website)
- [Tutorial: criar uma API Web com o ASP.NET Core](https://learn.microsoft.com/pt-br/aspnet/core/tutorials/first-web-api?view=aspnetcore-8.0&tabs=visual-studio)
- [pgAdmin4](https://www.pgadmin.org/download/pgadmin-4-apt/)

<!-- ## Add your files

- [ ] [Create](https://docs.gitlab.com/ee/user/project/repository/web_editor.html#create-a-file) or [upload](https://docs.gitlab.com/ee/user/project/repository/web_editor.html#upload-a-file) files
- [ ] [Add files using the command line](https://docs.gitlab.com/ee/gitlab-basics/add-file.html#add-a-file-using-the-command-line) or push an existing Git repository with the following command: -->

## Comandos github

    cd existing_repo
    git remote add origin https://gitlab.com/evz-ufg-software-rural/imperium-web.git
    git branch -M main
    git push -uf origin main

<!-- ## Integrate with your tools

- [ ] [Set up project integrations](https://gitlab.com/evz-ufg-software-rural/imperium-web/-/settings/integrations)

## Collaborate with your team

- [x] [Invite team members and collaborators](https://docs.gitlab.com/ee/user/project/members/)
- [ ] [Create a new merge request](https://docs.gitlab.com/ee/user/project/merge_requests/creating_merge_requests.html)
- [ ] [Automatically close issues from merge requests](https://docs.gitlab.com/ee/user/project/issues/managing_issues.html#closing-issues-automatically)
- [ ] [Enable merge request approvals](https://docs.gitlab.com/ee/user/project/merge_requests/approvals/)
- [ ] [Set auto-merge](https://docs.gitlab.com/ee/user/project/merge_requests/merge_when_pipeline_succeeds.html)

## Test and Deploy

Use the built-in continuous integration in GitLab.

- [ ] [Get started with GitLab CI/CD](https://docs.gitlab.com/ee/ci/quick_start/index.html)
- [ ] [Analyze your code for known vulnerabilities with Static Application Security Testing (SAST)](https://docs.gitlab.com/ee/user/application_security/sast/)
- [ ] [Deploy to Kubernetes, Amazon EC2, or Amazon ECS using Auto Deploy](https://docs.gitlab.com/ee/topics/autodevops/requirements.html)
- [ ] [Use pull-based deployments for improved Kubernetes management](https://docs.gitlab.com/ee/user/clusters/agent/)
- [ ] [Set up protected environments](https://docs.gitlab.com/ee/ci/environments/protected_environments.html)

***

# Editing this README

When you're ready to make this README your own, just edit this file and use the handy template below (or feel free to structure it however you want - this is just a starting point!). Thanks to [makeareadme.com](https://www.makeareadme.com/) for this template.

## Suggestions for a good README

Every project is different, so consider which of these sections apply to yours. The sections used in the template are suggestions for most open source projects. Also keep in mind that while a README can be too long and detailed, too long is better than too short. If you think your README is too long, consider utilizing another form of documentation rather than cutting out information.

## Name
Choose a self-explaining name for your project.

## Description
Let people know what your project can do specifically. Provide context and add a link to any reference visitors might be unfamiliar with. A list of Features or a Background subsection can also be added here. If there are alternatives to your project, this is a good place to list differentiating factors.

## Badges
On some READMEs, you may see small images that convey metadata, such as whether or not all the tests are passing for the project. You can use Shields to add some to your README. Many services also have instructions for adding a badge.

## Visuals
Depending on what you are making, it can be a good idea to include screenshots or even a video (you'll frequently see GIFs rather than actual videos). Tools like ttygif can help, but check out Asciinema for a more sophisticated method.

## Installation
Within a particular ecosystem, there may be a common way of installing things, such as using Yarn, NuGet, or Homebrew. However, consider the possibility that whoever is reading your README is a novice and would like more guidance. Listing specific steps helps remove ambiguity and gets people to using your project as quickly as possible. If it only runs in a specific context like a particular programming language version or operating system or has dependencies that have to be installed manually, also add a Requirements subsection. -->

## Configuração do Ambiente .NET

<!-- [Create Project + Install](https://www.youtube.com/watch?v=qBTe6uHJS_Y&list=PL82C6-O4XrHfrGOCPmKmwTO7M0avXyQKc&index=1) -->

    dotnet new webapi -n backend
    dotnet add package Microsoft.EntityFrameworkCore
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer
    dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
    dotnet add package Microsoft.EntityFrameworkCore.Design
    dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
    dotnet add package Microsoft.EntityFrameworkCore.Tools
    dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
    dotnet tool uninstall -g dotnet-aspnet-codegenerator
    dotnet tool install -g dotnet-aspnet-codegenerator
    dotnet tool update -g dotnet-aspnet-codegenerator
    dotnet tool install --global dotnet-ef --version 8.*
    dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
    dotnet aspnet-codegenerator controller -name ItemController -async -api -m DbItem -dc AppDbContext -outDir Controllers

## Comandos dotnet

    dotnet restore
    dotnet ef migrations add InitialCreate
    dotnet ef database update
    dotnet build
    dotnet run

## Documentação do backend

### Arquitetura em Camadas

O sistema é dividido em camadas bem definidas, e cada camada é responsável por uma parte específica da aplicação

Controllers: Interagem com a camada de apresentação ou de API, manipulando as requisições HTTP e retornando as respostas.
DTOs: Objetos usados para transferir dados entre camadas, sem expor as entidades diretamente.
Mappers: Utilizados para mapear entidades para DTOs e vice-versa.
Interfaces: Definem contratos ou abstrações para classes concretas, facilitando a inversão de controle e a injeção de dependências.
Repositories: Responsáveis pela persistência de dados e interações com o banco de dados.

### 1. Models

As models do nosso projeto foram criadas de acordo com este [diagrama de entidade-relacionamento](https://drive.google.com/file/d/1ewlbDLb5ggOYnSZO35JDM6wvF99qu__U/view?usp=sharing)

<!-- [Models](https://www.youtube.com/watch?v=jMFaAc3sa04&list=PL82C6-O4XrHfrGOCPmKmwTO7M0avXyQKc&index=2) -->

### 2. Application DB Context

<!-- [Entity Framework](https://www.youtube.com/watch?v=SIQhe-yt3mA&list=PL82C6-O4XrHfrGOCPmKmwTO7M0avXyQKc&index=3) -->

Entity Framework é o **object relational mapper** (ORM) de aplicações .NET Web Core API

Depois de configurado o DB context, para utilizar postgreSQL devemos adicioná-lo ao program.cs com:

    builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

E em appsettings.json:

    "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Database=seu_db;Username="seu usuario";Password="sua senha""
    }

Com isso podemos gerar a migration (Entity Framework) que irá construir o nosso banco de dados de acordo com o DBContext:

    dotnet ef migrations add InitialCreate

Criação do banco de dados:

    dotnet ef database update

### 3. Controllers

<!-- [Controllers](https://www.youtube.com/watch?v=cSnoAGHKVus&list=PL82C6-O4XrHfrGOCPmKmwTO7M0avXyQKc&index=4) -->

Os responsáveis por fazerem as requisições ao banco de dados através dos endpoints.

    [Route("api/usuarios")] // ou api/[controller], mas o "[controller]" é capitalizado e isso não é aesthetic
    [ApiController]
    public class UsuarioController : ControllerBase // ControllerBase: padrão seguido
    { // código do controlador 
    }

Em program.cs, adicione os controladores, o swagger e o mapeamento dos controladores para o swagger funcionar:

    builder.Services.AddControllers(); 

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.MapControllers();

Para acessar o swagger: http://localhost:"porta definida"/swagger.

### 4. Data Transfer Objects (DTOs)

<!-- [DTOs](https://www.youtube.com/watch?v=Mxm81T7waO8&list=PL82C6-O4XrHfrGOCPmKmwTO7M0avXyQKc&index=5) -->

Responses vs Requests.

Definição de quais dados das models serão transferidos entre as requisições.

<!-- ### 6. Repository Pattern / DI

[Repository Pattern/DI](https://www.youtube.com/watch?v=6vsONJla1Fk&list=PL82C6-O4XrHfrGOCPmKmwTO7M0avXyQKc&index=10)

Entity -> Code to an interface (abstraction).

Envelopar as requisições em Find "Something" com Dependency Injection (DI).

Precisa ter o código de construção pronto antes de realizar a DI. -->

#### Interfaces

Declaração das funções que são utilizadas.

<!-- #### Repository

Definição e implementação das funções encapsuladas do controlador. Usando Dependecy Injection. Refatoração dos controladores. -->

## Tecnologias frontend

- [Node.js 20.14](https://nodejs.org/en/download/package-manager)
- [Create a new React App](https://legacy.reactjs.org/docs/create-a-new-react-app.html)

## Configuração do ambiente React

    npx create-react-app frontend
    cd frontend
    npm install

## Comandos React

<!--  Informações do REACT - index.html
      Notice the use of %PUBLIC_URL% in the tags above.
      It will be replaced with the URL of the `public` folder during the build.
      Only files inside the `public` folder can be referenced from the HTML.

      Unlike "/favicon.ico" or "favicon.ico", "%PUBLIC_URL%/favicon.ico" will
      work correctly both with client-side routing and a non-root public URL.
      Learn how to configure a non-root public URL by running `npm run build`. -->

<!-- You can add webfonts, meta tags, or analytics to this file.
    The build step will place the bundled scripts into the <body> tag. -->

    To begin the development, run `npm start` or `yarn start`.
    To create a production bundle, use `npm run build` or `yarn build`.


## Documentação do frontend


<!-- ## Usage
Use examples liberally, and show the expected output if you can. It's helpful to have inline the smallest example of usage that you can demonstrate, while providing links to more sophisticated examples if they are too long to reasonably include in the README.

## Support
Tell people where they can go to for help. It can be any combination of an issue tracker, a chat room, an email address, etc.

## Roadmap
If you have ideas for releases in the future, it is a good idea to list them in the README.

## Contributing
State if you are open to contributions and what your requirements are for accepting them.

For people who want to make changes to your project, it's helpful to have some documentation on how to get started. Perhaps there is a script that they should run or some environment variables that they need to set. Make these steps explicit. These instructions could also be useful to your future self.

You can also document commands to lint the code or run tests. These steps help to ensure high code quality and reduce the likelihood that the changes inadvertently break something. Having instructions for running tests is especially helpful if it requires external setup, such as starting a Selenium server for testing in a browser.

## Authors and acknowledgment
Show your appreciation to those who have contributed to the project.

## License
For open source projects, say how it is licensed.

## Project status
If you have run out of energy or time for your project, put a note at the top of the README saying that development has slowed down or stopped completely. Someone may choose to fork your project or volunteer to step in as a maintainer or owner, allowing your project to keep going. You can also make an explicit request for maintainers. -->

