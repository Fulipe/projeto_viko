export interface LoginRequest {
    username: string;
    password: string;
}

export interface LoginResponse {
    token: string;
}

export interface UserInfo {
    Name: string;
    Username: string;
    Email: string;
    Language: string;
    Birthdate: string;
    Phone: string;
}