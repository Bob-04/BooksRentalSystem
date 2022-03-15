import { Profile } from '../publishers/profile/profile.model';

export interface Book {
    id?: number;
    title: string;//
    description: string;//
    imageUrl: string;
    pricePerDay: number;
    author: string;//
    category: number;
    pagesNumber: number;//
    language: string;//
    cover: number;//
    isAvailable?: boolean;
    publisher?: Profile;//


    // manufacturer: string;
    // model: string;
    // hasClimateControl: boolean;
    // numberOfSeats: number;
    // transmissionType: number;
    // dealer?: Profile;
}