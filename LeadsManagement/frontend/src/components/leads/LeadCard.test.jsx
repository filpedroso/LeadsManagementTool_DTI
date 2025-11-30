import { describe, it, expect, vi } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/react'
import { LeadCard } from './LeadCard'

describe('LeadCard', () => {
  const mockLead = {
    id: 1,
    contactFirstName: 'John',
    contactLastName: 'Doe',
    suburb: 'Sydney',
    category: 'Plumbing',
    description: 'Fix kitchen sink leak',
    price: 150,
    contactPhoneNumber: '0412345678',
    contactEmail: 'john@example.com',
    dateCreated: '2025-11-30T10:30:00Z'
  }

  describe('Invited Status', () => {
    it('should render invited lead card correctly', () => {
      render(
        <LeadCard
          lead={mockLead}
          status="Invited"
          onAccept={vi.fn()}
          onDecline={vi.fn()}
          actionLoading={false}
        />
      )

      expect(screen.getByText('John')).toBeInTheDocument()
      expect(screen.getByText('J')).toBeInTheDocument() // Avatar
      expect(screen.getByText('Sydney')).toBeInTheDocument()
      expect(screen.getByText('Plumbing')).toBeInTheDocument()
      expect(screen.getByText('Fix kitchen sink leak')).toBeInTheDocument()
      expect(screen.getByText('Accept')).toBeInTheDocument()
      expect(screen.getByText('Decline')).toBeInTheDocument()
    })

    it('should show only first name for invited status', () => {
      render(
        <LeadCard
          lead={mockLead}
          status="Invited"
          onAccept={vi.fn()}
          onDecline={vi.fn()}
          actionLoading={false}
        />
      )

      expect(screen.getByText('John')).toBeInTheDocument()
      expect(screen.queryByText('John Doe')).not.toBeInTheDocument()
    })

    it('should call onAccept when Accept button is clicked', () => {
      const onAccept = vi.fn()

      render(
        <LeadCard
          lead={mockLead}
          status="Invited"
          onAccept={onAccept}
          onDecline={vi.fn()}
          actionLoading={false}
        />
      )

      fireEvent.click(screen.getByText('Accept'))
      expect(onAccept).toHaveBeenCalledWith(1)
    })

    it('should call onDecline when Decline button is clicked', () => {
      const onDecline = vi.fn()

      render(
        <LeadCard
          lead={mockLead}
          status="Invited"
          onAccept={vi.fn()}
          onDecline={onDecline}
          actionLoading={false}
        />
      )

      fireEvent.click(screen.getByText('Decline'))
      expect(onDecline).toHaveBeenCalledWith(1)
    })

    it('should disable buttons when actionLoading is true', () => {
      render(
        <LeadCard
          lead={mockLead}
          status="Invited"
          onAccept={vi.fn()}
          onDecline={vi.fn()}
          actionLoading={true}
        />
      )

      expect(screen.getByText('Accept')).toBeDisabled()
      expect(screen.getByText('Decline')).toBeDisabled()
    })

    it('should display price with Lead Invitation text', () => {
      render(
        <LeadCard
          lead={mockLead}
          status="Invited"
          onAccept={vi.fn()}
          onDecline={vi.fn()}
          actionLoading={false}
        />
      )

      expect(screen.getByText('$150.00 Lead Invitation')).toBeInTheDocument()
    })

    it('should not show contact details in invited status', () => {
      render(
        <LeadCard
          lead={mockLead}
          status="Invited"
          onAccept={vi.fn()}
          onDecline={vi.fn()}
          actionLoading={false}
        />
      )

      expect(screen.queryByText('0412345678')).not.toBeInTheDocument()
      expect(screen.queryByText('john@example.com')).not.toBeInTheDocument()
    })
  })

  describe('Accepted Status', () => {
    it('should render accepted lead card correctly', () => {
      render(
        <LeadCard
          lead={mockLead}
          status="Accepted"
          onAccept={vi.fn()}
          onDecline={vi.fn()}
          actionLoading={false}
        />
      )

      expect(screen.getByText('John Doe')).toBeInTheDocument()
      expect(screen.getByText('0412345678')).toBeInTheDocument()
      expect(screen.getByText('john@example.com')).toBeInTheDocument()
    })

    it('should show full name for accepted status', () => {
      render(
        <LeadCard
          lead={mockLead}
          status="Accepted"
          onAccept={vi.fn()}
          onDecline={vi.fn()}
          actionLoading={false}
        />
      )

      expect(screen.getByText('John Doe')).toBeInTheDocument()
    })

    it('should not show action buttons for accepted status', () => {
      render(
        <LeadCard
          lead={mockLead}
          status="Accepted"
          onAccept={vi.fn()}
          onDecline={vi.fn()}
          actionLoading={false}
        />
      )

      expect(screen.queryByText('Accept')).not.toBeInTheDocument()
      expect(screen.queryByText('Decline')).not.toBeInTheDocument()
    })

    it('should show contact details for accepted status', () => {
      render(
        <LeadCard
          lead={mockLead}
          status="Accepted"
          onAccept={vi.fn()}
          onDecline={vi.fn()}
          actionLoading={false}
        />
      )

      expect(screen.getByText('0412345678')).toBeInTheDocument()
      expect(screen.getByText('john@example.com')).toBeInTheDocument()
    })
  })

  describe('Declined Status', () => {
    it('should render declined lead card correctly', () => {
      render(
        <LeadCard
          lead={mockLead}
          status="Declined"
          onAccept={vi.fn()}
          onDecline={vi.fn()}
          actionLoading={false}
        />
      )

      expect(screen.getByText('John Doe')).toBeInTheDocument()
      expect(screen.getByText('Declined')).toBeInTheDocument()
    })

    it('should not show action buttons for declined status', () => {
      render(
        <LeadCard
          lead={mockLead}
          status="Declined"
          onAccept={vi.fn()}
          onDecline={vi.fn()}
          actionLoading={false}
        />
      )

      expect(screen.queryByText('Accept')).not.toBeInTheDocument()
      expect(screen.queryByText('Decline')).not.toBeInTheDocument()
    })
  })

  describe('Avatar Circle', () => {
    it('should display first letter of first name in avatar', () => {
      render(
        <LeadCard
          lead={mockLead}
          status="Invited"
          onAccept={vi.fn()}
          onDecline={vi.fn()}
          actionLoading={false}
        />
      )

      expect(screen.getByText('J')).toBeInTheDocument()
    })

    it('should handle lowercase first name', () => {
      const lead = { ...mockLead, contactFirstName: 'alice' }
      
      render(
        <LeadCard
          lead={lead}
          status="Invited"
          onAccept={vi.fn()}
          onDecline={vi.fn()}
          actionLoading={false}
        />
      )

      expect(screen.getByText('A')).toBeInTheDocument()
    })
  })

  describe('Date Formatting', () => {
    it('should format date correctly for invited status', () => {
      render(
        <LeadCard
          lead={mockLead}
          status="Invited"
          onAccept={vi.fn()}
          onDecline={vi.fn()}
          actionLoading={false}
        />
      )

      // Check that date is formatted (exact format depends on timezone)
      expect(screen.getByText(/November 30 @ .* - 2025/)).toBeInTheDocument()
    })
  })

  describe('Price Display', () => {
    it('should format price with two decimal places', () => {
      render(
        <LeadCard
          lead={mockLead}
          status="Invited"
          onAccept={vi.fn()}
          onDecline={vi.fn()}
          actionLoading={false}
        />
      )

      expect(screen.getByText('$150.00 Lead Invitation')).toBeInTheDocument()
    })

    it('should handle decimal prices correctly', () => {
      const lead = { ...mockLead, price: 99.99 }
      
      render(
        <LeadCard
          lead={lead}
          status="Invited"
          onAccept={vi.fn()}
          onDecline={vi.fn()}
          actionLoading={false}
        />
      )

      expect(screen.getByText('$99.99 Lead Invitation')).toBeInTheDocument()
    })
  })
})
