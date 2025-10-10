# MediPorta App

Wymagania:
- pobrać min. 1000 tagów z API SO do lokalnej bazy danych lub innego trwałego zasobu (pobranie może nastąpić na starcie lub przy pierwszym żądaniu, od razu w całości lub stopniowo tylko brakujących danych)
- obliczyć procentowy udział tagów w całej pobranej populacji (źródłowe pole count, odpowiednio przeliczone)
- udostępnić tagi poprzez stronicowane API z opcją sortowania po nazwie i udziale w obu kierunkach
- udostępnić metodę API do wymuszenia ponownego pobrania tagów z SO
- udostępnić definicję OpenAPI przygotowanych metod API
- przygotować warstwę prezentacyjną przy użyciu dowolnego frontendowego frameworka, wykorzystującą zaimplementowane API do wyświetlenia wyników w formie tabeli z mechanizmem paginacji i sortowania
- przygotować kilka wybranych testów jednostkowych wewnętrznych usług implementacji
- przygotować kilka wybranych testów integracyjnych opartych o udostępniane API
- wykorzystać konteneryzację do zapewnienia powtarzalnego budowania i uruchamiania projektu
- całość powinna się uruchamiać po wykonaniu wyłącznie polecenia "docker compose up"
- rozwiązanie opublikować w repozytorium GitHub

## Spis treści

- [Struktura projektu](#struktura-projektu)
- [Wymagania](#wymagania)
- [Instrukcja uruchomienia](#instrukcja-uruchomienia)
  - [Uruchomienie lokalnie](#uruchomienie-lokalnie)
  - [Uruchomienie przy użyciu Dockera](#uruchomienie-przy-użyciu-dockera)
- [Dostęp do aplikacji](#dostęp-do-aplikacji)
- [Konfiguracja Nginx](#konfiguracja-nginx)
- [Kontakt](#kontakt)

---

## Struktura projektu

MediPortaAPP/
│
├─ backend/ # Node.js + Express
│ ├─ src/
│ ├─ package.json
│ └─ Dockerfile.backend
│
├─ frontend/ # Angular
│ ├─ mediporta-frontend/
│ ├─ package.json
│ └─ Dockerfile.frontend
│
├─ docker-compose.yml
└─ nginx/
└─ default.conf # Konfiguracja serwera Nginx

## Backend

cd backend
npm install
npm run start

Backend będzie dostępny pod http://localhost:5000

## Frontend
cd frontend/mediporta-frontend
npm install
ng serve --open

Frontend będzie dostępny pod http://localhost:4200

## Uruchomienie przy użyciu Dockera

git clone https://github.com/TwojaNazwaUzytkownika/MediPortaAPP.git
cd MediPortaAPP

docker compose up -d --build

docker ps
