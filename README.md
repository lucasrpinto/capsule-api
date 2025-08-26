# Capsule‑API (Projeto não finalizado)

API backend em C# (ex: ASP.NET Core) para gerenciamento de cápsulas do tempo virtuais — registre mensagens que só poderão ser lidas no futuro.

## 🚀 Tecnologias utilizadas

- **.NET** (versão, ex: .NET 7 ou .NET 8)
- **ASP.NET Core Web API**
- **Entity Framework Core** (ou outra camada de acesso a dados)
- Banco de dados: SQL Server / PostgreSQL / SQLite (especificar)
- **Swagger / OpenAPI** para documentação interativa

## 📌 Funcionalidades principais

- Criar, listar, visualizar e deletar cápsulas do tempo
- Agendar abertura futura das cápsulas
- … (outras funcionalidades que você tenha implementado)

## ⚙️ Começando (setup local)

### Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/) (ex: 7.0+)
- (Opcional) Docker e Docker Compose, se tiver containers configurados

### Executando localmente

```bash
# clonar o repositório
git clone https://github.com/lucasrpinto/capsule-api.git
cd capsule-api

# restaurar dependências e rodar
dotnet restore
dotnet run
```

Após isso, a API estará rodando em `http://localhost:5000` (ou porta configurada). Para ver os endpoints disponíveis, acesse:
```
http://localhost:<porta>/swagger
```

## 📡 Endpoints (exemplos)

| Verbo HTTP | Rota                 | Descrição                                      |
|------------|----------------------|------------------------------------------------|
| `GET`      | `/api/capsules`       | Lista todas as cápsulas                         |
| `GET`      | `/api/capsules/{id}`  | Retorna uma cápsula por ID                      |
| `POST`     | `/api/capsules`       | Cria uma nova cápsula                           |
| `DELETE`   | `/api/capsules/{id}`  | Deleta uma cápsula                              |

> Substitua pelo que você já implementou na sua API.

## 🤝 Contribuições

Contribuições são bem-vindas!  
Para contribuir, siga estes passos:

1. Fork no repositório
2. Crie uma branch (`git checkout -b feature/nome-da-feature`)
3. Faça alterações e crie commits (`git commit -m "Descrição da feature"`)
4. Envie para sua fork (`git push origin feature/nome-da-feature`)
5. Abra um Pull Request

## 📄 Licença

## 👤 Autor

- **Lucas Rodrigo Pinto** — [@lucasrpinto](https://github.com/lucasrpinto)
