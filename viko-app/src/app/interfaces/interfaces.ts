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

export interface CategoriesObjectFormat {
    index: number,
    name: string,
    emoji?: string
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
    guid:string;
}

export interface CreateEventDto {
    Title: string;
    Description: string;
    Category: string;
    //Teacher: number => serve Teachers UserID in a select box
    Language: string;
    City: string;
    Image: string | undefined;
    Location: string;
    StartDate: string;
    // registrationDeadline: string;
    EndDate: string;

}

export interface EventFetched {
    title: string;
    description: string;
    category: string;
    teacher: string;
    teacherPhoto: string;
    language: string;
    city: string;
    image: string;
    eventStatus: number;
    location: string;
    startDate: string;
    endDate: string;
    registrationDeadline: string;
    guid: string;
}