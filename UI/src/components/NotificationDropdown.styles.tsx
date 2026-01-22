import styled from "styled-components";

export const DropdownContainer = styled.div`
    position: absolute;
    top: calc(100% + 8px);
    right: 0;
    width: 320px;
    max-height: 400px;
    overflow-y: auto;
    background: rgba(15, 23, 42, 0.98);
    border: 1px solid rgba(255, 255, 255, 0.1);
    border-radius: 12px;
    box-shadow: var(--shadow-lg);
    z-index: 1001;
`;

export const NotificationItem = styled.div<{ $unread: boolean }>`
    padding: 16px;
    border-bottom: 1px solid rgba(255, 255, 255, 0.05);
    cursor: pointer;
    transition: background var(--transition-fast);
    background: ${({ $unread }) => $unread ? "rgba(14, 165, 233, 0.1)" : "transparent"};
    font-weight: ${({ $unread }) => $unread ? "600" : "400"};

    &:hover {
        background: rgba(14, 165, 233, 0.15);
    }

    &:last-child {
        border-bottom: none;
    }
`;

export const NotificationTitle = styled.div`
    color: var(--gray-100);
    font-size: 0.9rem;
    margin-bottom: 4px;
`;

export const NotificationTime = styled.div`
    color: var(--gray-400);
    font-size: 0.75rem;
`;

export const EmptyState = styled.div`
    padding: 32px 16px;
    text-align: center;
    color: var(--gray-400);
`;

export const LoadingState = styled.div`
    padding: 32px 16px;
    text-align: center;
    color: var(--gray-400);
`;
