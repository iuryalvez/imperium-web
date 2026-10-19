# ==============================================================================
# CONFIGURAÇÕES GERAIS E INICIALIZAÇÃO DE VARIÁVEIS
# ==============================================================================

COMPOSE ?= docker compose
TARGET ?= prod
ENV_FILE := .env
SERVICE_API := backend-imperium

# Carrega o arquivo unificado .env se ele existir na raiz do projeto.
# O bloco 'ifneq' valida a existência do arquivo antes de incluí-lo, evitando erros.
ifneq ($(wildcard $(ENV_FILE)),)
    include $(ENV_FILE)
endif

# ==============================================================================
# MAPEAMENTO DE VARIÁVEIS PARA RODAR EM DESENVOLVIMENTO LOCAL (NATIVE MODE)
# ==============================================================================

ConnectionStrings__DefaultConnection := Host=$(DB_HOST);Port=$(DB_PORT);Database=$(DB_NAME);Username=$(DB_USER);Password=$(DB_PASS)
Jwt__Key := $(Jwt__Key)
Jwt__Issuer := $(Jwt__Issuer)
Jwt__Audience := $(Jwt__Audience)
SwaggerAuth__Username := $(SwaggerAuth__Username)
SwaggerAuth__Password := $(SwaggerAuth__Password)
CookieSecure := $(CookieSecure)
AllowedHosts := $(AllowedHosts)

# O comando global 'export' força o Make a injetar todas as variáveis declaradas
# acima no ambiente de memória de qualquer terminal filho criado pelas regras abaixo.
export

# ==============================================================================
# ALIASES E DOCUMENTAÇÃO (HELP)
# ==============================================================================

# Definição de alvos fonéticos (.PHONY) para garantir que o Make execute os 
# comandos mesmo se existirem pastas ou arquivos com os mesmos nomes no disco.
.PHONY: help build build-frontend-no-cache up update down logs ps shell seed clean clean-all set migrator deploy dev-set-dependencies dev-add-migration dev-database-update dev-up-api dev-up-frontend dev-clean

help:
	@echo "Comandos disponíveis:"
	@echo "  make build         - Builda as imagens dos containers Docker"
	@echo "  make up            - Sobe os containers Docker em background"
	@echo "  make down          - Para e remove os containers Docker"
	@echo "  make logs          - Exibe os logs em tempo real do container da API"
	@echo "  make ps            - Lista o status de execução dos containers"
	@echo "  make shell         - Abre o terminal iterativo (sh) dentro do container da API"
	@echo "  make clean         - Remove containers, imagens e arquivos locais de build (não inclui banco de dados)"
	@echo "  make clean-all     - Remove containers, imagens, volumes e arquivos locais (incluindo banco de dados)"
	@echo "  make deploy        - Executa o fluxo sequencial de build e subida dos containers"
	@echo "  make set           - Inicializa o arquivo .env a partir do template de amostra"
	@echo "  make migrator      - Executa as migrações de banco dentro do container transiente"
	@echo "  make dev-set-dependencies - Instala as dependências para desenvolvimento local"
	@echo "  make dev-add-migration    - Adiciona uma nova migração ao projeto localmente"
	@echo "  make dev-database-update  - Aplica as migrações no banco de dados local"
	@echo "  make dev-up-api          -- Inicia a API localmente com hot reload"
	@echo "  make dev-up-frontend     -- Inicia o frontend localmente"
	@echo "  make dev-clean           -- Remove containers, imagens e arquivos locais de build (desenvolvimento)"

# ==============================================================================
# COMANDOS DO MODO CONTEINERIZADO (DOCKER / PRODUÇÃO)
# ==============================================================================

set:
	@echo "Criando arquivo de variáveis de ambiente..."
	@cp config/.env-sample .env
	@cp config/docker-compose-sample.yml docker-compose.yml
	@echo "Edite o arquivo .env para escolher o ambiente e configurar as variáveis."

build:
	$(COMPOSE) build 

build-frontend-no-cache:
	$(COMPOSE) build --no-cache frontend-imperium

up:
	$(COMPOSE) up --build --detach

migrator:
	$(COMPOSE) run --rm migrations-imperium

down:
	$(COMPOSE) down

logs:
	$(COMPOSE) logs -f $(SERVICE_API)

ps:
	$(COMPOSE) ps
	
deploy:
	@make build && make up

shell:
	$(COMPOSE) exec $(SERVICE_API) sh

seed:
# 	$(COMPOSE) exec $(SERVICE_API) npm run seed

update:
	$(COMPOSE) build
	$(COMPOSE) up -d db-imperium
	make migrator
	$(COMPOSE) up -d --no-build backend-imperium frontend-imperium
	docker image prune -f
	@echo ""
	@echo "======================================================================"
	@echo " APLICAÇÃO ATUALIZADA COM SUCESSO!"
	@echo " Para aplicar as rotas, acesse o proxy global e reinicie o serviço:"
	@echo " -> cd ~/https-server && make restart"
	@echo "======================================================================"
	@echo ""

# Remove containers, redes e imagens, mas PRESERVA o banco de dados (seguro)
clean:
	$(COMPOSE) down --rmi all --remove-orphans
	@rm -rf ./backend/bin
	@rm -rf ./frontend/node_modules
	@echo "Limpeza de infraestrutura concluída. Dados do banco preservados."

# CUIDADO: Remove tudo, INCLUSIVE o volume do banco de dados (destrutivo)
clean-all:
	$(COMPOSE) down --rmi all -v --remove-orphans
	@rm -f docker-compose.yml
	@rm -f .env
	@rm -rf ./backend/bin
	@rm -rf ./frontend/node_modules
	@echo "Limpeza total concluída. O banco de dados foi APAGADO."

# ==============================================================================
# COMANDOS DO MODO NATIVO (DESENVOLVIMENTO LOCAL NO HOST)
# ==============================================================================

dev-set-dependencies: # ATENÇÃO: Executar obrigatoriamente sem 'sudo' para manter permissões do NVM
	@echo "Instalando dependências para desenvolvimento local..."
	@cd backend && dotnet restore
	@cd frontend && npm install

dev-add-migration:
	@read -p "Digite o nome da nova migração: " name; \
	echo "Criando migração $$name..."; \
	cd backend && dotnet ef migrations add $$name

dev-database-update:
	@echo "Aplicando migrações no banco de dados local..."
	cd backend && dotnet ef database update

dev-up-api:
	@echo "Iniciando API localmente..."
	cd backend && dotnet watch run

dev-up-frontend:
	@echo "Iniciando Frontend localmente..."
	cd frontend && npm start

dev-clean:
	@echo "Limpando arquivos locais..."
	@rm -rf ./backend/bin ./backend/obj
	@rm -rf ./frontend/node_modules ./frontend/build
	@rm -f .env
	@echo "Limpeza concluída!"
