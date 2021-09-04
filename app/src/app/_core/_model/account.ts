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
  group_id: number;
  team_id: number;
  modifiedTime: string | null;
  accountType: AccountType;
}
