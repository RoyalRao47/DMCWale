export type ProfileDetails = {
    firstName: string;
    lastName: string;
    mobile: string;
    salutation: string;
    countryCode: string;
    city: string;
    address1: string;
    address2: string;
    signature: string;
    username: string;
    email: string;
    agentSupplierCode: string;
    roleName: string;
    profileImagePath?: string | null;
};

export type UpdateProfileRequest = {
    firstName: string;
    lastName: string;
    email: string;
    mobile: string;
    city: string;
    address1: string;
    salutation: string;
    countryCode: string;
    address2: string;
    signature: string;
};

export type UpdateProfileResponse = {
    message: string;
    profile: ProfileDetails;
};
