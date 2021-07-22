export interface Store {
  id: number;
  name: string;
  createdBy: number;
  modifiedBy: number | null;
  createdTime: string;
  modifiedTime: string | null;
}
