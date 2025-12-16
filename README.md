# 🗳️ Infnet.OnlineSurveys

Sistema de gerenciamento de pesquisas online desenvolvido em **.NET 9**, seguindo os princípios de **Clean Architecture**, com foco em organização, regras de negócio bem definidas e simplicidade de execução para fins acadêmicos, baseado na disciplina de Arquitetura de Software .NET do Instituto Infnet (2025).

---

## 📌 Índice

- [📖 Sobre o Projeto](#-sobre-o-projeto)
- [🛠️ Tecnologias Utilizadas](#️-tecnologias-utilizadas)
- [🏗️ Arquitetura](#️-arquitetura)
- [🚀 Como Executar](#-como-executar)
- [🧪 Como Testar com Postman](#-como-testar-com-postman)
- [📋 Regras de Negócio](#-regras-de-negócio)
- [✅ Testes](#-testes)
- [💾 Banco de Dados](#-banco-de-dados)
- [🛠️ Troubleshooting](#️-troubleshooting)
- [📄 Licença](#-licença)
- [👨‍💻 Autor](#-autor)

---

## 📖 Sobre o Projeto

O **Infnet.OnlineSurveys** é um sistema para **criação, gerenciamento e coleta de respostas de pesquisas online**.  
Permite a criação de pesquisas com múltiplas questões de **múltipla escolha**, publicação controlada e submissão de respostas por usuários.

### ✨ Funcionalidades Principais

- 📝 Criação e gerenciamento de pesquisas (Surveys)
- ❓ Adição de questões e opções de resposta
- 🔄 Controle de status das pesquisas (Rascunho, Publicada, Fechada)
- 👤 Coleta de respostas com identificação do respondente
- ✅ Validações de regras de negócio
- 🌐 API REST
- 📚 Documentação Swagger
- 🧠 Banco de dados em memória (InMemory)
- 🧪 Testes unitários

---

## 🛠️ Tecnologias Utilizadas

- **.NET 9**
- **ASP.NET Core**
- **Entity Framework Core 9**
- **Entity Framework InMemory**
- **Swagger / OpenAPI**
- **xUnit**

---

## 🏗️ Arquitetura

O projeto segue os princípios da **Clean Architecture**, separando claramente responsabilidades entre as camadas:

```
Infnet.OnlineSurveys/
├─ Infnet.OnlineSurveys.Domain/
│  ├─ Entities/
│  ├─ Enums/
│  └─ Repositories/
├─ Infnet.OnlineSurveys.Infrastructure.Data/
│  ├─ OnlineSurveysDbContext.cs
│  └─ Repositories/
├─ Infnet.OnlineSurveys.Api/
│  ├─ Controllers/
│  ├─ DTOs/
│  └─ Program.cs
├─ Infnet.OnlineSurveys.Infrastructure.Data.Tests/
├─ Infnet.OnlineSurveys.Api.Tests/
└─ Infnet.OnlineSurveys.Domain.Tests/
```

A descrição detalhada da arquitetura do sistema, incluindo decisões arquiteturais e diagramas no padrão **C4 Model**, está disponível no documento abaixo:

👉 [📄 Arquitetura Conceitual do Sistema](docs/arquitetura_conceitual.md)

---

## 🚀 Como Executar

### 🔧 Pré-requisitos

- **.NET 9 SDK**
- Visual Studio 2022 / VS Code / Rider

### ▶️ Execução

```bash
git clone <url-do-repositorio>
cd Infnet.OnlineSurveys
dotnet restore
cd Infnet.OnlineSurveys.Api
dotnet run
```

Acesse o Swagger em:
```
https://localhost:<porta>/swagger
```

---

## 🧪 Como Testar com Postman

O projeto é testado através de uma **collection do Postman** que pode ser encontrada na raiz do projeto. Na collection haverá uma pasta chamada **Fluxo Completo - Exemplo**, para validar basta executar sequencialmente.

Arquivo **Infnet.OnlineSurveysAPI.postman_collection,json**

### ⚠️ Execução em Ordem (Fluxo Completo)

Os endpoints **devem ser executados em ordem**, pois cada etapa depende da anterior:

1. Criar Survey  
2. Adicionar Questions e Options  
3. Publicar Survey  
4. Submeter Response  
5. Consultar resultados  
6. (Opcional) Fechar Survey  

> Como o projeto utiliza **Entity Framework InMemory**, os dados são perdidos ao reiniciar a aplicação.

---

## 📋 Regras de Negócio

- Surveys possuem status **Draft**, **Published** e **Closed**
- Apenas surveys publicados aceitam respostas
- Para publicar, é necessário ao menos 1 questão com 2 opções

---

## ✅ Testes

```bash
dotnet test
```

---

## 💾 Banco de Dados

- Entity Framework **InMemory**
- Ideal para testes e fins acadêmicos
- Dados não persistem após reinício

A decisão para utilização do Entity Framework InMemory foi baseada em realizar uma implementação para prova de conceito, assim, com a escala do projeto, é facilmente possível substituir o InMemory por algum outro banco de dados.

---

## 🛠️ Troubleshooting

- Dados somem ao reiniciar → comportamento esperado do InMemory

---

## 📄 Licença

Projeto desenvolvido para fins educacionais no **Instituto Infnet** | MIT.

---

💙 Desenvolvido com **.NET 9**

## 👨‍💻 Autor

**Caique Reimberg**
- GitHub: https://github.com/CaiqueReimberg
- LinkedIn: https://www.linkedin.com/in/caique-carara-reimberg-75639514a/