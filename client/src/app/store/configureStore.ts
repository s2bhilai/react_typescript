//import { createStore } from "redux";
//import counterReducer from "../../feature/contact/counterReducer";

import { configureStore } from "@reduxjs/toolkit";
import { TypedUseSelectorHook, useDispatch, useSelector } from "react-redux";
import { basketSlice } from "../../feature/basket/basketSlice";
import { catalogSlice } from "../../feature/catalog/catalogSlice";
import { counterSlice } from "../../feature/contact/counterSlice";
import { accountSlice } from "../../feature/Account/accountSlice";

// export function configureStore() {
//   return createStore(counterReducer);
// }

export const store = configureStore({
  reducer: {
    counter: counterSlice.reducer,
    basket: basketSlice.reducer,
    catalog: catalogSlice.reducer,
    account: accountSlice.reducer
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export const useAppDispatch = () => useDispatch<AppDispatch>();
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;
