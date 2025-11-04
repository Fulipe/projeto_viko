export interface LoginRequest {
    username: string;
    password: string;
}

export interface LoginResponse {
    token?: string;
    msg: string,
}

export interface LanguagesObjectFormat {
    index: number,
    name: string,
    flag?: string
}

export interface UserInfo {
    name: string;
    email: string;
    username: string;
    language: string;
    birthdate: string;
    phone: string;
    photo: string;
    role: string;
}

export interface EventItem {
    title: string;
}

export interface CreateEventDto {
    title: string;
    description: string;
    category: string;
    language: string;
    city: string;
    image: string;
    location: string;
    startDate: string;
    registrationDeadline: string;
    endDate: string;

}

export interface EventFetched {
    title: string;
    description: string;
    category: string;
    teacher: string;
    language: string;
    //city: string;
    image: string;
    eventStatus: number;
    location: string;
    startDate: string;
    endDate: string;
    registrationDeadline: string;
    guid: string;
}