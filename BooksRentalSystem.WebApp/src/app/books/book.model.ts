import { Profile } from "../publishers/profile/profile.model";

export interface Book {
  id?: string;
  title: string; //
  description: string; //
  imageUrl: string;
  pricePerDay: number;
  author: string; //
  category: number;
  pagesNumber: number; //
  language: string; //
  coverType: number; //
  isAvailable?: boolean;
  publisher?: Profile; //
}
