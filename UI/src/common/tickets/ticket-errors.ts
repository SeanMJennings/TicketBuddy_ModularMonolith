import {toast} from "react-toastify";

export function handleError(error: ApiResponseError) {
    if (error.errors && Array.isArray(error.errors)) {
        error.errors.forEach((errorMessage: string) => {
            toast.error(errorMessage);
        });
    } else {
        toast.error('Failed to complete purchase. Please try again.');
    }
}

export type ApiResponseError = {
    errors: string[];
    code: number;
}