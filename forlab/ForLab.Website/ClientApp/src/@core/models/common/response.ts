export class ResponseDto {
    isPassed: boolean;
    message: string;
    data: any;
}
export class QueryParamsDto {
    key: string;
    value: any;
}

export interface TableColumn<T> {
    label: string;
    property: keyof T | string;
    type: 'text' | 'image' | 'badge' | 'progress' | 'checkbox' | 'button' | 'date' | 'datetime' | 'price' | 'number' | 'int' | 'percent' | 'bool' | 'custom' | 'select';
    visible?: boolean;
    cssClasses?: string[];
}
