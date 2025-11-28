// ===================================================================
// FILE: frontend/package.json
// ===================================================================

{
  "name": "leads-management-ui",
  "version": "1.0.0",
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "vite build",
    "preview": "vite preview",
    "lint": "eslint . --ext .js,.jsx,.ts,.tsx"
  },
  "dependencies": {
    "react": "^18.2.0",
    "react-dom": "^18.2.0",
    "axios": "^1.6.0",
    "react-toastify": "^9.1.3"
  },
  "devDependencies": {
    "@types/react": "^18.2.0",
    "@types/react-dom": "^18.2.0",
    "@vitejs/plugin-react": "^4.0.0",
    "vite": "^4.4.0"
  }
}

// ===================================================================
// FILE: frontend/vite.config.js
// ===================================================================

import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 3000,
    proxy: {
      '/api': {
        target: 'https://localhost:7000',
        changeOrigin: true,
        secure: false,
      }
    }
  }
})

// ===================================================================
// FILE: frontend/.env.development
// ===================================================================

/*
VITE_API_BASE_URL=https://localhost:7000/api/v1
*/

// ===================================================================
// FILE: frontend/.env.production
// ===================================================================

/*
VITE_API_BASE_URL=/api/v1
*/

// ===================================================================
// FILE: frontend/src/main.jsx
// ===================================================================

import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App'
import './index.css'

ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
)

// ===================================================================
// FILE: frontend/src/index.css
// ===================================================================

/*
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

:root {
  --color-primary: #208091;
  --color-primary-hover: #1a7478;
  --color-primary-active: #1a6873;
  --color-secondary: #faf5f1;
  --color-surface: #fcfcf9;
  --color-text: #134252;
  --color-text-secondary: #626c70;
  --color-border: #5e5240;
  --color-error: #c0152f;
  --color-success: #208091;
  --color-warning: #a84b2f;
  --color-bg: #faf5f1;
  
  --font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  --border-radius: 8px;
  --shadow-sm: 0 1px 3px rgba(0, 0, 0, 0.1);
  --shadow-md: 0 4px 6px rgba(0, 0, 0, 0.1);
}

body {
  font-family: var(--font-family);
  background-color: var(--color-bg);
  color: var(--color-text);
  line-height: 1.5;
}

button {
  font-family: var(--font-family);
  cursor: pointer;
  border: none;
  border-radius: var(--border-radius);
  transition: all 0.2s ease;
}

button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

input,
select,
textarea {
  font-family: var(--font-family);
  border-radius: var(--border-radius);
  border: 1px solid var(--color-border);
  padding: 8px 12px;
}

input:focus,
select:focus,
textarea:focus {
  outline: none;
  border-color: var(--color-primary);
  box-shadow: 0 0 0 3px rgba(32, 128, 145, 0.1);
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}
*/

// ===================================================================
// FILE: frontend/src/api/apiClient.js
// ===================================================================

/*
import axios from 'axios'

const BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7000/api/v1'

const apiClient = axios.create({
  baseURL: BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Request interceptor
apiClient.interceptors.request.use(
  (config) => {
    // Adicionar token JWT se necessário
    // const token = localStorage.getItem('token')
    // if (token) {
    //   config.headers.Authorization = `Bearer ${token}`
    // }
    return config
  },
  (error) => Promise.reject(error)
)

// Response interceptor
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Redirecionar para login se necessário
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export default apiClient
*/

// ===================================================================
// FILE: frontend/src/api/leadsApi.js
// ===================================================================

/*
import apiClient from './apiClient'

export const leadsApi = {
  // Buscar leads por status
  getLeadsByStatus: (status) => 
    apiClient.get(`/leads/status/${status}`),

  // Buscar lead específico
  getLeadById: (id) =>
    apiClient.get(`/leads/${id}`),

  // Criar novo lead
  createLead: (leadData) =>
    apiClient.post('/leads', leadData),

  // Aceitar lead
  acceptLead: (id) =>
    apiClient.post(`/leads/${id}/accept`),

  // Recusar lead
  declineLead: (id) =>
    apiClient.post(`/leads/${id}/decline`),
}
*/

// ===================================================================
// FILE: frontend/src/hooks/useLeads.js
// ===================================================================

/*
import { useState, useEffect } from 'react'
import { leadsApi } from '../api/leadsApi'

export const useLeads = (status) => {
  const [leads, setLeads] = useState([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)

  const fetchLeads = async () => {
    try {
      setLoading(true)
      setError(null)
      const response = await leadsApi.getLeadsByStatus(status)
      setLeads(response.data)
    } catch (err) {
      setError(err.message || 'Erro ao carregar leads')
      console.error('Error fetching leads:', err)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchLeads()
  }, [status])

  const refetch = () => fetchLeads()

  return { leads, loading, error, refetch }
}

export const useLeadActions = () => {
  const [actionLoading, setActionLoading] = useState(false)
  const [actionError, setActionError] = useState(null)

  const acceptLead = async (id) => {
    try {
      setActionLoading(true)
      setActionError(null)
      await leadsApi.acceptLead(id)
      return true
    } catch (err) {
      setActionError(err.message || 'Erro ao aceitar lead')
      console.error('Error accepting lead:', err)
      return false
    } finally {
      setActionLoading(false)
    }
  }

  const declineLead = async (id) => {
    try {
      setActionLoading(true)
      setActionError(null)
      await leadsApi.declineLead(id)
      return true
    } catch (err) {
      setActionError(err.message || 'Erro ao recusar lead')
      console.error('Error declining lead:', err)
      return false
    } finally {
      setActionLoading(false)
    }
  }

  return { acceptLead, declineLead, actionLoading, actionError }
}
*/

// ===================================================================
// FILE: frontend/src/components/common/Header.jsx
// ===================================================================

/*
import React from 'react'
import './Header.css'

export function Header() {
  return (
    <header className="header">
      <div className="header-content">
        <h1>Leads Management System</h1>
        <p>Gerenciamento profissional de leads de vendas</p>
      </div>
    </header>
  )
}
*/

// ===================================================================
// FILE: frontend/src/components/common/Header.css
// ===================================================================

/*
.header {
  background: linear-gradient(135deg, #208091 0%, #1a7478 100%);
  color: white;
  padding: 40px 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  margin-bottom: 30px;
}

.header-content {
  max-width: 1200px;
  margin: 0 auto;
}

.header h1 {
  font-size: 32px;
  margin-bottom: 8px;
  font-weight: 600;
}

.header p {
  font-size: 14px;
  opacity: 0.9;
  margin: 0;
}
*/

// ===================================================================
// FILE: frontend/src/components/common/TabNavigation.jsx
// ===================================================================

/*
import React from 'react'
import './TabNavigation.css'

export function TabNavigation({ tabs, activeTab, onTabChange }) {
  return (
    <nav className="tab-navigation">
      {tabs.map((tab) => (
        <button
          key={tab.id}
          className={`tab-button ${activeTab === tab.id ? 'active' : ''}`}
          onClick={() => onTabChange(tab.id)}
        >
          {tab.icon && <span className="tab-icon">{tab.icon}</span>}
          {tab.label}
          {tab.count !== undefined && (
            <span className="tab-count">{tab.count}</span>
          )}
        </button>
      ))}
    </nav>
  )
}
*/

// ===================================================================
// FILE: frontend/src/components/common/TabNavigation.css
// ===================================================================

/*
.tab-navigation {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
  border-bottom: 2px solid #e0e0e0;
}

.tab-button {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 16px;
  background: none;
  border: none;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  color: #626c70;
  border-bottom: 3px solid transparent;
  transition: all 0.2s ease;
  margin-bottom: -2px;
}

.tab-button:hover {
  color: #208091;
}

.tab-button.active {
  color: #208091;
  border-bottom-color: #208091;
}

.tab-icon {
  font-size: 18px;
}

.tab-count {
  background: #208091;
  color: white;
  border-radius: 12px;
  padding: 2px 8px;
  font-size: 12px;
  font-weight: 600;
}
*/

// ===================================================================
// FILE: frontend/src/components/leads/LeadCard.jsx
// ===================================================================

/*
import React from 'react'
import './LeadCard.css'

export function LeadCard({ lead, status, onAccept, onDecline, actionLoading }) {
  const formattedDate = new Date(lead.dateCreated).toLocaleDateString('pt-BR', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
  })

  const isInvitedStatus = status === 'Invited'

  return (
    <div className="lead-card">
      <div className="lead-card-header">
        <div className="lead-info-header">
          <h3 className="lead-name">
            {lead.contactFirstName} {lead.contactLastName || ''}
          </h3>
          <span className="lead-id">ID: {lead.id}</span>
        </div>
        <span className={`lead-status ${status.toLowerCase()}`}>
          {status}
        </span>
      </div>

      <div className="lead-card-body">
        <div className="lead-field">
          <label>Data de Criação</label>
          <p>{formattedDate}</p>
        </div>

        <div className="lead-field">
          <label>Região</label>
          <p>{lead.suburb}</p>
        </div>

        <div className="lead-field">
          <label>Categoria</label>
          <p>{lead.category}</p>
        </div>

        <div className="lead-field">
          <label>Descrição</label>
          <p>{lead.description}</p>
        </div>

        <div className="lead-field">
          <label>Preço</label>
          <p className="price">${lead.price.toFixed(2)}</p>
        </div>

        {!isInvitedStatus && (
          <>
            {lead.contactPhoneNumber && (
              <div className="lead-field">
                <label>Telefone</label>
                <p>{lead.contactPhoneNumber}</p>
              </div>
            )}

            {lead.contactEmail && (
              <div className="lead-field">
                <label>Email</label>
                <p>{lead.contactEmail}</p>
              </div>
            )}
          </>
        )}
      </div>

      {isInvitedStatus && (
        <div className="lead-card-footer">
          <button
            className="btn btn-accept"
            onClick={() => onAccept(lead.id)}
            disabled={actionLoading}
          >
            ✓ Aceitar
          </button>
          <button
            className="btn btn-decline"
            onClick={() => onDecline(lead.id)}
            disabled={actionLoading}
          >
            ✕ Recusar
          </button>
        </div>
      )}
    </div>
  )
}
*/

// ===================================================================
// FILE: frontend/src/components/leads/LeadCard.css
// ===================================================================

/*
.lead-card {
  background: white;
  border-radius: 8px;
  border: 1px solid #e0e0e0;
  padding: 20px;
  margin-bottom: 16px;
  transition: all 0.2s ease;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
}

.lead-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  transform: translateY(-2px);
}

.lead-card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 16px;
  padding-bottom: 12px;
  border-bottom: 1px solid #f0f0f0;
}

.lead-info-header {
  flex: 1;
}

.lead-name {
  font-size: 18px;
  font-weight: 600;
  color: #134252;
  margin-bottom: 4px;
}

.lead-id {
  font-size: 12px;
  color: #626c70;
  background: #f5f5f5;
  padding: 2px 8px;
  border-radius: 4px;
  display: inline-block;
}

.lead-status {
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
}

.lead-status.invited {
  background: #faf5f1;
  color: #a84b2f;
}

.lead-status.accepted {
  background: #e8f5f1;
  color: #208091;
}

.lead-status.declined {
  background: #fef0f0;
  color: #c0152f;
}

.lead-card-body {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
  margin-bottom: 16px;
}

.lead-field {
  display: flex;
  flex-direction: column;
}

.lead-field label {
  font-size: 12px;
  font-weight: 600;
  color: #626c70;
  text-transform: uppercase;
  margin-bottom: 4px;
  letter-spacing: 0.5px;
}

.lead-field p {
  font-size: 14px;
  color: #134252;
  margin: 0;
}

.lead-field .price {
  font-size: 18px;
  font-weight: 600;
  color: #208091;
}

.lead-card-footer {
  display: flex;
  gap: 10px;
  border-top: 1px solid #f0f0f0;
  padding-top: 12px;
}

.btn {
  flex: 1;
  padding: 10px 16px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 600;
  transition: all 0.2s ease;
  border: none;
  cursor: pointer;
}

.btn-accept {
  background: #208091;
  color: white;
}

.btn-accept:hover:not(:disabled) {
  background: #1a7478;
  transform: translateY(-1px);
}

.btn-decline {
  background: #f5f5f5;
  color: #c0152f;
}

.btn-decline:hover:not(:disabled) {
  background: #f0f0f0;
  transform: translateY(-1px);
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
*/

// ===================================================================
// FILE: frontend/src/components/leads/LeadsList.jsx
// ===================================================================

/*
import React from 'react'
import { LeadCard } from './LeadCard'
import './LeadsList.css'

export function LeadsList({ leads, status, onAccept, onDecline, loading, error, actionLoading }) {
  if (loading) {
    return <div className="leads-list-empty">Carregando leads...</div>
  }

  if (error) {
    return <div className="leads-list-error">Erro ao carregar leads: {error}</div>
  }

  if (leads.length === 0) {
    return (
      <div className="leads-list-empty">
        <p>Nenhum lead encontrado com o status "{status}"</p>
      </div>
    )
  }

  return (
    <div className="leads-list">
      {leads.map((lead) => (
        <LeadCard
          key={lead.id}
          lead={lead}
          status={status}
          onAccept={onAccept}
          onDecline={onDecline}
          actionLoading={actionLoading}
        />
      ))}
    </div>
  )
}
*/

// ===================================================================
// FILE: frontend/src/components/leads/LeadsList.css
// ===================================================================

/*
.leads-list {
  display: flex;
  flex-direction: column;
}

.leads-list-empty,
.leads-list-error {
  padding: 40px 20px;
  text-align: center;
  background: white;
  border-radius: 8px;
  color: #626c70;
  border: 1px dashed #e0e0e0;
}

.leads-list-error {
  color: #c0152f;
  background: #fef0f0;
  border-color: #f0d0d0;
}

.leads-list-empty p,
.leads-list-error p {
  margin: 0;
  font-size: 16px;
}
*/

// ===================================================================
// FILE: frontend/src/components/leads/LeadsContainer.jsx
// ===================================================================

/*
import React, { useState, useCallback } from 'react'
import { useLeads, useLeadActions } from '../../hooks/useLeads'
import { LeadsList } from './LeadsList'
import { TabNavigation } from '../common/TabNavigation'
import { toast } from 'react-toastify'

export function LeadsContainer() {
  const [activeTab, setActiveTab] = useState('Invited')

  const { leads, loading, error, refetch } = useLeads(activeTab)
  const { acceptLead, declineLead, actionLoading, actionError } = useLeadActions()

  const handleAccept = useCallback(
    async (leadId) => {
      const success = await acceptLead(leadId)
      if (success) {
        toast.success('Lead aceito com sucesso!')
        refetch()
      } else {
        toast.error(actionError || 'Erro ao aceitar lead')
      }
    },
    [acceptLead, actionError, refetch]
  )

  const handleDecline = useCallback(
    async (leadId) => {
      const success = await declineLead(leadId)
      if (success) {
        toast.success('Lead recusado com sucesso!')
        refetch()
      } else {
        toast.error(actionError || 'Erro ao recusar lead')
      }
    },
    [declineLead, actionError, refetch]
  )

  const tabs = [
    { id: 'Invited', label: '📨 Convidados', icon: '📨', count: leads.length },
    { id: 'Accepted', label: '✓ Aceitos', icon: '✓' },
    { id: 'Declined', label: '✕ Recusados', icon: '✕' },
  ]

  return (
    <div className="leads-container">
      <TabNavigation tabs={tabs} activeTab={activeTab} onTabChange={setActiveTab} />
      <LeadsList
        leads={leads}
        status={activeTab}
        onAccept={handleAccept}
        onDecline={handleDecline}
        loading={loading}
        error={error}
        actionLoading={actionLoading}
      />
    </div>
  )
}
*/

// ===================================================================
// FILE: frontend/src/App.jsx
// ===================================================================

/*
import React from 'react'
import { ToastContainer } from 'react-toastify'
import 'react-toastify/dist/ReactToastify.css'
import { Header } from './components/common/Header'
import { LeadsContainer } from './components/leads/LeadsContainer'
import './App.css'

function App() {
  return (
    <div className="app">
      <Header />
      <div className="container">
        <LeadsContainer />
      </div>
      <ToastContainer
        position="bottom-right"
        autoClose={3000}
        hideProgressBar={false}
        newestOnTop={true}
        closeOnClick
        rtl={false}
        pauseOnFocusLoss
        draggable
        pauseOnHover
      />
    </div>
  )
}

export default App
*/

// ===================================================================
// FILE: frontend/src/App.css
// ===================================================================

/*
.app {
  min-height: 100vh;
  background: #faf5f1;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}
*/

// ===================================================================
// FILE: frontend/index.html
// ===================================================================

/*
<!doctype html>
<html lang="pt-BR">
  <head>
    <meta charset="UTF-8" />
    <link rel="icon" type="image/svg+xml" href="/vite.svg" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Leads Management System</title>
  </head>
  <body>
    <div id="root"></div>
    <script type="module" src="/src/main.jsx"></script>
  </body>
</html>
*/
