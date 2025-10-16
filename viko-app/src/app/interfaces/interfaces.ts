export interface LoginRequest {
    username: string;
    password: string;
}

export interface LoginResponse {
    token: string;
}

export interface UserInfo {
    photo: string;
    name: string;
    username: string;
    email: string;
    language: string;
    birthdate: string;
    phone: string;
}