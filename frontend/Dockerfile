# Estágio 1: Build (usando Node.js 22)
FROM node:22-alpine AS build
WORKDIR /app

# Copia os arquivos de manifesto e instala dependências (aproveita cache)
COPY package.json package-lock.json ./
RUN npm install

# Copia o restante do código e constrói a aplicação
COPY . .
RUN npm run build

# Estágio 2: Serve (usando Nginx)
FROM nginx:stable-alpine AS final
WORKDIR /usr/share/nginx/html

# Remove a página padrão do Nginx
RUN rm -rf ./*

# Copia os arquivos estáticos gerados no estágio 'build'
COPY --from=build /app/build .

# Copia o arquivo de configuração customizado do Nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expõe a porta 80
EXPOSE 80

# Comando para iniciar o Nginx em modo foreground
CMD ["nginx", "-g", "daemon off;"]