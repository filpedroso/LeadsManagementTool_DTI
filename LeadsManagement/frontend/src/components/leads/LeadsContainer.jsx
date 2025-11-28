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
