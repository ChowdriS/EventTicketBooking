export interface ApiResponse<T = any> {
  $id: string;
  success: boolean;
  message: string;
  data: T | null;
  errors: any[] | null;
}

export interface PagedResponse<T = any> {
  items: T | null;
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}
