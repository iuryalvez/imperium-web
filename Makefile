COMPOSE ?= docker compose
TARGET ?= prod
ENV_FILE := .env
SERVICE_API := api

.PHONY: help build up down logs ps shell seed clean

help:
	@echo "Comandos disponíveis:"
	@echo "  make build         - Builda os containers com TARGET=$(TARGET)"
	@echo "  make up            - Sobe os containers com docker-compose up"
	@echo "  make down          - Derruba os containers"
	@echo "  make logs          - Mostra os logs do container da API"
	@echo "  make ps            - Lista os containers em execução"
	@echo "  make shell         - Entra no bash do container da API"
	@echo "  make seed          - Executa um seed no container da API"
	@echo "  make clean         - Remove tudo e rebuilda do zero"

set:
	@echo "Criando arquivo de variáveis ​​de ambiente.."
	@cp config/.env-sample .env
	@echo "Copiando arquivos do docker..."
	@cp config/docker-compose-sample.yml docker-compose.yml

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
	@make build && make up

shell:
	$(COMPOSE) exec $(SERVICE_API) sh

seed:
# 	$(COMPOSE) exec $(SERVICE_API) npm run seed

clean:
	$(COMPOSE) down --rmi all -v --remove-orphans
	@rm docker-compose.yml
	@rm -rf ./backend/bin
	@rm -rf ./frontend/node_modules
