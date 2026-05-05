export type LoginRequest = {
    agentSupplierCode: string;
    email: string;
    password: string;
};

export type AuthUser = {
    token: string;
    userId: string;
    fullName: string;
    email: string;
    agentSupplierCode: string;
    role: string;
    walletAmount: number;
};

export type LoginResponse = AuthUser;
