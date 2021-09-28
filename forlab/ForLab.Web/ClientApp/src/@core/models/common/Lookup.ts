
export class Lookup {
    id?: number = 0;
    name: string = null;
    isDeleted: boolean = false;
    isActive: boolean = true;
    createdBy: number;
    createdOn?: string = new Date().toISOString();
    updatedBy?: number;
    updatedOn?: string;
    
    // UI
    creator: string;
    updator: string;
}
