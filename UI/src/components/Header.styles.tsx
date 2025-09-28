import styled from "styled-components";
import TicketStub from "../assets/ticket-stub.svg";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faUser } from '@fortawesome/free-solid-svg-icons';
import { Link } from 'react-router-dom';

export const HeaderBar = styled.div`
    display: flex;
    align-items: center;
    padding: 16px 32px;
    background: rgba(15, 23, 42, 0.95);
    backdrop-filter: blur(20px);
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    z-index: 1000;
    
    &::before {
        content: '';
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        height: 1px;
        background: var(--gradient-primary);
        opacity: 0.6;
    }
    
    > h1 {
        margin: 0 0 0 16px;
        font-size: 1.5rem;
        font-weight: 700;
        background: var(--gradient-text);
        -webkit-background-clip: text;
        -webkit-text-fill-color: transparent;
        background-clip: text;
        letter-spacing: -0.025em;
    }
`

const StyledTicketLink = styled(Link)`
    display: flex;
    align-items: center;
    transition: all var(--transition-normal);
    border-radius: 12px;
    padding: 8px;
    position: relative;
    overflow: hidden;
    
    &::before {
        content: '';
        position: absolute;
        top: 0;
        left: -100%;
        width: 100%;
        height: 100%;
        background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.1), transparent);
        transition: left var(--transition-slow);
    }
    
    &:hover {
        transform: scale(1.05);
        
        &::before {
            left: 100%;
        }
        
        img {
            filter: drop-shadow(0 0 20px rgba(14, 165, 233, 0.4));
        }
    }
    
    &:active {
        transform: scale(0.98);
    }
    
    img {
        transition: filter var(--transition-normal);
    }
    
    &::after {
        display: none;
    }
`;

export const TicketStubImage = () => (
    <StyledTicketLink to="/">
        <img src={TicketStub} alt="Ticket Stub" width={120} height={80} />
    </StyledTicketLink>
);

export const Container = styled.div`
    margin-left: auto;
    display: flex;
    align-items: center;
    gap: 24px;
`;

export const UserIconContainer = styled.div`
    font-size: 1.25rem;
    cursor: pointer;
    padding: 12px;
    border-radius: 50%;
    background: rgba(255, 255, 255, 0.05);
    border: 1px solid rgba(255, 255, 255, 0.1);
    transition: all var(--transition-normal);
    position: relative;
    
    &:hover {
        background: rgba(14, 165, 233, 0.1);
        border-color: var(--primary-500);
        transform: translateY(-2px);
        box-shadow: var(--shadow-md);
        color: var(--primary-300);
    }
    
    &:active {
        transform: translateY(0);
    }
`;

export const UserIcon = () => <FontAwesomeIcon icon={faUser} />;

export const UsersDropdown = styled.select`
    padding: 12px 16px;
    font-size: 16px;
    border-radius: 8px;
    border: 1px solid var(--gray-600);
    background: rgba(255, 255, 255, 0.05);
    color: var(--gray-100);
    cursor: pointer;
    transition: all var(--transition-normal);
    backdrop-filter: blur(10px);
    min-width: 200px;

    &:focus {
        outline: none;
        border-color: var(--primary-500);
        box-shadow: 0 0 0 3px rgba(14, 165, 233, 0.1);
        background: rgba(255, 255, 255, 0.08);
    }
    
    &:hover {
        border-color: var(--primary-400);
        background: rgba(255, 255, 255, 0.08);
    }
    
    option {
        background: var(--gray-800);
        color: var(--gray-100);
        padding: 12px;
    }
`;

export const UserDetails = styled.div`
    position: absolute;
    top: 80px;
    right: 10px;
    background: var(--gradient-card);
    border: 1px solid var(--gray-600);
    border-radius: 12px;
    padding: 20px;
    box-shadow: var(--shadow-xl);
    backdrop-filter: blur(20px);
    z-index: 1000;
    min-width: 280px;
    
    &::before {
        content: '';
        position: absolute;
        top: -8px;
        right: 260px;
        width: 16px;
        height: 16px;
        background: var(--gray-900);
        border: 1px solid var(--gray-600);
        border-bottom: none;
        border-right: none;
        transform: rotate(45deg);
    }
    
    h2 {
        margin: 0 0 12px 0;
        font-size: 1.25rem;
        color: var(--gray-100);
        font-weight: 600;
    }
    
    p {
        margin: 0 0 8px 0;
        font-size: 14px;
        color: var(--gray-300);
        
        &:last-child {
            margin-bottom: 0;
        }
    }
`;

export const EventsManagementLink = styled(Link)`
    font-weight: 600;
    color: var(--primary-400);
    text-decoration: none;
    padding: 12px 20px;
    border-radius: 8px;
    background: rgba(14, 165, 233, 0.1);
    border: 1px solid rgba(14, 165, 233, 0.2);
    transition: all var(--transition-normal);
    position: relative;
    overflow: hidden;
    
    &::before {
        content: '';
        position: absolute;
        top: 0;
        left: -100%;
        width: 100%;
        height: 100%;
        background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.1), transparent);
        transition: left var(--transition-slow);
    }
    
    &:hover {
        color: var(--primary-300);
        background: rgba(14, 165, 233, 0.2);
        border-color: var(--primary-500);
        transform: translateY(-2px);
        box-shadow: var(--shadow-md);
        
        &::before {
            left: 100%;
        }
    }
    
    &:active {
        transform: translateY(0);
    }
    
    &::after {
        display: none;
    }
`;