# API Produtos

API REST para gerenciamento de produtos desenvolvida em .NET Core 7 com PostgreSQL.

## Pré-requisitos

- .NET Core 7 SDK
- PostgreSQL 12 ou superior
- Visual Studio 2022 ou VS Code

## Configuração do Banco de Dados

### 1. Instalar PostgreSQL
- Baixe e instale o PostgreSQL: https://www.postgresql.org/download/
- Anote a senha do usuário `postgres`

### 2. Criar Banco de Dados
```sql
CREATE DATABASE produtos_db;
```

### 3. Configurar Connection String
Edite o arquivo `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=produtos_db;Username=postgres;Password=SUA_SENHA"
  }
}
```

### 4. Estrutura das Tabelas
A API criará automaticamente as seguintes tabelas:

#### Tabela: departamentos
- `id` - SERIAL PRIMARY KEY
- `nome` - VARCHAR(100) NOT NULL UNIQUE

#### Tabela: produtos
- `id` - SERIAL PRIMARY KEY
- `codigo` - VARCHAR(50) NOT NULL UNIQUE
- `descricao` - VARCHAR(200) NOT NULL
- `departamento_id` - INTEGER NOT NULL (FK para departamentos)
- `preco` - DECIMAL(18,2) NOT NULL CHECK (preco > 0)
- `status` - BOOLEAN NOT NULL DEFAULT true

#### Tabela: usuarios
- `id` - SERIAL PRIMARY KEY
- `nome` - VARCHAR(100) NOT NULL
- `email` - VARCHAR(100) NOT NULL UNIQUE
- `senha` - VARCHAR(255) NOT NULL
- `data_criacao` - TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
- `ativo` - BOOLEAN NOT NULL DEFAULT true

## Como Executar

### 1. Restaurar Pacotes
```bash
dotnet restore
```

### 2. Executar a API
```bash
dotnet run
```

### 3. Acessar Swagger
- http://localhost:44317/swagger

## Endpoints Disponíveis

### Produtos
- `GET /api/produtos` - Listar todos os produtos
- `GET /api/produtos/{id}` - Buscar produto por ID
- `POST /api/produtos` - Criar novo produto
- `PUT /api/produtos/{id}` - Atualizar produto
- `DELETE /api/produtos/{id}` - Excluir produto

### Departamentos
- `GET /api/departamentos` - Listar todos os departamentos

### Autenticação
- `POST /api/auth/login` - Login de usuário
- `POST /api/auth/registro` - Registro de usuário

## Configurações

### CORS
A API está configurada para aceitar requisições do frontend Angular em `http://localhost:4200`.

### JWT
A autenticação JWT está configurada mas não é obrigatória para os endpoints de produtos e departamentos.

## Tecnologias

- .NET Core 7
- PostgreSQL
- ADO.NET
- Swagger/OpenAPI
- JWT (opcional)
- AutoMapper
