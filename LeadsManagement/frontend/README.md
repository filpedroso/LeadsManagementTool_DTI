# 🎨 Leads Management - Frontend

Interface de gerenciamento de leads desenvolvida com React + Vite.

## 🚀 Tecnologias

- **React 18** - Biblioteca UI
- **Vite** - Build tool e dev server
- **Axios** - Cliente HTTP
- **React Toastify** - Notificações toast

## 📋 Funcionalidades

### Tab "Invited" (Convidados)
Exibe todos os leads com status "Invited" contendo:
- ✅ Contact first name
- ✅ Date created
- ✅ Suburb
- ✅ Category
- ✅ ID
- ✅ Description
- ✅ Price
- ✅ Botão **Accept** - Aceita o lead (aplica 10% desconto se preço > $500)
- ✅ Botão **Decline** - Recusa o lead

### Tab "Accepted" (Aceitos)
Exibe todos os leads aceitos com informações adicionais:
- ✅ Contact full name
- ✅ Contact phone number
- ✅ Contact email
- ✅ Todas as informações da tab Invited

### Tab "Declined" (Recusados)
Exibe todos os leads recusados com as mesmas informações da tab Accepted.

## 🎯 Estrutura do Projeto

```
frontend/
├── src/
│   ├── api/              # Cliente API e endpoints
│   │   ├── apiClient.js
│   │   └── leadsApi.js
│   ├── components/
│   │   ├── common/       # Componentes reutilizáveis
│   │   │   ├── Header.jsx
│   │   │   └── TabNavigation.jsx
│   │   └── leads/        # Componentes de leads
│   │       ├── LeadCard.jsx
│   │       ├── LeadsList.jsx
│   │       └── LeadsContainer.jsx
│   ├── hooks/            # Custom hooks
│   │   └── useLeads.js
│   ├── App.jsx
│   └── main.jsx
├── .env.development      # Variáveis de ambiente (dev)
├── .env.production       # Variáveis de ambiente (prod)
└── package.json
```

## ⚙️ Instalação e Execução

### Desenvolvimento

```bash
# Instalar dependências
npm install

# Rodar servidor de desenvolvimento
npm run dev
```

O frontend estará disponível em: **http://localhost:3000**

### Build de Produção

```bash
# Criar build otimizado
npm run build

# Preview do build
npm run preview
```

## 🔧 Configuração

### Variáveis de Ambiente

**`.env.development`** (Desenvolvimento):
```env
VITE_API_BASE_URL=http://localhost:5000/api
```

**`.env.production`** (Produção):
```env
VITE_API_BASE_URL=https://your-production-api.com/api
```

## 🎨 UI/UX

- **Design responsivo** - Funciona em desktop, tablet e mobile
- **Cards interativos** - Animações suaves ao hover
- **Feedback visual** - Toast notifications para ações
- **Loading states** - Indicadores de carregamento
- **Error handling** - Mensagens de erro amigáveis

## 📡 Integração com Backend

O frontend consome os seguintes endpoints da API:

- `GET /api/leads/status/{status}` - Buscar leads por status
- `GET /api/leads/{id}` - Buscar lead específico
- `POST /api/leads/{id}/accept` - Aceitar lead
- `POST /api/leads/{id}/decline` - Recusar lead
- `POST /api/leads` - Criar novo lead

## 🧪 Features Implementadas

✅ SPA (Single Page Application) com React  
✅ Tabs navegáveis (Invited, Accepted, Declined)  
✅ Listagem de leads em cards  
✅ Ações de Accept/Decline  
✅ Notificações toast  
✅ Loading states  
✅ Error handling  
✅ Contadores dinâmicos nas tabs  
✅ Exibição condicional de campos (nome completo, telefone, email)  
✅ Design moderno e responsivo  
