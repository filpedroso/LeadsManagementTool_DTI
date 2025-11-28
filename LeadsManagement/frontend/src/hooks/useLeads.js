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
