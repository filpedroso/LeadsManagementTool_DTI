import { describe, it, expect, beforeEach, afterEach } from 'vitest'
import MockAdapter from 'axios-mock-adapter'
import apiClient from './apiClient'
import { leadsApi } from './leadsApi'

describe('leadsApi', () => {
  let mock

  beforeEach(() => {
    mock = new MockAdapter(apiClient)
  })

  afterEach(() => {
    mock.restore()
  })

  describe('getLeadsByStatus', () => {
    it('should fetch leads by status successfully', async () => {
      const mockLeads = [
        {
          id: 1,
          contactFirstName: 'John',
          contactLastName: 'Doe',
          suburb: 'Sydney',
          category: 'Plumbing',
          description: 'Fix leak',
          price: 150,
          status: 0,
          dateCreated: '2025-11-30T10:00:00Z'
        }
      ]

      mock.onGet('/leads/status/Invited').reply(200, mockLeads)

      const response = await leadsApi.getLeadsByStatus('Invited')
      expect(response.data).toEqual(mockLeads)
    })

    it('should throw error when status fetch fails', async () => {
      mock.onGet('/leads/status/Invited').reply(404)

      await expect(leadsApi.getLeadsByStatus('Invited')).rejects.toThrow()
    })
  })

  describe('getLeadById', () => {
    it('should fetch a specific lead successfully', async () => {
      const mockLead = {
        id: 1,
        contactFirstName: 'John',
        contactLastName: 'Doe',
        status: 0
      }

      mock.onGet('/leads/1').reply(200, mockLead)

      const response = await leadsApi.getLeadById(1)
      expect(response.data).toEqual(mockLead)
    })

    it('should throw error when lead not found', async () => {
      mock.onGet('/leads/999').reply(404)

      await expect(leadsApi.getLeadById(999)).rejects.toThrow()
    })
  })

  describe('acceptLead', () => {
    it('should accept a lead successfully', async () => {
      const leadId = 1

      mock.onPost(`/leads/${leadId}/accept`).reply(200)

      await expect(leadsApi.acceptLead(leadId)).resolves.not.toThrow()
    })

    it('should throw error when accept fails', async () => {
      const leadId = 1

      mock.onPost(`/leads/${leadId}/accept`).reply(400)

      await expect(leadsApi.acceptLead(leadId)).rejects.toThrow()
    })
  })

  describe('declineLead', () => {
    it('should decline a lead successfully', async () => {
      const leadId = 1

      mock.onPost(`/leads/${leadId}/decline`).reply(200)

      await expect(leadsApi.declineLead(leadId)).resolves.not.toThrow()
    })

    it('should throw error when decline fails', async () => {
      const leadId = 1

      mock.onPost(`/leads/${leadId}/decline`).reply(400)

      await expect(leadsApi.declineLead(leadId)).rejects.toThrow()
    })
  })
})
