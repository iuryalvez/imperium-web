# ==============================================================================
# CONFIGURAÇÕES GERAIS E INICIALIZAÇÃO DE VARIÁVEIS
# ==============================================================================

COMPOSE ?= docker compose
ENV_FILE := .env

# Carrega o arquivo unificado .env se ele existir na raiz do projeto.
# O bloco 'ifneq' valida a existência do arquivo antes de incluí-lo, evitando erros.
ifneq ($(wildcard $(ENV_FILE)),)
    include $(ENV_FILE)
endif

# ==============================================================================
# MAPEAMENTO DE VARIÁVEIS PARA RODAR EM DESENVOLVIMENTO LOCAL (NATIVE MODE)
# ==============================================================================

ENV_SUFFIX ?=
SERVICE_DB := db-imperium$(ENV_SUFFIX)
SERVICE_API := backend-imperium$(ENV_SUFFIX)
SERVICE_FRONT := frontend-imperium$(ENV_SUFFIX)
SERVICE_MIGRATOR := migrations-imperium$(ENV_SUFFIX)
# ATENÇÃO: se DB_PASS tiver '$$' ou ';', a connection string sai errada (o Make interpreta '$$' e o ';' separa campos).
ConnectionStrings__DefaultConnection := Host=$(DB_HOST);Port=$(DB_PORT);Database=$(DB_NAME);Username=$(DB_USER);Password=$(DB_PASS)

# O comando global 'export' força o Make a injetar todas as variáveis declaradas
# acima e as lidas do .env (Jwt__*, SwaggerAuth__*, CookieSecure, AllowedHosts...)
# no ambiente de memória de qualquer terminal filho criado pelas regras abaixo.
export

# ==============================================================================
# ALIASES E DOCUMENTAÇÃO (HELP)
# ==============================================================================

# Definição de alvos falsos (.PHONY) para garantir que o Make execute os 
# comandos mesmo se existirem pastas ou arquivos com os mesmos nomes no disco.
.PHONY: help build build-frontend-no-cache up update down logs ps shell seed clean clean-all drop-database dev-drop-database set migrator deploy dev-set-dependencies dev-add-migration dev-database-update dev-up-api dev-up-frontend dev-clean

help:
	@echo "Comandos disponíveis:"
	@echo "  make build         - Builda as imagens dos containers Docker"
	@echo "  make up            - Sobe os containers Docker em background"
	@echo "  make down          - Para e remove os containers Docker"
	@echo "  make logs          - Exibe os logs em tempo real do container da API"
	@echo "  make ps            - Lista o status de execução dos containers"
	@echo "  make shell         - Abre o terminal iterativo (sh) dentro do container da API"
	@echo "  make clean         - Remove containers, imagens e arquivos locais de build (não inclui banco de dados)"
	@echo "  make clean-all     - Remove containers, imagens, volumes e arquivos locais (incluindo banco de dados). APAGA também o .env e o docker-compose.yml"
	@echo "  make update        - Atualiza a aplicação, rebuildando e aplicando migrações no banco de dados"
	@echo "  make build-frontend-no-cache - Builda a imagem do frontend sem cache (forçando rebuild completo)"
	@echo "  make drop-database - Apaga o banco de dados deste ambiente (container + volume, destrutivo, pede confirmação)"
	@echo "  make deploy        - Executa o fluxo sequencial de build e subida dos containers"
	@echo "  make set           - Inicializa o arquivo .env a partir do template de amostra"
	@echo "  make migrator      - Executa as migrações de banco dentro do container transiente"
	@echo "  make dev-set-dependencies - Instala as dependências para desenvolvimento local"
	@echo "  make dev-add-migration    - Adiciona uma nova migração ao projeto localmente"
	@echo "  make dev-database-update  - Aplica as migrações no banco de dados local"
	@echo "  make dev-up-api          -- Inicia a API localmente com hot reload"
	@echo "  make dev-up-frontend     -- Inicia o frontend localmente"
	@echo "  make dev-clean           -- Remove arquivos locais de build e dependências (bin, obj, node_modules, build)"
	@echo "  make dev-drop-database   -- Remove o banco de dados local"

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

up:
	$(COMPOSE) up --build --detach

down:
	$(COMPOSE) down

logs:
	$(COMPOSE) logs -f $(SERVICE_API)

ps:
	$(COMPOSE) ps

deploy:
	$(COMPOSE) up --build --detach

shell:
	$(COMPOSE) exec $(SERVICE_API) sh

seed:
# 	$(COMPOSE) exec $(SERVICE_API) npm run seed

build-frontend-no-cache:
	$(COMPOSE) build --no-cache $(SERVICE_FRONT)

migrator:
	$(COMPOSE) run --rm $(SERVICE_MIGRATOR)

update:
	$(COMPOSE) build
	$(COMPOSE) up -d $(SERVICE_DB)
	$(MAKE) migrator
	$(COMPOSE) up -d --no-build $(SERVICE_API) $(SERVICE_FRONT)
	docker image prune -f

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

# CUIDADO: apaga o banco deste ambiente (container + volume de dados). Destrutivo.
# Remove apenas o volume montado no container $(SERVICE_DB), sem tocar no banco de outro ambiente.
drop-database:
	@CONTAINER=$$($(COMPOSE) ps -aq $(SERVICE_DB)); \
	if [ -z "$$CONTAINER" ]; then echo "Container $(SERVICE_DB) não encontrado. Nada a apagar."; exit 1; fi; \
	VOLUME=$$(docker inspect --format '{{range .Mounts}}{{if eq .Destination "/var/lib/postgresql/data"}}{{.Name}}{{end}}{{end}}' $$CONTAINER); \
	if [ -z "$$VOLUME" ]; then echo "Volume de dados do $(SERVICE_DB) não encontrado."; exit 1; fi; \
	echo "Serão apagados o container $(SERVICE_DB) e o volume $$VOLUME (todos os dados do banco)."; \
	read -p "Digite 'apagar' para confirmar: " c; [ "$$c" = "apagar" ] || { echo "Cancelado."; exit 1; }; \
	$(COMPOSE) stop $(SERVICE_API) $(SERVICE_FRONT) $(SERVICE_DB) && \
	$(COMPOSE) rm -f $(SERVICE_DB) && \
	docker volume rm $$VOLUME
	@echo "Banco de dados removido. Para recriar o banco e aplicar as migrações, rode make update."

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
	@echo "Limpeza concluída! (o .env foi preservado)"

dev-drop-database:
	@read -p "Isso APAGA o banco de dados local. Digite 'apagar' para confirmar: " c; [ "$$c" = "apagar" ] || { echo "Cancelado."; exit 1; }
	@echo "Removendo banco de dados local..."
	cd backend && dotnet ef database drop --force
	@echo "Banco de dados local removido."
	@echo "Se quiser recriar o banco e aplicar as migrações, rode make dev-database-update."
