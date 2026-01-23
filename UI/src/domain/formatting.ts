import moment from "moment";

export const formatCurrency = (amount: number): string => {
    return `£${amount.toFixed(2)}`;
};

export const getInitials = (fullName: string): string => {
    return fullName
        .split(' ')
        .map(name => name.charAt(0))
        .join('')
        .toUpperCase()
        .slice(0, 2);
};

export const formatDate = (dateString: string): string => {
    return moment(dateString).format('DD MMM YYYY');
};
