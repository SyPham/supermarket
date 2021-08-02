import { AccountType } from "./account.type";

export interface Account {
  id: number;
  username: string;
  fullName: string;
  password: string;
  email: string;
  isLock: boolean;
  accountTypeId: number | null;
  createdBy: number;
  modifiedBy: number | null;
  createdTime: string;
  group: string;
  modifiedTime: string | null;
  accountType: AccountType;
}
