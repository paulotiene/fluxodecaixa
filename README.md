# Sistema de Fluxo de Caixa Diário

Sistema para controle de lançamentos financeiros (crédito/débito) e consulta de saldo diário consolidado.

---

## Visão Geral

A aplicação é composta por dois serviços:

### Lancamentos.Api
- Recebe e persiste lançamentos
- Publica eventos via Outbox Pattern
- Responsável pela autenticação (JWT)

### Consolidado.Api
- Consome eventos via RabbitMQ
- Processa os lançamentos
- Mantém o saldo diário consolidado

---

## Fluxo Simplificado

    Cliente → Lancamentos.Api → RabbitMQ → Consolidado.Api

- Escrita: assíncrona via mensageria  
- Leitura: direta no serviço de consolidado  
- Consistência: eventual  

---

## Tecnologias

- .NET 9 / C#
- ASP.NET Core
- Entity Framework Core
- SQL Server
- RabbitMQ
- JWT
- Docker / Docker Compose
- Swagger

---

## Como Rodar

### Pré-requisitos

- Docker
- Docker Compose

### Subir ambiente

    docker compose down -v
    docker compose up --build

---

## Acessos

- Swagger Lançamentos: http://localhost:8081/swagger  
- Swagger Consolidado: http://localhost:8082/swagger  
- RabbitMQ: http://localhost:15672  

---

## 🔑 Credenciais

### RabbitMQ
- Usuário: guest  
- Senha: guest  

### SQL Server
- Servidor: localhost,1433  
- Usuário: sa  
- Senha: Admin#123  

---

## Autenticação

### Gerar token

POST /api/auth/token

    {
      "userName": "admin",
      "password": "123456"
    }

---

## Como Testar (Passo a Passo)

### 1. Gerar token
- Acesse o Swagger de Lançamentos
- Execute `/api/auth/token`
- Copie o `accessToken`

### 2. Autorizar
- Clique em **Authorize**
- Cole o token

### 3. Criar lançamento

POST /api/lancamentos

    {
      "tipo": 1,
      "valor": 100.00,
      "descricao": "Venda teste",
      "dataOcorrencia": "2026-03-20T10:00:00Z"
    }

### 4. Consultar consolidado

GET /api/consolidados/2026-03-20

---

## Observação

Este README apresenta uma visão simplificada da solução.  
Para detalhes completos de arquitetura e decisões técnicas, consulte o relatório técnico do projeto em docs/RelatorioTecnico.pdf.