export class ProductParams
{
  productName: string | undefined;
  vendorName: string[] | undefined;
  categoryName: string[] | undefined;
  status: boolean | undefined;
  minPrice : number | undefined;
  maxPrice : number | undefined;
  pageNumber = 1;
  pageSize = 12;

  constructor() {
  }
}
