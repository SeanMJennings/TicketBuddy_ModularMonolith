import styled from "styled-components";

export const EventList = styled.div`
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
    gap: 24px;
    padding: 0;
    max-width: 1200px;
    margin: 0 auto;
    
    @media (max-width: 768px) {
        grid-template-columns: 1fr;
        gap: 16px;
    }
`;

export const EventItem = styled.div`
    position: relative;
    padding: 24px;
    background: var(--gradient-card);
    border: 1px solid rgba(255, 255, 255, 0.1);
    border-radius: 16px;
    box-shadow: var(--shadow-lg);
    backdrop-filter: blur(20px);
    transition: all var(--transition-normal);
    overflow: hidden;
    
    &::before {
        content: '';
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        height: 3px;
        background: var(--gradient-primary);
        opacity: 0;
        transition: opacity var(--transition-normal);
        pointer-events: none; /* Prevent blocking clicks */
    }
    
    &:hover {
        transform: translateY(-8px);
        box-shadow: var(--shadow-xl), var(--shadow-glow);
        border-color: rgba(14, 165, 233, 0.3);
        
        &::before {
            opacity: 1;
        }
        
        &::after {
            opacity: 1;
            transform: translateX(100%);
        }
    }
    
    &::after {
        content: '';
        position: absolute;
        top: 0;
        left: -100%;
        width: 100%;
        height: 100%;
        background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.05), transparent);
        transition: all var(--transition-slow);
        opacity: 0;
        pointer-events: none;
    }
    
    h2 {
        margin: 0 0 16px 0;
        font-size: 1.5rem;
        font-weight: 700;
        color: var(--gray-100);
        background: var(--gradient-text);
        -webkit-background-clip: text;
        -webkit-text-fill-color: transparent;
        background-clip: text;
        line-height: 1.3;
    }
    
    p {
        margin: 0 0 12px 0;
        color: var(--gray-300);
        font-size: 0.95rem;
        line-height: 1.5;
        
        &:last-of-type {
            margin-bottom: 20px;
        }
    }
    
    button {
        margin-top: auto;
        position: relative;
        z-index: 1; /* Ensure button is above pseudo-elements */
    }
`;

export const PageTitle = styled.h1`
    text-align: center;
    margin: 10px 0 40px 0;
    font-size: 3rem;
    font-weight: 800;
    background: var(--gradient-text);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
    letter-spacing: -0.02em;
    position: relative;
    
    &::after {
        content: '';
        position: absolute;
        bottom: -12px;
        left: 50%;
        transform: translateX(-50%);
        width: 80px;
        height: 4px;
        background: var(--gradient-primary);
        border-radius: 2px;
    }
    
    @media (max-width: 768px) {
        font-size: 2.5rem;
        margin-bottom: 32px;
    }
`;

export const Container = styled.div`
  margin: 2rem auto;
`;

export const PageContainer = styled.div`
    max-width: 1200px;
    margin: 0 auto;
    width: 100%;
`;

export const ActionBar = styled.div`
    display: flex;
    justify-content: flex-start;
    margin-bottom: 24px;
    
    @media (max-width: 768px) {
        justify-content: center;
    }
`;
