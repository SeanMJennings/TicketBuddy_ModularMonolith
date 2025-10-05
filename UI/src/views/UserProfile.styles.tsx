import styled from 'styled-components';

export const ProfileContainer = styled.div`
    max-width: 800px;
    margin: 0 auto;
    padding: 0 24px;
`;

export const UserCard = styled.div`
    background: var(--gradient-card);
    border: 1px solid rgba(255, 255, 255, 0.1);
    border-radius: 16px;
    padding: 32px;
    margin-bottom: 32px;
    box-shadow: var(--shadow-lg);
    backdrop-filter: blur(20px);
    position: relative;
    overflow: hidden;
    
    &::before {
        content: '';
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        height: 4px;
        background: var(--gradient-primary);
    }
    
    &::after {
        content: '';
        position: absolute;
        top: -50%;
        right: -50%;
        width: 100px;
        height: 100px;
        background: radial-gradient(circle, rgba(14, 165, 233, 0.1) 0%, transparent 70%);
        border-radius: 50%;
        pointer-events: none;
    }
`;

export const UserAvatar = styled.div`
    width: 80px;
    height: 80px;
    border-radius: 50%;
    background: var(--gradient-primary);
    display: flex;
    align-items: center;
    justify-content: center;
    color: white;
    font-size: 32px;
    font-weight: 600;
    margin-bottom: 16px;
    box-shadow: var(--shadow-md);
`;

export const UserName = styled.h2`
    font-size: 28px;
    font-weight: 700;
    margin: 0 0 8px 0;
    background: var(--gradient-text);
    background-clip: text;
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
`;

export const UserEmail = styled.p`
    font-size: 16px;
    margin: 0;
    opacity: 0.8;
`;

export const SectionTitle = styled.h3`
    font-size: 24px;
    font-weight: 600;
    margin: 0 0 24px 0;
    display: flex;
    align-items: center;
    gap: 12px;
    
    &::before {
        content: '🎫';
        font-size: 20px;
    }
`;

export const TicketsGrid = styled.div`
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
    gap: 20px;
    margin-top: 16px;
    
    @media (max-width: 768px) {
        grid-template-columns: 1fr;
        gap: 16px;
    }
`;

export const TicketCard = styled.div`
    background: var(--gradient-card);
    border: 1px solid rgba(255, 255, 255, 0.1);
    border-radius: 12px;
    padding: 20px;
    position: relative;
    transition: all var(--transition-normal);
    backdrop-filter: blur(20px);
    overflow: hidden;
    
    &::before {
        content: '';
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        height: 3px;
        background: linear-gradient(90deg, #10b981, #06d6a0);
    }
    
    &::after {
        content: '';
        position: absolute;
        top: 50%;
        right: -20px;
        width: 40px;
        height: 40px;
        background: radial-gradient(circle, rgba(16, 185, 129, 0.15) 0%, transparent 70%);
        border-radius: 50%;
        transform: translateY(-50%);
        pointer-events: none;
    }
    
    &:hover {
        transform: translateY(-4px);
        box-shadow: var(--shadow-xl), var(--shadow-glow);
        border-color: rgba(16, 185, 129, 0.3);
    }
`;

export const TicketHeader = styled.div`
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    margin-bottom: 16px;
`;

export const SeatNumber = styled.div`
    background: rgba(16, 185, 129, 0.2);
    color: #10b981;
    padding: 6px 12px;
    border-radius: 8px;
    font-weight: 600;
    font-size: 14px;
    border: 1px solid rgba(16, 185, 129, 0.3);
`;

export const TicketPrice = styled.div`
    font-size: 24px;
    font-weight: 700;
    background: var(--gradient-text);
    background-clip: text;
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
`;

export const TicketMeta = styled.div`
    display: flex;
    flex-direction: column;
    gap: 8px;
    padding-top: 12px;
    border-top: 1px solid rgba(255, 255, 255, 0.1);
`;

export const TicketDetail = styled.div`
    font-size: 14px;
    display: flex;
    align-items: center;
    gap: 8px;
    
    &::before {
        content: '•';
        color: rgba(16, 185, 129, 0.6);
        font-weight: bold;
    }
`;

export const EmptyState = styled.div`
    text-align: center;
    
    .emoji {
        font-size: 48px;
        margin-bottom: 16px;
        display: block;
    }
    
    h4 {
        margin: 0 0 8px 0;
        font-size: 20px;
        font-weight: 600;
    }
    
    p {
        margin: 0;
        opacity: 0.8;
        font-size: 16px;
    }
`;

export const StatsGrid = styled.div`
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
    gap: 16px;
    margin-bottom: 32px;
`;

export const StatCard = styled.div`
    background: var(--gradient-card);
    border: 1px solid rgba(255, 255, 255, 0.1);
    border-radius: 12px;
    padding: 20px;
    text-align: center;
    backdrop-filter: blur(20px);
    
    .stat-value {
        font-size: 32px;
        font-weight: 700;
        margin: 0 0 4px 0;
        background: var(--gradient-text);
        background-clip: text;
        -webkit-background-clip: text;
        -webkit-text-fill-color: transparent;
    }
    
    .stat-label {
        font-size: 14px;
        margin: 0;
        opacity: 0.8;
    }
`;
