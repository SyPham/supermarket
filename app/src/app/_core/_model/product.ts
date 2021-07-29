export interface Product {
  id: number;
  vietnameseName: string;
  englishName: string;
  chineseName: string;
  description: string;
  originalPrice: number;
  avatar: string;
  createdBy: number;
  createdTime: string;
  modifiedTime: string | null;
  storeId: number;
  kindId: number,
  file: null,
  status: boolean
}
export interface FilterRequest {
  kindId: number
  storeId: number;
  langId: string;
}
