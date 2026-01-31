FROM node:20-alpine AS build

ARG USE_COMPOSE_ENV=false

WORKDIR /app

COPY package.json package-lock.json* ./
RUN npm install

COPY . .

RUN if [ "$USE_COMPOSE_ENV" = "true" ] && [ -f .env.compose ]; then cp .env.compose .env; fi

EXPOSE 5173

CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0", "--port", "5173"]
