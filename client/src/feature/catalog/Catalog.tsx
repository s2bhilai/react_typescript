import { Button } from "@mui/material";
import { useEffect, useState } from "react";
import agent from "../../app/api/agent";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { Product } from "../../app/models/product";
import { useAppDispatch, useAppSelector } from "../../app/store/configureStore";
import { fetchProductsAsync, productSelectors } from "./catalogSlice";
import ProductList from "./ProductList";

export default function Catalog() {
  //const [products, setProducts] = useState<Product[]>([]);
  //const [loading, setLoading] = useState(true);
  const products = useAppSelector(productSelectors.selectAll);
  const { productsLoaded, status } = useAppSelector((state) => state.catalog);
  const dispatch = useAppDispatch();

  useEffect(() => {
    if (!productsLoaded) dispatch(fetchProductsAsync());
  }, [productsLoaded, dispatch]);

  if (status.includes("pending"))
    return <LoadingComponent message="Loading products..." />;

  // function addProduct() {
  //   setProducts((prevState) => {
  //     return [
  //       ...prevState,
  //       {
  //         id: prevState.length + 101,
  //         name: "Product" + (prevState.length + 1),
  //         price: prevState.length * 100 + 100,
  //         brand: "some brand",
  //         description: "some desc",
  //         pictureUrl: "https://picssum",
  //       },
  //     ];
  //   });
  // }

  return (
    <>
      <ProductList products={products} />
      {/* <Button variant="contained" onClick={addProduct}>
        Add Product
      </Button> */}
    </>
  );
}
