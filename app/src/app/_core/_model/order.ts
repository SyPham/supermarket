export interface Order {
  id: number;
  code: string;
  totalPrice: number | null;
  isDelete: boolean | null;
  status: number | null;
  fullName: string;
  employeeId: string;
  consumerId: number;
  createdBy: number | null;
  createdTime: string;
  modifiedTime: string | null;
}
