/*
# 📖 DOCUMENTAÇÃO DA API

## Base URL
\`\`\`
https://localhost:7000/api/v1
\`\`\`

## Endpoints

### 1. Listar Leads por Status
\`\`\`
GET /leads/status/{status}
\`\`\`

**Parâmetros:**
- \`status\` (string): "Invited", "Accepted" ou "Declined"

**Resposta (200 OK):**
\`\`\`json
[
  {
    "id": 1,
    "contactFirstName": "João",
    "contactLastName": "Silva",
    "contactEmail": "joao@email.com",
    "contactPhoneNumber": "11999999999",
    "dateCreated": "2025-11-27T10:30:00Z",
    "suburb": "São Paulo",
    "category": "Tecnologia",
    "description": "Venda de software",
    "price": 900.00,
    "status": "Accepted"
  }
]
\`\`\`

### 2. Buscar Lead Específico
\`\`\`
GET /leads/{id}
\`\`\`

**Parâmetros:**
- \`id\` (int): ID do lead

**Resposta (200 OK):**
\`\`\`json
{
  "id": 1,
  "contactFirstName": "João",
  "contactLastName": "Silva",
  "contactEmail": "joao@email.com",
  "contactPhoneNumber": "11999999999",
  "dateCreated": "2025-11-27T10:30:00Z",
  "suburb": "São Paulo",
  "category": "Tecnologia",
  "description": "Venda de software",
  "price": 900.00,
  "status": "Accepted"
}
\`\`\`

### 3. Criar Novo Lead
\`\`\`
POST /leads
\`\`\`

**Body (JSON):**
\`\`\`json
{
  "contactFirstName": "Maria",
  "contactLastName": "Santos",
  "contactEmail": "maria@email.com",
  "contactPhoneNumber": "21987654321",
  "suburb": "Rio de Janeiro",
  "category": "Imóvel",
  "description": "Propriedade comercial",
  "price": 1500.00
}
\`\`\`

**Resposta (201 Created):**
\`\`\`json
{
  "id": 2
}
\`\`\`

### 4. Aceitar Lead
\`\`\`
POST /leads/{id}/accept
\`\`\`

**Parâmetros:**
- \`id\` (int): ID do lead

**Comportamento:**
- Muda status para "Accepted"
- Se preço > $500, aplica 10% de desconto
- Envia email para vendas@test.com

**Resposta (200 OK):**
\`\`\`json
{
  "message": "Lead 1 accepted successfully"
}
\`\`\`

### 5. Recusar Lead
\`\`\`
POST /leads/{id}/decline
\`\`\`

**Parâmetros:**
- \`id\` (int): ID do lead

**Resposta (200 OK):**
\`\`\`json
{
  "message": "Lead 1 declined successfully"
}
\`\`\`

## Códigos de Erro

| Código | Significado |
|--------|-------------|
| 200 | OK - Requisição bem sucedida |
| 201 | Created - Recurso criado |
| 400 | Bad Request - Dados inválidos |
| 404 | Not Found - Recurso não encontrado |
| 500 | Internal Server Error - Erro no servidor |

## Exemplo de Fluxo Completo

\`\`\`bash
# 1. Criar um novo lead
curl -X POST https://localhost:7000/api/v1/leads \\
  -H "Content-Type: application/json" \\
  -d '{
    "contactFirstName": "Pedro",
    "suburb": "Brasília",
    "category": "Serviços",
    "description": "Consultoria",
    "price": 600
  }'

# Resposta: { "id": 3 }

# 2. Listar leads convidados
curl https://localhost:7000/api/v1/leads/status/Invited

# 3. Aceitar o lead (com desconto aplicado)
curl -X POST https://localhost:7000/api/v1/leads/3/accept

# 4. Verificar se foi aceito
curl https://localhost:7000/api/v1/leads/3

# Resposta mostrará price reduzido para 540 (10% desconto de 600)
\`\`\`
*/
