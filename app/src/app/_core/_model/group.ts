export interface Group {
  id: number;
  name: string;
  createdBy: number;
  modifiedBy: number | null;
  createdTime: string;
  modifiedTime: string | null;
  status: boolean
}
