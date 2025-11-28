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
