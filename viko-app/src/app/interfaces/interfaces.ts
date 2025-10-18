export interface LoginRequest {
    username: string;
    password: string;
}

export interface LoginResponse {
    token: string;
}

export interface UserInfo {
    name: string;
    email: string;
    username: string;
    language: string;
    birthdate: string;
    phone: string;
    photo: string;
}