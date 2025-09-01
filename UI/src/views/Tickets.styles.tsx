﻿import styled from 'styled-components';

export const TicketsContainer = styled.div`
  margin: 2rem 0;
`;

export const SeatMapContainer = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin: 2rem auto;
  max-width: 600px;
`;

export const SeatRow = styled.div`
  display: flex;
  justify-content: center;
  gap: 10px;
`;

export const Seat = styled.div<{ isbooked: boolean }>`
  width: 40px;
  height: 40px;
  border: 2px solid #ccc;
  border-radius: 5px;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: ${props => props.isbooked ? '#f5f5f5' : '#4CAF50'};
  color: ${props => props.isbooked ? '#999' : '#fff'};
  font-weight: bold;
  cursor: ${props => props.isbooked ? 'not-allowed' : 'pointer'};
  
  &:hover {
    transform: ${props => props.isbooked ? 'none' : 'scale(1.05)'};
  }
  
  &.booked {
    background-color: #f5f5f5;
    color: #999;
    cursor: not-allowed;
  }
`;

export const ScreenArea = styled.div`
  height: 30px;
  background-color: #ddd;
  margin-bottom: 2rem;
  border-radius: 5px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #666;
  font-size: 0.8rem;
  letter-spacing: 1px;
`;

export const PriceInfo = styled.div`
  text-align: center;
  margin: 1rem 0;
  font-weight: bold;
`;

export const Legend = styled.div`
  display: flex;
  justify-content: center;
  gap: 20px;
  margin: 1rem 0;
`;

export const LegendItem = styled.div`
  display: flex;
  align-items: center;
  gap: 5px;
`;

export const LegendColor = styled.div<{ color: string }>`
  width: 15px;
  height: 15px;
  background-color: ${props => props.color};
  border-radius: 3px;
`;

export const EventTitle = styled.h1`
  text-align: center;
  margin-bottom: 2rem;
`;
