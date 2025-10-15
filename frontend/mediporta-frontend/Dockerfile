# Stage 1: Build Angular
FROM node:20 AS build
WORKDIR /app

# Kopiujemy tylko package.json i package-lock.json i instalujemy zależności
COPY package*.json ./
RUN npm ci

# Kopiujemy resztę kodu i budujemy produkcyjny build
COPY . .
RUN npm run build -- --configuration production

# Stage 2: Nginx
FROM nginx:alpine
# Kopiujemy build Angulara do folderu Nginx
COPY --from=build /app/dist/mediporta-frontend/. /usr/share/nginx/html/

# Kopiujemy własny nginx.conf
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
